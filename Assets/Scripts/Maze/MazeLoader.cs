using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// MazeLoader class for positioning prefabs, in line with what the values received from the ProceduralNumberGenerator.
/// </summary>
[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(NavMeshSurface))]
public class MazeLoader : MonoBehaviour
{
    [Tooltip("The wall game object prefab.")]
    public GameObject wall;
    [Tooltip("The ground object prefab.")]
    public GameObject ground;
    [Tooltip("Script reference to the NavMeshSurface.")]
    public NavMeshSurface surface;
    private int mazeRows;
    private int mazeColumns;
    private int levelNumber = 0;

    public float size;
    private MazeCell[,] mazeCells;
    // Use this for initialization
    void Start()
    {   
        if (LevelDifficulty.levelDifficulty == 0)
        {
            setRowAndColumnNumber(3);
        }
        else
        {
            setRowAndColumnNumber(LevelDifficulty.levelDifficulty + 3);
        }

        InitializeMaze();
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze();

        // bake nav mesh
        surface.BuildNavMesh();
    }

    private void InitializeMaze()
    {
        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell();

                // For now, use the same wall object for the floor!
                mazeCells[r, c].floor = Instantiate(ground, new Vector3(r * size, -(size / 2f), c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].floor.name = "Floor " + r + "," + c;
                mazeCells[r, c].floor.transform.Rotate(Vector3.right, 90f);

                if (c == 0)
                {
                    mazeCells[r, c].westWall = Instantiate(wall, new Vector3(r * size, 0, (c * size) - (size / 2f)), Quaternion.identity) as GameObject;
                    mazeCells[r, c].westWall.name = "West Wall " + r + "," + c;
                }

                mazeCells[r, c].eastWall = Instantiate(wall, new Vector3(r * size, 0, (c * size) + (size / 2f)), Quaternion.identity) as GameObject;
                mazeCells[r, c].eastWall.name = "East Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].northWall = Instantiate(wall, new Vector3((r * size) - (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                    mazeCells[r, c].northWall.name = "North Wall " + r + "," + c;
                    mazeCells[r, c].northWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].southWall = Instantiate(wall, new Vector3((r * size) + (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].southWall.name = "South Wall " + r + "," + c;
                mazeCells[r, c].southWall.transform.Rotate(Vector3.up * 90f);
            }
        }
    }

    public void setRowAndColumnNumber(int levelDifficulty = 3)
    {
        mazeColumns = levelDifficulty + levelNumber;
        mazeRows = mazeColumns;
    }

    public int getRowAndColumnNumber()
    {
        return mazeRows;
    }
}
