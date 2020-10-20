using UnityEngine;
using System.Collections.Generic;
using System;
using ExtensionMethods;

namespace Pathfinding {
	// based on javascript-astar
	// http://github.com/bgrins/javascript-astar
	// Freely distributable under the MIT License.
	// Includes Binary Heap (with modifications) from Marijn Haverbeke. 
	// http://eloquentjavascript.net/appendix2.html

	public enum GraphNodeType { 
		OPEN=0,
		WALL=1,
	}
	
	public class Graph {
	
		public List<List<GraphNode>> nodes;
	
		public Graph(List<List<GraphNodeType>> grid){
			var nodes = new List<List<GraphNode>>();
			
			for (var x = 0; x < grid.Count; x++) {
				nodes.Add(new List<GraphNode>());
				List<GraphNodeType> row= grid[x];
				
				for (var y = 0; y < row.Count; y++) {
					nodes[x].Add(new GraphNode(x, y, row[y])); 
				}
			}
			
			this.nodes = nodes;
		}
		
		override public string ToString(){
			var graphString = "\n";
			var nodes = this.nodes;
			string rowDebug;
			List<GraphNode> row; 
			int y;
			int l;
			var len = nodes.Count;
			for (var x = 0; x < len; x++) {
				rowDebug = "";
				row = nodes[x];
				for (y = 0, l = row.Count; y < l; y++) {
					rowDebug += row[y].type + " ";
				}
				graphString = graphString + rowDebug + "\n";
			}
			return graphString;
		}
	}
	
	public class GraphNode {
		public int x;
		public int y;
		public Vector2 pos;
		public GraphNodeType type;
		
		public int f = 0;
		public int g = 0;
		public int h = 0;
		public int cost;
		public bool visited;
		public bool closed;
		public GraphNode parent;
	
		public GraphNode(int x,int y,GraphNodeType type){
			this.x = x;
			this.y = y;
			this.type = type;
			
			pos = new Vector2(x,y);
		}
		
		override public string ToString(){
			return "[" + this.x + " " + this.y + "]";
		}
		
		public bool IsWall(){
			return type == GraphNodeType.WALL;
		}
	}
	
	public class BinaryHeap {
		private List<GraphNode> content;
		private Func<GraphNode,int> scoreFunction;
		
		public BinaryHeap(Func<GraphNode,int> scoreFunction){
			this.scoreFunction = scoreFunction;
			content = new List<GraphNode>();
		}
		
		public void Push(GraphNode element){
			// Add the new element to the end of the array.
			this.content.Add(element);
			
			// Allow it to sink down.
			this.SinkDown(this.content.Count - 1);
        }
        
        public GraphNode Pop(){
			// Store the first element so we can return it later.
			var result = this.content[0];
			// Get the element at the end of the array.
			var end = this.content.Pop();
			// If there are any elements left, put the end element at the
			// start, and let it bubble up.
			if (this.content.Count > 0) {
				this.content[0] = end;
				this.BubbleUp(0);
            }
            return result;
        }
        
        public void Remove(GraphNode node){
			var i = this.content.IndexOf(node);
			
			// When it is found, the process seen in 'pop' is repeated
			// to fill up the hole.
			var end = this.content.Pop();
			
			if (i != (this.content.Count - 1)) {
				this.content[i] = end;
				
				if (this.scoreFunction(end) < this.scoreFunction(node)) {
					this.SinkDown(i);
				}
				else {
                    this.BubbleUp(i);
                }
            }
        }
        
        public int Size(){
            return content.Count;
        }
        
        public void RescoreElement(GraphNode node){
			this.SinkDown(this.content.IndexOf(node));
		}
		
