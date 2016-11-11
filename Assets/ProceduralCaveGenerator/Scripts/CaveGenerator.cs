/*
 * ==================================================================
 *  CaveGenerator.cs
 *  CaveGenerator
 *
 *  Created by Parag Padubidri
 *  Copyright (c) 2016, Parag Padubidri. All rights reserved.
 * ==================================================================
 */


using UnityEngine;
using System.Collections.Generic;

public static class CaveGenerator
{
    public static GameObject cave;
    static int[,] caveMap;
    static int width;
    static int height;
    static int threshold;
    static int seed;

    /// <summary>
    /// Generates a cave with the given parameters 
    /// </summary>
    /// <param name="caveWidth">Width of the cave</param>
    /// <param name="caveHeight">Height of the cave</param>
    /// <param name="wallFillPercent">Threshold Percentage of cave to be filled with walls</param>
    /// <param name="roomthreshold">Threshold value required for expected empty cells</param>
    /// <param name="caveSeed">Set to 0 for random caves or any other integer value for cave regeneration given same width and height</param>
    public static void CreateCaveMap(int caveWidth, int caveHeight, int wallFillPercent, int roomthreshold,int caveSeed)
    {
        //Initialize required values
        width = caveWidth;
        height = caveHeight;
        caveMap = new int[width, height];
        threshold = roomthreshold;
        seed = caveSeed;

        int smoothValue = 7; //Iterations to run on generated cave for surrounding wall checks

        //Generate random with or without seed 
        System.Random prng = (seed != 0) ? new System.Random(seed) : new System.Random();   

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Set border cells to walls
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    caveMap[x, y] = 1;
                }
                else
                {
                    caveMap[x, y] = ((prng.Next(1, 100) >= wallFillPercent) ? 0 : 1); //set walls inside grid depending on threshold value
                }
            }
        }


        //Smooth out the walls a few times
        SmoothWalls(smoothValue, false);

        //Fill Empty cell Islands with walls 
        FillMap();

        //Remove as many single Single cells as possible
        SmoothWalls(smoothValue, true);

        //Draw 3D Primitives for walls
        DrawCaveMap();                
    }


    /// <summary>
    /// Funtion to make multiple wall adjustments
    /// </summary>
    /// <param name="loopCount">Number of iterations to run</param>
    /// <param name="doCleanse">Try to reduce single cells?</param>
    static void SmoothWalls(int loopCount, bool doCleanse)
    {
        for (int i = 0; i < loopCount; i++)
        {
            AjustWalls(false);
        }
    }


    /// <summary>
    /// Function to Adjust walls
    /// </summary>
    /// <param name="doCleanse">Try to reduce single cells?</param>
    static void AjustWalls(bool doCleanse)
    {
        //Tweakable values
        int wallUpperThreshold = 5; 
        int wallLowerThreshold = 5;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Get Surrounding Walls Count 
                int wallCount = GetSurroundingWalls(x, y);

                if (!doCleanse)
                {
                    if (wallCount > wallUpperThreshold)
                    {
                        caveMap[x, y] = 1;
                    }
                    else if (wallCount < wallLowerThreshold)
                    {
                        caveMap[x, y] = 0;
                    } 
                }
                else
                {
                    if (wallCount == 0)
                    {
                        caveMap[x, y] = 0; //Get rid of single cells
                    }                    
                }
            }
        }
    }


    /// <summary>
    /// Function to get surrounding walls of a cell
    /// </summary>
    /// <param name="cellX">Row number of cell</param>
    /// <param name="cellY">Column number of cell</param>
    static int GetSurroundingWalls(int cellX, int cellY)
    {
        int surroundWalls = 0;
        for (int x = cellX - 1; x <= cellX + 1; x++)
        {
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                //Check bounds
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    //Filter self
                    if (x != cellX || y != cellY)
                    {
                        surroundWalls += caveMap[cellX, cellY];
                    }
                }
                else
                {
                    surroundWalls++;
                }
            }
        }

        return surroundWalls;
    }

    /// <summary>
    /// Function to Get biggest connected region based on threshold and fill the rest with walls
    /// </summary>
    static void  FillMap()
    {
        int loopFailSafeCount = 1000; // Number of tries before giving up to get desired cave

        //Get first random empty cell
        int[] startXY = isEmptyCell();
        int startX = startXY[0];
        int startY = startXY[1];

        //Run FloodFill function with random cell indices for given seed
        List<Cell> cells = FloodFill(startX, startY);

        int iterations = 1; //Set FailSafe loop count to 1

        while (cells.Count < threshold && seed == 0)
        {
            iterations++;

            startXY = isEmptyCell();
            startX = startXY[0];
            startY = startXY[1];

            //Run FloodFill function with random empty cell indices without seed
            cells = FloodFill(startX, startY);


            //Break if failed to get desired cave after trying too many times
            if (iterations > loopFailSafeCount)
            {
                //Log warning
                Debug.LogWarning("Falied to get desired cave. Try changing parameters and generate cave again.");
                break;
            }
        }

        //Log calculation output
        Debug.Log("Empty cell count : " + cells.Count + " iterations:  " + iterations);

        //Convert everything outside threshold region to walls
        foreach (Cell cell in cells)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x != cell.cellX && y != cell.cellY)
                    {                        
                        caveMap[x, y] = 1;
                    }
                }
            }
        }

        //Set everything not a wall back to empty cells
        foreach (Cell cell in cells)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == cell.cellX && y == cell.cellY)
                    {
                        caveMap[x, y] = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Function to return empty cell with or without seed
    /// </summary>
    static int[] isEmptyCell()
    {
        bool isEmpty = false;
        int randX;
        int randY;
        int[] emptyXY = new int[2];

        while (!isEmpty)
        {
            System.Random rand;
            rand = (seed != 0) ? new System.Random(seed) : new System.Random();           
                        
            randX = rand.Next(1, width);
            randY = rand.Next(1, height);

            if (caveMap[randX, randY] == 0 || seed != 0)
            {
                emptyXY[0] = randX;
                emptyXY[1] = randY;
                isEmpty = true;
            }
        }
        return emptyXY;
    }

    /// <summary>
    /// Function for FloodFill Algorithm
    /// </summary>
    /// <param name="startX">Row Number of Start cell value</param>
    /// <param name="startY">Column Number of Start cell value</param>  
    static List<Cell> FloodFill(int startX, int startY)
    {
        //Setup Cell List & 2D array for tracking cell visits with passed random cell indices
        List<Cell> cells = new List<Cell>();
        int[,] visitMap = new int[width, height];
        int startCell = caveMap[startX, startY];

        //Create Queue to store visited cells
        Queue<Cell> cellQueue = new Queue<Cell>();
        cellQueue.Enqueue(new Cell(startX, startY));

        visitMap[startX, startY] = 1;

        while(cellQueue.Count > 0)
        {
            Cell cell = cellQueue.Dequeue();
            cells.Add(cell);

            //Check Adjacent cells
            for (int x = cell.cellX - 1; x <= cell.cellX; x++)
            {
                for (int y = cell.cellY - 1; y <= cell.cellY; y++)
                {
                    //Filter out diagonal cells and check range
                    if ((x >= 0 && x < width && y >= 0 && y < height) && (x == cell.cellX || y == cell.cellY))
                    {
                        //Check if visited and cell type
                        if (visitMap[x,y] == 0 && caveMap[x,y] == startCell)
                        {
                            visitMap[x, y] = 1;
                            cellQueue.Enqueue(new Cell(x, y));
                        }
                    }
                }
            }
        }
        
        //Return empty cells' list
        return cells;
    }

    /// <summary>
    /// Function to generate 3D cave using CaveMap grid
    /// </summary>
    static void DrawCaveMap()
    {                
        cave = new GameObject();
        cave.name = "Cave";

        float wallOffset = 0.5f;

        //Setup cave ground and its material
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3((float)width / 10f, 1f, (float)height / 10f);
        ground.transform.parent = cave.transform;
        Material groundMat = Resources.Load("Ground_MAT") as Material;
        ground.GetComponent<Renderer>().material = groundMat;

        //Wall Material
        Material wallMat = Resources.Load("Wall_MAT") as Material;       

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveMap[x, y] == 1)
                {
                    //Generate and setup walls from cube primitives
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.name = "Wall";
                    wall.transform.position = new Vector3((float)(-(float)width / 2 + x) + wallOffset, wallOffset, (float)(-(float)height / 2 + y) + wallOffset); // offset wall position to start from ground and grid center
                    wall.transform.parent = cave.transform;
                    wall.GetComponent<Renderer>().material = wallMat;
                }
            }
        }
    }

    /// <summary>
    /// Function to remove single walls in generated CaveMap 
    /// </summary>
    public static void CleanCaveMap()
    {
        List<GameObject> walls = new List<GameObject>();

        float overlapBoxSize = 0.7f;
        int overlapCount = 3;

        int childCount = cave.transform.childCount;
        int singleWallsCount = 0;

        //Get list of all child walls
        for (int i = 0; i < childCount - 1; i++)
        {
            if (cave.transform.GetChild(i).transform.name == "Wall")
            {
                walls.Add(cave.transform.GetChild(i).transform.gameObject);
            }
        }

        //Detect collision with other walls if any
        for (int i = 0; i < walls.Count; i++)
        {
            Collider[] cols = Physics.OverlapBox(walls[i].transform.position, new Vector3(overlapBoxSize, overlapBoxSize, overlapBoxSize));

            //Destroy single wall  
            if (cols.Length < overlapCount)
            {
                singleWallsCount++; 
                UnityEngine.GameObject.DestroyImmediate(walls[i]);
            }
        }

        //Output Operation Log
        if (singleWallsCount == 0)
        {
            Debug.LogWarning("No single walls detected!");
        }
        else
        {
            Debug.Log("Deleted " + singleWallsCount + " walls");
        }
    }
}
