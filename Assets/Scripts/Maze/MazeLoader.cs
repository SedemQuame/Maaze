using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// MazeLoader class for positioning prefabs, in line with what the values received from the ProceduralNumberGenerator.
/// </summary>
[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(NavMeshSurface))]
public class MazeLoader : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public float size;
    public int levelWallIndex = 0;
    [Tooltip("The walls game object prefab.")]
    public GameObject[] walls;
    [Tooltip("The ground object prefab.")]
    public GameObject ground;
    [Tooltip("Script reference to the NavMeshSurface.")]
    public NavMeshSurface surface;

    // ===============PRIVATE VARIABLES===============
    [SerializeField]
    private int mazeRows, mazeColumns, levelNumber = 0;
    private MazeCell[,] mazeCells;

    void Start()
    {   
        // change wall index based on level category.
        if (1 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 8)
        {            
            levelWallIndex = 0;
        }else if (9 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 16)
        {
            levelWallIndex = 1;
        }else if (17 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 24)
        {
            levelWallIndex = 2;
        }else if (25 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 32)
        {
            levelWallIndex = 3;
        }else if (33 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 40)
        {
            levelWallIndex = 4;
        }
        else if (41 <= LevelDifficulty.levelDifficulty && LevelDifficulty.levelDifficulty <= 48)
        {
            levelWallIndex = 5;
        }else{
            levelNumber = 6;
        }

        if (LevelDifficulty.levelDifficulty == 0)
        {
            setRowAndColumnNumber(3);
        }
        else
        {
            setRowAndColumnNumber(LevelDifficulty.levelDifficulty + 1);
        }

        InitializeMaze();
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze();

        // bake nav mesh
        surface.BuildNavMesh();
    }

    void InitializeMaze()
    {
        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell();

                // For now, use the same walls object for the floor!
                mazeCells[r, c].floor = Instantiate(ground, new Vector3(r * size, -(size / 2f), c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].floor.name = "Floor " + r + "," + c;
                mazeCells[r, c].floor.transform.Rotate(Vector3.right, 90f);

                if (c == 0)
                {
                    mazeCells[r, c].westWall = Instantiate(walls[levelWallIndex], new Vector3(r * size, 0, (c * size) - (size / 2f)), Quaternion.identity) as GameObject;
                    mazeCells[r, c].westWall.name = "West Wall " + r + "," + c;
                }

                mazeCells[r, c].eastWall = Instantiate(walls[levelWallIndex], new Vector3(r * size, 0, (c * size) + (size / 2f)), Quaternion.identity) as GameObject;
                mazeCells[r, c].eastWall.name = "East Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].northWall = Instantiate(walls[levelWallIndex], new Vector3((r * size) - (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                    mazeCells[r, c].northWall.name = "North Wall " + r + "," + c;
                    mazeCells[r, c].northWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].southWall = Instantiate(walls[levelWallIndex], new Vector3((r * size) + (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].southWall.name = "South Wall " + r + "," + c;
                mazeCells[r, c].southWall.transform.Rotate(Vector3.up * 90f);
            }
        }
    }

    public void setRowAndColumnNumber(int levelDifficulty = 3)
    {
        // change the value for columns and rows based on, section strategy.
        if(LevelDifficulty.levelDifficulty <= 4){
            mazeRows = mazeColumns = 4;
        }else if(LevelDifficulty.levelDifficulty > 4 && LevelDifficulty.levelDifficulty <= 8){
            mazeRows = mazeColumns = 5;
        }else if(LevelDifficulty.levelDifficulty > 8 && LevelDifficulty.levelDifficulty <= 16)
        {
            mazeRows = mazeColumns = LevelDifficulty.levelDifficulty - 3;
        }else if(LevelDifficulty.levelDifficulty > 16 && LevelDifficulty.levelDifficulty <= 40)
        {
            mazeRows = mazeColumns = 13;
        }
        else if(LevelDifficulty.levelDifficulty > 40 && LevelDifficulty.levelDifficulty <= 48)
        {
            mazeRows = mazeColumns = LevelDifficulty.levelDifficulty - 27;
        }else if(LevelDifficulty.levelDifficulty > 48 && LevelDifficulty.levelDifficulty <= 50)
        {
            mazeRows = mazeColumns = 21;
        }
        // mazeColumns = levelDifficulty + levelNumber;
        // mazeRows = mazeColumns;
    }

    public int getRowAndColumnNumber()
    {
        return mazeRows;
    }
}