		public void SinkDown(int n){
			// Fetch the element that has to be sunk.
			var element = this.content[n];
			
			// When at 0, an element can not sink any further.
			while (n > 0) {
				
				// Compute the parent element's index, and fetch it.
				int parentN = ((n + 1) >> 1) - 1;
				GraphNode parent = this.content[parentN];
				// Swap the elements if the parent is greater.
				if (this.scoreFunction(element) < this.scoreFunction(parent)) {
					this.content[parentN] = element;
					this.content[n] = parent;
					// update 'n' to continue at the new position.
					n = parentN;
				}
				
				// Found a parent that is less, no need to sink any further.
                else {
                    break;
                }
            }
        }
        
		GraphNodeType graphNodeType(int g){
			if(g==0)
				return GraphNodeType.OPEN;
			else 
				return GraphNodeType.WALL;
		}
            
        public void BubbleUp(int n){
			// Look up the target element and its score.
			var length = this.content.Count;
			var element = this.content[n];
			var elemScore = this.scoreFunction(element);
			
			while(true) {
				// Compute the indices of the child elements.
				int child2N = (n + 1) << 1;
				int child1N = child2N - 1;
				// This is used to store the new position of the element,
				// if any.
				int? swap = null;
				// If the first child exists (is inside the array)...
				int child1Score=0;
				if (child1N < length) {
					// Look it up and compute its score.
					var child1 = this.content[child1N];
					child1Score = this.scoreFunction(child1);
					
					// If the score is less than our element's, we need to swap.
					if (child1Score < elemScore)
						swap = child1N;
				}
				
				// Do the same checks for the other child.
				if (child2N < length) {
					var child2 = this.content[child2N];
					var child2Score = this.scoreFunction(child2); 
					if (child2Score < (swap == null ? elemScore : child1Score)) {
						swap = child2N;
					}
				}
				
				// If the element needs to be moved, swap it, and continue.
				if (swap != null) {
					this.content[n] = this.content[(int)swap];
					this.content[(int)swap] = element;
                    n = (int)swap;
                }
                
                // Otherwise, we are done.
                else {
                    break;
                }
            }
        }
    }
    
    public class SimplePathfinding  {
		public Func<int,int, Direction, bool> hasWall;
		public Func<int,int, bool> blockedByOthers;

    	private int TypeToInt(GraphNodeType type){
			if(type==GraphNodeType.OPEN)
				return 0;
			else 
				return 1;
        }
    
		private void Init(List<List<GraphNode>> grid){
			var xl = grid.Count;
            for(var x = 0; x < xl; x++) {
				var yl = grid[x].Count;
                for(var y = 0; y < yl; y++) {
					var node = grid[x][y];
					node.f = 0;
					node.g = 0;
					node.h = 0;
					node.cost = TypeToInt(node.type);
					node.visited = false;
					node.closed = false;
					node.parent = null;
                }
            }
        }
        
        private BinaryHeap Heap(){
			Func<GraphNode,int> func = delegate(GraphNode node){
				return node.f; 
            };
        	return new BinaryHeap(func);
        }
        
