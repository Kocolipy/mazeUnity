using System.Collections.Generic;
using UnityEngine;

public class Maze{
    private List<List<byte>> Map;
    public int mWidth;
    public int mHeight;

    public Maze(int height, int width)
    {
        mWidth = width;
        mHeight = height;
        Map = new List<List<byte>>();
        for (int i = 0; i<height; i++)
        {
            List<byte> row = new List<byte>(new byte[width]);
            Map.Add(row);
        }
    }
    public Maze(Maze maze)
    {
        mWidth = maze.mWidth;
        mHeight = maze.mHeight;
        Map = new List<List<byte>>();
        for (int i = 0; i < mHeight; i++)
        {
            List<byte> row = new List<byte>();
            for (int j =0; j<mWidth; j++)
            {
                row.Add(maze.getValue(new Vector2Int(i, j)));
            }
            Map.Add(row);
        }
    }
    public byte getValue(Vector2Int coord){ return Map[coord.x][coord.y]; }
    public void addTile(Vector2Int coord, byte value) { Map[coord.x][coord.y] = value; }
    public bool isEmpty(Vector2Int coord){ return Map[coord.x][coord.y] == 0; }
    public byte isCorner(Vector2Int coord)
    {
        if (coord.x == 0) return 1;
        if (coord.x == mHeight-1) return 4;
        if (coord.y == 0) return 8;
        if (coord.y == mWidth - 1) return 2;
        else return 0;
    }
    public List<bool> getOpenings(Vector2Int coord)
    {
        /*  Return a list of 4 booleans
         *  where true is presence of an opening
         *  [West, South, East, North]
         */
        List<bool> arr = new List<bool>();
        byte val = getValue(coord);
        //Check openings
        arr.Add(val >= 8 ? true : false);
        arr.Add(val %8 >= 4 ? true : false);
        arr.Add(val %4 >= 2 ? true : false);
        arr.Add(val %2 >= 1 ? true : false);
        return arr;
    }
    public int getEmptiness()
    {
        //Return the number of empty cells
        int val = 0;
        foreach (List<byte> row in Map)
        {
            foreach (byte b in row) val += b == 0 ? 1 : 0;
        }
        return val;
    }
    public List<Vector2Int> getOccupied()
    {
        List<Vector2Int> occupied = new List<Vector2Int>();
        Vector2Int tmp;
        for (int i =0; i<mHeight; i++)
        {
            for (int j=0; j<mWidth; j++)
            {
                tmp = new Vector2Int(i, j);
                if (getValue(tmp) != 0) { occupied.Add(tmp); }
            }
        }
        return occupied;
    }
}
