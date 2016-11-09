using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public static class CaveGenerator
{
    public static GameObject cave;
    static int[,] caveMap;

    public static void CreateCaveMap(int width, int height, int wallFillPercent, int threshold,int seed)
    {
        Debug.Log("Creating...");
        caveMap = new int[width, height];

        System.Random prng = new System.Random(seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    caveMap[x, y] = 1;
                }
                else
                {
                    caveMap[x, y] = ((prng.Next(1, 100) >= wallFillPercent) ? 0 : 1);
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            AjustWalls(caveMap);
        }

        for (int i = 0; i < 3; i++)
        {
            AjustWalls(caveMap);
        }
        FillMap(width, height, threshold);
        DrawCaveMap(caveMap);
    }

    static void AjustWalls(int[,] caveMap)
    {
        int width = caveMap.GetLength(0);
        int height = caveMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Get Surrounding Walls Count 
                int wallCount = GetSurroundingWalls(x, y, width, height);

                if (wallCount > 5)
                {
                    caveMap[x, y] = 1;
                }
                else if (wallCount < 2)
                {
                    caveMap[x, y] = 0;
                }
            }
        }
    }

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

    //Cell Culling
    static void  FillMap(int width, int height, int threshold)
    {
        int emptyCellThreshold = threshold;

        //Get first random empty cell
        int[] startXY = isEmptyCell(width, height);
        int startX = startXY[0];
        int startY = startXY[1];

        List<Cell> cells = FloodFill(startX, startY, width, height);

        int iterations = 0;

        while (cells.Count < emptyCellThreshold)
        {
            iterations++;

            startXY = isEmptyCell(width, height);
            startX = startXY[0];
            startY = startXY[1];

            cells = FloodFill(startX, startY, width, height);


            //Break if failed to get desired cave after trying too many times
            if (iterations > 1000)
            {
                Debug.Log("Falied to get desired cave. Try changing parameters & try again.");
                break;
            }
        }

        Debug.Log("Empty cell count : " + cells.Count + " iterations:  " + iterations);

        //Convert everything outside threshold region to walls
        foreach (Cell cell in cells)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if ((x != cell.cellX && y != cell.cellY))
                    {                        
                        caveMap[x, y] = 1;
                    }
                }
            }
        }

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

    //Check empty cell
    static int[] isEmptyCell(int width, int height)
    {
        bool isEmpty = false;
        int randX;
        int randY;
        int[] emptyXY = new int[2];

        while (!isEmpty)
        {
            System.Random rand =  new System.Random();
                        
            randX = rand.Next(1, width);
            randY = rand.Next(1, height);

            if (caveMap[randX, randY] == 0)
            {
                emptyXY[0] = randX;
                emptyXY[1] = randY;
                isEmpty = true;
            }
        }

        return emptyXY;
    }

    //FloodFill Algorithm
    static List<Cell> FloodFill(int startX, int startY, int width, int height)
    {
        List<Cell> cells = new List<Cell>();
        int[,] visitMap = new int[width, height];
        int startCell = caveMap[startX, startY];

        Queue<Cell> cellQueue = new Queue<Cell>();
        cellQueue.Enqueue(new Cell(startX, startY));

        visitMap[startX, startY] = 1;

        while(cellQueue.Count > 0)
        {
            Cell cell = cellQueue.Dequeue();
            cells.Add(cell);

            for (int x = cell.cellX - 1; x <= cell.cellX; x++)
            {
                for (int y = cell.cellY - 1; y <= cell.cellY; y++)
                {
                    //Filter in range & diagonal cells
                    if ((x >= 0 && x < width && y >= 0 && y < height) && (x == cell.cellX || y == cell.cellY))
                    {
                        if (visitMap[x,y] == 0 && caveMap[x,y] == startCell)
                        {
                            visitMap[x, y] = 1;
                            cellQueue.Enqueue(new Cell(x, y));
                        }
                    }
                }
            }
        }

        return cells;
    } 

    static void DrawCaveMap(int[,] caveMap)
    {
        int width = caveMap.GetLength(0);
        int height = caveMap.GetLength(1);

        cave = new GameObject();
        cave.name = "Cave";

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3((float)width / 10f, 1f, (float)height / 10f);
        ground.transform.parent = cave.transform;
        Material groundMat = Resources.Load("Ground_MAT") as Material;
        ground.GetComponent<Renderer>().material = groundMat;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveMap[x, y] == 1)
                {
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.name = "Wall";
                    wall.transform.position = new Vector3((float)(-(float)width / 2 + x) + 0.5f, 0.5f, (float)(-(float)height / 2 + y) + 0.5f);
                    wall.transform.parent = cave.transform;
                }
            }
        }
    }
}
