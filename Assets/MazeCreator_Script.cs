using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;

public class MazeCreator_Script : MonoBehaviour
{
    [SerializeField]
    int2 mazeSize = 20;

    [SerializeField, Tooltip("Use zero for random seed.")]
    int seed;

    [SerializeField, Range(0f, 1f)]
    float
        pickLastProbability = 0.5f,
        openDeadEndProbability = 0.5f,
        openArbitraryProbability = 0.5f;


    static Quaternion[] rotations =
        {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(0f, 0f, 180f),
        Quaternion.Euler(0f, 0f, 270f)
    };

    // [SerializeField]
    // MazeCellObject
    //     deadEnd, straight,
    //     cornerClosed, cornerOpen,
    //     tJunctionClosed, tJunctionOpenNE, tJunctionOpenSE, tJunctionOpen,
    //     xJunctionClosed, xJunctionOpenNE, xJunctionOpenNE_SE, xJunctionOpenNE_SW,
    //     xJunctionClosedNE, xJunctionOpen;

    [SerializeField]
    MazeCellObject orginalMazeWall;

    // MazeCellObject deadEnd;
    // MazeCellObject straight;
    // MazeCellObject corner;
    // MazeCellObject tJunction;
    // MazeCellObject empty;



    void GenerateMaze()
    {

        // deadEnd = Instantiate(orginalMazeWall);
        // DeactivateWalls(deadEnd, false, "South");

        // straight = orginalMazeWall;
        // DeactivateWalls(straight, false, "South");
        // DeactivateWalls(straight, false, "North");

        // corner = orginalMazeWall;
        // DeactivateWalls(corner, false, "South");
        // DeactivateWalls(corner, false, "East");

        // tJunction = orginalMazeWall;
        // DeactivateWalls(tJunction, false, "South");
        // DeactivateWalls(tJunction, false, "East");
        // DeactivateWalls(tJunction, false, "West");

        // empty = orginalMazeWall;
        // DeactivateWalls(empty, false, "South");
        // DeactivateWalls(empty, false, "East");
        // DeactivateWalls(empty, false, "West");
        // DeactivateWalls(empty, false, "North");

        // deadEnd = orginalMazeWall;
        // straight = orginalMazeWall;
        // corner = orginalMazeWall;
        // tJunction = orginalMazeWall;
        // empty = orginalMazeWall;


        Maze maze = new Maze(mazeSize);


        // seed = seed != 0 ? seed : UnityEngine.Random.Range(1, int.MaxValue),
        new FindDiagonalPassagesJob
        {
            maze = maze
        }.ScheduleParallel(
            maze.Length, maze.SizeEW, new GenerateMazeJob
            {
                maze = maze,
                seed = 1,
                pickLastProbability = pickLastProbability,
                openDeadEndProbability = openDeadEndProbability,
                openArbitraryProbability = openArbitraryProbability
            }.Schedule()
        ).Complete();


        Visualize(maze);
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    public void Visualize(Maze maze)
    {

        Debug.LogWarning("CREATED THE MAZE Of Length" + maze.Length);
        for (int i = 0; i < maze.Length; i++)
        {
            (string, int, MazeCellObject) prefabWithRotation = GetPrefab(maze[i]);

            // MazeCellObject instance = cellObjects[i]
            // = prefabWithRotation.Item1.GetInstance();

            MazeCellObject instance = prefabWithRotation.Item3.GetInstance();

            switch (prefabWithRotation.Item1)
            {
                case "deadEnd":
                    DeactivateWalls(instance, false, "South");
                    break;
                case "straight":
                    DeactivateWalls(instance, false, "South");
                    DeactivateWalls(instance, false, "North");

                    // DeactivateWalls(instance, false, "East"); //
                    // DeactivateWalls(instance, false, "West"); //
                    break;
                case "corner":
                    // DeactivateWalls(instance, false, "South");
                    // DeactivateWalls(instance, false, "West");

                    DeactivateWalls(instance, false, "North");
                    DeactivateWalls(instance, false, "East");

                    break;


                case "tJunction":
                    DeactivateWalls(instance, false, "North"); //
                    DeactivateWalls(instance, false, "South"); //
                    // DeactivateWalls(instance, false, "West");
                    DeactivateWalls(instance, false, "East");
                    break;



                case "empty":
                    DeactivateWalls(instance, false, "South");
                    DeactivateWalls(instance, false, "North");
                    DeactivateWalls(instance, false, "East");
                    DeactivateWalls(instance, false, "West");
                    break;
                default:
                    DeactivateWalls(instance, false, "South");
                    DeactivateWalls(instance, false, "North");
                    DeactivateWalls(instance, false, "East");
                    DeactivateWalls(instance, false, "West");
                    break;
            }

            instance.transform.SetPositionAndRotation(
                maze.IndexToWorldPosition2D(i), rotations[prefabWithRotation.Item2]
            );

        }
    }

    (string, int, MazeCellObject) GetPrefab(MazeFlags flags) => flags.StraightPassages() switch
    {
        MazeFlags.PassageN => ("deadEnd", 0, orginalMazeWall),
        MazeFlags.PassageE => ("deadEnd", 1, orginalMazeWall),
        MazeFlags.PassageS => ("deadEnd", 2, orginalMazeWall),
        MazeFlags.PassageW => ("deadEnd", 3, orginalMazeWall),

        MazeFlags.PassageN | MazeFlags.PassageS => ("straight", 0, orginalMazeWall),
        MazeFlags.PassageE | MazeFlags.PassageW => ("straight", 1, orginalMazeWall),

        MazeFlags.PassageN | MazeFlags.PassageE => ("corner", 0, orginalMazeWall),
        MazeFlags.PassageE | MazeFlags.PassageS => ("corner", 1, orginalMazeWall),
        MazeFlags.PassageS | MazeFlags.PassageW => ("corner", 2, orginalMazeWall),
        MazeFlags.PassageW | MazeFlags.PassageN => ("corner", 3, orginalMazeWall),

        MazeFlags.PassagesStraight & ~MazeFlags.PassageW => ("tJunction", 0, orginalMazeWall),
        MazeFlags.PassagesStraight & ~MazeFlags.PassageN => ("tJunction", 1, orginalMazeWall),
        MazeFlags.PassagesStraight & ~MazeFlags.PassageE => ("tJunction", 2, orginalMazeWall),
        MazeFlags.PassagesStraight & ~MazeFlags.PassageS => ("tJunction", 3, orginalMazeWall),

        _ => ("empty", 0, orginalMazeWall)
    };


    public void DeactivateWalls(MazeCellObject mazeCellObject, bool ActiveStatus, string Name)
    {
        foreach (Transform child in mazeCellObject.transform)
        {
            if (child.gameObject.name == Name)
            {
                child.gameObject.SetActive(ActiveStatus);
            }
        }
    }


}

