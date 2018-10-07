using UnityEngine;
using System.Collections.Generic;

public static class BFS
{
    public static Vector2Int getFurthest(Maze maze, Vector2Int start, int maxStep = 1000)
    {
        /* Get the cell furthest from start or maxStep distance from start, whichever is closer
         * */
        if (maze.getValue(start) == 0) { return start; }

        Dictionary<Vector2Int, Vector2Int?> visited = new Dictionary<Vector2Int, Vector2Int?>();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        visited.Add(start, null);
        q.Enqueue(start);
        Vector2Int? depthMarker = start;

        Vector2Int current = start;
        while (q.Count > 0)
        {
            current = q.Dequeue();
            if (current == depthMarker)
            {
                maxStep--;
                depthMarker = null;
            }
            if (maxStep < 0) { return current; }
            List<bool> openings = maze.getOpenings(current);
            List<Vector2Int> coords = new List<Vector2Int>();
            if (openings[0]) { coords.Add(new Vector2Int(current.x, current.y - 1)); }
            if (openings[1]) { coords.Add(new Vector2Int(current.x + 1, current.y)); }
            if (openings[2]) { coords.Add(new Vector2Int(current.x, current.y + 1)); }
            if (openings[3]) { coords.Add(new Vector2Int(current.x - 1, current.y)); }

            foreach (Vector2Int coord in coords)
            {
                if (!visited.ContainsKey(coord))
                {
                    visited.Add(coord, current);
                    q.Enqueue(coord);
                    if (depthMarker == null) { depthMarker = coord; }
                }
            }
        }
        return current;
    }
    public static List<Vector2Int> getShortest(Maze maze, Vector2Int start, Vector2Int end, int maxStep = 1000)
    {
        /* Get the shortest path from start to end
         * Returns a empty Stack if there isnt a path or size path exceeds maxStep 
         * */
        List<Vector2Int> shortest = new List<Vector2Int>();
        if (maze.getValue(start) == 0) { return shortest; }
        if (maze.getValue(end) == 0) { return shortest; }

        Dictionary<Vector2Int, Vector2Int?> visited = new Dictionary<Vector2Int, Vector2Int?>();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        visited.Add(start, null);
        q.Enqueue(start);
        Vector2Int? depthMarker = start;
        
        Vector2Int current;
        while (q.Count > 0)
        {
            current = q.Dequeue();
            if (current == depthMarker) {
                maxStep--;
                depthMarker = null;
            }
            if (maxStep < 0) { return shortest; }
            List<bool> openings = maze.getOpenings(current);
            List<Vector2Int> coords = new List<Vector2Int>();
            if (openings[0]) { coords.Add(new Vector2Int(current.x, current.y-1)); }
            if (openings[1]) { coords.Add(new Vector2Int(current.x+1, current.y)); }
            if (openings[2]) { coords.Add(new Vector2Int(current.x, current.y+1)); }
            if (openings[3]) { coords.Add(new Vector2Int(current.x-1, current.y)); }
            
            foreach (Vector2Int coord in coords)
            {
                if (!visited.ContainsKey(coord))
                {
                    visited.Add(coord, current);
                    q.Enqueue(coord);
                    if (depthMarker == null) { depthMarker = coord; }
                }
                if (coord == end) q.Clear();
            }
        }
        
        if (!visited.ContainsKey(end)) { return shortest; }

        current = end;
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        while (visited[current] != null)
        {
            stack.Push(current);
            current = (Vector2Int) visited[current];
        }
        foreach (Vector2Int v in stack)
        {
            shortest.Add(v);
        }
        return shortest;
    }
    public static List<Vector2Int> getRandomPath(Maze maze, Vector2Int start, int steps)
    {
        /* Get a path of size steps from start (return longest path if longest path size < step)
         * Returns a empty Stack if start is not connected
         * */
        List<Vector2Int> shortest = new List<Vector2Int>();
        if (maze.getValue(start) == 0) { return shortest; }

        Vector2Int current = start;

        for (int i = 0; i < steps; i++)
        {
            List<bool> openings = maze.getOpenings(current);
            List<Vector2Int> coords = new List<Vector2Int>();
            if (openings[0]) { coords.Add(new Vector2Int(current.x, current.y - 1)); }
            if (openings[1]) { coords.Add(new Vector2Int(current.x + 1, current.y)); }
            if (openings[2]) { coords.Add(new Vector2Int(current.x, current.y + 1)); }
            if (openings[3]) { coords.Add(new Vector2Int(current.x - 1, current.y)); }

            //Remove all coord that is in the path
            int count = 0;
            while (count < coords.Count)
            {
                if (shortest.Contains(coords[count])){ coords.RemoveAt(count);}
                else { count++;  }
            }

            if (coords.Count > 0)
            {
                current = coords[Random.Range(0, coords.Count)];
                shortest.Add(current);
            } else { return shortest; }
        }
        return shortest;
    }
}
