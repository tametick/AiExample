using System.Collections.Generic;
using Pathfinding;
using System;

public class Pathfinder {
	List<List<GraphNode>> pathfindingGrid;
	private SimplePathfinding pathfinder;
	
	Pathfinder() {
		pathfinder = new SimplePathfinding();
	}

	List<List<GraphNodeType>> ListFromArray (int[,] tiles, List<int> open)	{
		var graphInputGrid = new List<List<GraphNodeType>> ();
		for (int x = 0; x < tiles.GetLength(0); ++x) {
			graphInputGrid.Add (new List<GraphNodeType> ());
			for (int y = 0; y < tiles.GetLength(1); ++y) {
				if (open.Contains(tiles [x, y])) {
					graphInputGrid [x].Add (GraphNodeType.OPEN);
				}
				else {
					graphInputGrid [x].Add (GraphNodeType.WALL);
				}
			}
		}
		return graphInputGrid;
	}
	
	public void Init(int[,] tiles, List<int> open, Func<int,int, Direction, bool> hasWall,  Func<int,int, bool> blockedByOthers) {
		// hasWall(x,y,dir) returns if there is a wall to direction dir of <x,y>
		pathfinder.hasWall = hasWall;
		// blockedByOthers(x,y) returns if <x,y> is blocked by another actor
		pathfinder.blockedByOthers = blockedByOthers; 
		var graphInputGrid = ListFromArray (tiles, open);
		Graph graph = new Graph(graphInputGrid);
		pathfindingGrid = graph.nodes;
	}
	
	public GraphNode GetNode(int x, int y){
		return pathfindingGrid[x][y];
	}
	
	public List<GraphNode> Search(IntVector2 start, IntVector2 end,bool avoidOthers=false) {
		return pathfinder.Search(pathfindingGrid, GetNode(start.x,start.y), GetNode(end.x,end.y), false, null, avoidOthers);
	}
}
