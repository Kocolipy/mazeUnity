using UnityEngine;
using System.Collections.Generic;


public static class MazeGenerator
{
    private static int cornerTileNum = 0;
    private static double spaceTolerance = 0.4;
    private static double secondProb;
    private static double thirdProb;

    private static List<bool> findAdjacentCells(Maze maze, Vector2Int coord)
    {
        /* Given a empty cell, 
         * update the cell with its surrounding tile values
         * return a list of four boolean where true represent that that direction is empty
         * [West, South, East, North]
         * */

        byte value = 0;
        List<bool> openings = new List<bool>(new bool[4]);

        //Western side check
        if (coord.y != 0)
        {
            Vector2Int west = new Vector2Int(coord.x, coord.y - 1);
            if (!maze.isEmpty(west))
            {
                if (maze.getOpenings(west)[2]) value += 8;
            }
            else openings[0] = true;
        }
        //Southern side check
        if (coord.x != maze.mHeight-1)
        {
            Vector2Int south = new Vector2Int(coord.x+1, coord.y);
            if (!maze.isEmpty(south))
            {
                if (maze.getOpenings(south)[3]) value += 4;
            }
            else openings[1] = true;
        }
        //Eastern side check
        if (coord.y != maze.mWidth - 1)
        {
            Vector2Int east = new Vector2Int(coord.x, coord.y+1);
            if (!maze.isEmpty(east))
            {
                if (maze.getOpenings(east)[0]) value += 2;
            }
            else openings[2] = true;
        }
        //Northern side check
        if (coord.x != 0)
        {
            Vector2Int north = new Vector2Int(coord.x-1, coord.y);
            if (!maze.isEmpty(north))
            {
                if (maze.getOpenings(north)[1]) value += 1;
            }
            else openings[3] = true;
        }

        //Implement Corner
        byte results = maze.isCorner(coord);
        if (Random.value < 5 / (float)(maze.mWidth + maze.mHeight) && cornerTileNum > 0 && results > 0){
            value += results;
            cornerTileNum -= 1;
        }
        maze.addTile(coord, value);
        return openings;
    }

    private static void updateRouteArray(Maze maze, Vector2Int coord, List<Vector2Int> routeArray)
    {
        List<bool> openings = maze.getOpenings(coord);

        if (openings[0] && coord.y != 0)
        {
            Vector2Int west = new Vector2Int(coord.x, coord.y - 1);
            if (maze.isEmpty(west)) { routeArray.Add(west); }
        }
        if (openings[1] && coord.x != maze.mHeight-1)
        {
            Vector2Int south = new Vector2Int(coord.x+1, coord.y);
            if (maze.isEmpty(south)) { routeArray.Add(south); }
        }
        if (openings[2] && coord.y != maze.mWidth-1)
        {
            Vector2Int east = new Vector2Int(coord.x, coord.y + 1);
            if (maze.isEmpty(east)) { routeArray.Add(east); }
        }
        if (openings[3] && coord.x != 0)
        {
            Vector2Int north = new Vector2Int(coord.x-1, coord.y);
            if (maze.isEmpty(north)) { routeArray.Add(north); }
        }
    }
    private static Maze makeMaze(int height, int width)
    {
        Maze maze = new Maze(height, width);
        List<Vector2Int> routeArray = new List<Vector2Int>();

        //Starting Tile
        Vector2Int starting = new Vector2Int(height / 2, width / 2);
        maze.addTile(starting, 15);
        updateRouteArray(maze, starting, routeArray);

        while (routeArray.Count > 0)
        {
            Vector2Int coord = routeArray[0];
            routeArray.RemoveAt(0);
            if (maze.isEmpty(coord)) //Earlier operations might have acted on this coord
            {
                List<bool> openings = findAdjacentCells(maze, coord);

                List<byte> directions = new List<byte>();
                for (int i = 0; i < 4; i++)
                {
                    if (openings[i]) directions.Add((byte)Mathf.Pow(2, 3 - i));
                }
                byte value = maze.getValue(coord);

                if (Random.value < thirdProb)
                {
                    while (directions.Count > 0) { 
                        value += directions[0];
                        directions.RemoveAt(0);
                    }
                }
                else if (Random.value < secondProb)
                {
                    while (directions.Count > 1)
                    {
                        value += directions[0];
                        directions.RemoveAt(0);
                    }
                }
                else
                {
                    while (directions.Count > 2)
                    {
                        value += directions[0];
                        directions.RemoveAt(0);
                    }
                }
                maze.addTile(coord, value);
                updateRouteArray(maze, coord, routeArray);
            }
        }
        return maze;
    }
    public static Maze generateMaze(int height, int width, double secondP, double thirdP)
    {
        secondProb = secondP;
        thirdProb = thirdP;
        Maze maze = makeMaze(height, width);
    
        while (maze.getEmptiness()/((float)(height*width)) > spaceTolerance){
            maze = makeMaze(height, width);
        }
        return maze;
    }
}
