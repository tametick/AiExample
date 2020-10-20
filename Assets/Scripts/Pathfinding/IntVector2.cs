using UnityEngine;

public class IntVector2  {
	public int x;
	public int y;

	public IntVector2(){
	}

	public IntVector2(Vector2 vect){
		this.x = (int)vect.x;
		this.y = (int)vect.y;
	}

	public IntVector2(int x, int y){
		this.x = x;
		this.y = y;
	}

	public IntVector2 Clone () {
		return new IntVector2 (x, y);
	}

	public override string ToString () {
		return string.Format ("{0},{1}",x,y);
	}

	public Vector2 toVec2(){
		return new Vector2 (x, y);
	}

	public static IntVector2 FromString (string vectStr) {
		var xAndY = vectStr.Split (",".ToCharArray ());
		int x = int.Parse (xAndY [0].Substring (1));
		var yStr = xAndY [1].Replace('>',' ');
		int y = int.Parse (yStr);

		return new IntVector2 (x, y);
	}

	public override int GetHashCode(){
		// use a large number, bigger than your largest level
		return 10000*y+x;
	}

	public bool Equals (IntVector2 other) {
		return x==other.x && y==other.y;
	}
	
	public bool Equals (Vector2 other) {
		return x==other.x && y==other.y;
	}
	
	public bool Equals (int ox, int oy) {
		return x==ox && y==oy;
	}
	
	public Direction ToDirection(){
		var absX = Mathf.Abs (x);
		var absY = Mathf.Abs (y);
		if (absX > absY) {			
			if (x < 0) {
				return Direction.Left;
			}
			else {
				return Direction.Right;
			}
		} else {
			if (y < 0) {
				return Direction.Down;
			}
			else {
				return Direction.Up;
			}
		}
	}
}
