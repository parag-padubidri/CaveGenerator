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

    public static void CreateCaveMap(int width, int height, int wallFillPercent, int seed)
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

        DrawCaveMap(caveMap);
    }

    private static void AjustWalls(int[,] caveMap)
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

    static int GetSurroundingWalls(int x, int y, int width, int height)
    {
        int surroundWalls = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                //Check bounds
                if (i >= 0 && i < width && j >= 0 && j < height)
                {
                    //Filter self
                    if (i != x || j != y)
                    {
                        surroundWalls += caveMap[x, y];
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

    static List<Cell> FloodFill()
    {
        return null;
    } 

    private static void DrawCaveMap(int[,] caveMap)
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
