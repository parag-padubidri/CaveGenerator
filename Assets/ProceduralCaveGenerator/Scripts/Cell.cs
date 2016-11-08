using UnityEngine;
using System.Collections;

public class Cell
{
    public int cellX;
    public int cellY;

    public bool isVisited = false;

    public Cell(int x, int y)
    {
        cellX = x;
        cellY = y;
    }
}
