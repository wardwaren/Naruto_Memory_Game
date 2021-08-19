using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    private int width;
    private int height;
    private int[,] gridArray;
    
    public GridMap(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridArray = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
            }
        }
    }
}
