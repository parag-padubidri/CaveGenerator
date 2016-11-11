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

    public static void CreateCaveMap(int width, int height, int wallFillPercent, int threshold,int seed)
    {
        caveMap = new int[width, height];

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
        SmoothWalls(7, false);

        //Fill Empty cell Islands with walls 
        FillMap(width, height, threshold, seed);

        //Remove as many single Single walls as possible
        SmoothWalls(500, true);

        //Draw 3D Primitives for walls
        DrawCaveMap(caveMap);
    }
 
    //Funtion to make multiple wall adjustments
    static void SmoothWalls(int loopCount, bool doCleanse)
    {
        for (int i = 0; i < loopCount; i++)
        {
            AjustWalls(caveMap, false);
        }
    }

    //Function to Adjust walls
    static void AjustWalls(int[,] caveMap, bool doCleanse)
    {
        int width = caveMap.GetLength(0);
        int height = caveMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Get Surrounding Walls Count 
                int wallCount = GetSurroundingWalls(x, y, width, height);

                if (!doCleanse)
                {
                    if (wallCount > 5)
                    {
                        caveMap[x, y] = 1;
                    }
                    else if (wallCount < 5)
                    {
                        caveMap[x, y] = 0;
                    } 
                }
                else
                {
                    if (wallCount == 0)
                    {
                        caveMap[x, y] = 0; //Get rid of single walls
                    }                    
                }
            }
        }
    }

    //Function to get surrounding walls of a cell
    static int GetSurroundingWalls(int cellX, int cellY, int width, int height)
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

    //Function to Get biggest connected region based on threshold and fill the rest with walls
    static void  FillMap(int width, int height, int threshold, int emptySeed)
    {
        int emptyCellThreshold = threshold;

        //Get first random empty cell
        int[] startXY = isEmptyCell(width, height, emptySeed);
        int startX = startXY[0];
        int startY = startXY[1];

        //Run FloodFill function with random cell indices for given seed
        List<Cell> cells = FloodFill(startX, startY, width, height);

        int iterations = 1;

        while (cells.Count < emptyCellThreshold && emptySeed == 0)
        {
            iterations++;

            startXY = isEmptyCell(width, height, 0);
            startX = startXY[0];
            startY = startXY[1];

            //Run FloodFill function with random empty cell indices without seed
            cells = FloodFill(startX, startY, width, height);


            //Break if failed to get desired cave after trying too many times
            if (iterations > 1000)
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
                    if ((x == cell.cellX && y == cell.cellY))
                    {
                        caveMap[x, y] = 0;
                    }
                }
            }
        }
    }

    //Function to Check empty cell with or without seed
    static int[] isEmptyCell(int width, int height, int emptySeed)
    {
        bool isEmpty = false;
        int randX;
        int randY;
        int[] emptyXY = new int[2];
        
        bool isUsingSeed = false;

        if (emptySeed != 0)
        {
            isUsingSeed = true;
        }

        while (!isEmpty)
        {
            System.Random rand;
            if (isUsingSeed)
            {
                rand = new System.Random(emptySeed);
            }
            else
            {
                rand = new System.Random();
            }            
                        
            randX = rand.Next(1, width);
            randY = rand.Next(1, height);

            if (caveMap[randX, randY] == 0 || isUsingSeed)
            {
                emptyXY[0] = randX;
                emptyXY[1] = randY;
                isEmpty = true;
            }
        }
        return emptyXY;
    }

    //Function for FloodFill Algorithm
    static List<Cell> FloodFill(int startX, int startY, int width, int height)
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

    //Function to generate 3D cave using CaveMap grid
    static void DrawCaveMap(int[,] caveMap)
    {
        int width = caveMap.GetLength(0);
        int height = caveMap.GetLength(1);
                
        cave = new GameObject();
        cave.name = "Cave";

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
                    wall.transform.position = new Vector3((float)(-(float)width / 2 + x) + 0.5f, 0.5f, (float)(-(float)height / 2 + y) + 0.5f);
                    wall.transform.parent = cave.transform;
                    wall.GetComponent<Renderer>().material = wallMat;
                }
            }
        }
    }

    //Function to remove single walls in generated CaveMap
    public static void CleanCaveMap()
    {
        List<GameObject> walls = new List<GameObject>();

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
            Collider[] cols = Physics.OverlapBox(walls[i].transform.position, new Vector3(0.7f,0.7f,0.7f));

            //Destroy single wall  
            if (cols.Length < 3)
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