        public List<GraphNode> Search(List<List<GraphNode>> grid, GraphNode start, GraphNode end, bool diagonal=false, 
		                              Func<List<Vector2>, int> heuristic=null, bool avoidOthers=false){
			if(start.IsWall()) {
				return null;
			}
			
			Init(grid);
			heuristic = (heuristic==null)?Manhattan:heuristic;
			diagonal = !!diagonal;
			
			var openHeap = Heap();
			
			openHeap.Push(start);
			
			while(openHeap.Size() > 0) {
				
				// Grab the lowest f(x) to process next.  Heap keeps this sorted for us.
				var currentNode = openHeap.Pop();
				
				// End case -- result has been found, return the traced path.
				if(currentNode == end) {
					var curr = currentNode;
					var ret = new List<GraphNode>();
					while(curr.parent!=null) {
						ret.Add(curr);
						curr = curr.parent;
					}
					ret.Reverse();
					return ret;
				}
				
				// Normal case -- move currentNode from open to closed, process each of its neighbors.
				currentNode.closed = true;
				
				// Find all neighbors for the current node. Optionally find diagonal neighbors as well (false by default).
				var neighbors = Neighbors(grid, currentNode, diagonal, avoidOthers);
				
				var il = neighbors.Count;
				for(var i=0; i < il; i++) {
					var neighbor = neighbors[i];
					
					if(neighbor.closed || neighbor.IsWall()) {
						// Not a valid node to process, skip to next neighbor.
						continue;
					}
					
					// The g score is the shortest distance from start to current node.
					// We need to check if the path we have arrived at this neighbor is the shortest one we have seen yet.
					var gScore = currentNode.g + neighbor.cost;
					var beenVisited = neighbor.visited;
					
					if(!beenVisited || gScore < neighbor.g) {
						
						// Found an optimal (so far) path to this node.  Take score for node to see how good it is.
						neighbor.visited = true;
						neighbor.parent = currentNode;
						var pos = new List<Vector2>();
						pos.Add(neighbor.pos);
						pos.Add(end.pos);
						// todo - make sure this is really what the original js code does
						if(neighbor.h==0)
							neighbor.h = heuristic(pos);
						//
						
						neighbor.g = gScore;
						neighbor.f = neighbor.g + neighbor.h;
						
						if (!beenVisited) {
							// Pushing to heap will put it in proper place based on the 'f' value.
							openHeap.Push(neighbor);
                        }
                        else {
                            // Already seen the node, but since it has been rescored we need to reorder it in the heap
                            openHeap.RescoreElement(neighbor);
                        }
                    }
                }
            }
            
            // No result was found - empty array signifies failure to find path.
			return null;
        }
        
        private int Manhattan(List<Vector2> pos){
            // See list of heuristics: http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
            
			var pos0 = pos[0];
			var pos1 = pos[1];
				
            var d1 = Math.Abs (pos1.x - pos0.x);
            var d2 = Math.Abs (pos1.y - pos0.y);
            return Mathf.RoundToInt(d1 + d2);
        }
        
		private List<GraphNode> Neighbors(List<List<GraphNode>> grid, GraphNode node, bool diagonals, bool avoidOthers){
			//todo - avoid others

            var ret = new List<GraphNode>();
            var x = node.x;
            var y = node.y;
            
            // West
			if(grid.ValidIndex(x-1) && grid[x-1].ValidIndex(y) && !hasWall(x-1,y,Direction.Right) && (!avoidOthers || !blockedByOthers(x-1,y))) {
				ret.Add(grid[x-1][y]);
			}
			
			// East
			if(grid.ValidIndex(x+1) && grid[x+1].ValidIndex(y) && !hasWall(x,y,Direction.Right) && (!avoidOthers || !blockedByOthers(x+1,y))) {
				ret.Add(grid[x+1][y]);
			}
			
			// South
			if(grid.ValidIndex(x) && grid[x].ValidIndex(y-1) && !hasWall(x,y-1,Direction.Up) && (!avoidOthers || !blockedByOthers(x,y-1))) {
				ret.Add(grid[x][y-1]);
			}
			
			// North
			if(grid.ValidIndex(x) && grid[x].ValidIndex(y+1) && !hasWall(x,y,Direction.Up) && (!avoidOthers || !blockedByOthers(x,y+1))) {
				ret.Add(grid[x][y+1]);
			}
			
			if (diagonals) {
				
				// Southwest
				if(grid.ValidIndex(x-1) && grid[x-1].ValidIndex(y-1)) {
					ret.Add(grid[x-1][y-1]);
				}
				
				// Southeast
				if(grid.ValidIndex(x+1) && grid[x+1].ValidIndex(y-1)) {
					ret.Add(grid[x+1][y-1]);
				}
				
				// Northwest
				if(grid.ValidIndex(x-1) && grid[x-1].ValidIndex(y+1)) {
					ret.Add(grid[x-1][y+1]);
                }
                
                // Northeast
				if(grid.ValidIndex(x+1) && grid[x+1].ValidIndex(y+1)) {
					ret.Add(grid[x+1][y+1]);
                }
                
            }
            
            return ret;
        }
    }
}
