using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeLoader : MonoBehaviour {
	public int defaultRowAndColumnNumber = 3;
	private int mazeRows;
	private int mazeColumns;
	public GameObject wall;
	public int levelNumber = 0;

	public float size = 2f;
	private MazeCell[,] mazeCells;
	// Use this for initialization
	void Start () {
		mazeColumns = defaultRowAndColumnNumber + levelNumber;
		mazeRows = mazeColumns;
		InitializeMaze (); 
		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();
	}

	private void InitializeMaze() {
		mazeCells = new MazeCell[mazeRows,mazeColumns];
		// instantiate the player at the center position.
		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
				mazeCells [r, c] = new MazeCell ();

				// For now, use the same wall object for the floor!
				mazeCells [r, c].floor = Instantiate (wall, new Vector3 (r*size, -(size/2f), c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].floor.name = "Floor " + r + "," + c;
				mazeCells [r, c].floor.transform.Rotate (Vector3.right, 90f);

				if (c == 0) {
					mazeCells[r,c].westWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) - (size/2f)), Quaternion.identity) as GameObject;
					mazeCells [r, c].westWall.name = "West Wall " + r + "," + c;
				}

				mazeCells [r, c].eastWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) + (size/2f)), Quaternion.identity) as GameObject;
				mazeCells [r, c].eastWall.name = "East Wall " + r + "," + c;

				if (r == 0) {
					mazeCells [r, c].northWall = Instantiate (wall, new Vector3 ((r*size) - (size/2f), 0, c*size), Quaternion.identity) as GameObject;
					mazeCells [r, c].northWall.name = "North Wall " + r + "," + c;
					mazeCells [r, c].northWall.transform.Rotate (Vector3.up * 90f);
				}

				mazeCells[r,c].southWall = Instantiate (wall, new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells [r, c].southWall.transform.Rotate (Vector3.up * 90f);
			}
		}
	}

	public void setRowAndColumnNumber(){
		mazeColumns = defaultRowAndColumnNumber + levelNumber;
		mazeRows = mazeColumns;
	}

	public int getRowAndColumnNumber(){
		return mazeRows;
	}
}
