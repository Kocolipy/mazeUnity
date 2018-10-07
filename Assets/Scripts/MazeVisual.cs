using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeVisual : MonoBehaviour
{
    public static MazeVisual instance;
    public GameObject maze0, maze1, maze2, maze3, maze4, maze5,maze6, maze7, maze8, maze9,
                      maze10, maze11, maze12, maze13, maze14, maze15;
    public GameObject startloc, endloc;
    public GameObject Blood;
    public GameObject CommonMonster;
    public GameObject AlertMonster;
    public GameObject StalkerMonster;

    private Maze maze;
    private GameObject[] mazeArray;
    private List<GameObject> bloodstains = new List<GameObject>();
    private Dictionary<GameObject, List<GameObject>>  monsterDictionary = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<Vector2>> monsterLocations = new Dictionary<GameObject, List<Vector2>>();
    
    public int getWidth() { return maze.mWidth; } 
    public int getHeight() { return maze.mHeight; } 
    public Maze getMaze() { return new Maze(maze); } //return a deep copy of the Maze object

    protected AudioSource source;

    public Vector3 convertMazeSqToPos(Vector2Int MazeSq)
    {
        return new Vector3((float)((MazeSq.y - getWidth() / 2) * 1.5), (float)((getHeight() / 2 - MazeSq.x) * 1.5));
    }
    public Vector2Int convertPosToMazeSq(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt((float)(getHeight() / 2 - pos.y / 1.5)),
                              Mathf.RoundToInt((float)(pos.x / 1.5 + getWidth() / 2)));
    }
    
    void Awake()
    {
        if (instance == null) instance = this; 
        else if (instance != this) Destroy(this);  

        mazeArray = new GameObject[] {maze0, maze1, maze2, maze3, maze4, maze5,maze6, maze7, maze8, maze9,
                      maze10, maze11, maze12, maze13, maze14, maze15};
        monsterDictionary.Add(CommonMonster, new List<GameObject>());
        monsterLocations.Add(CommonMonster, new List<Vector2>());
        monsterDictionary.Add(AlertMonster, new List<GameObject>());
        monsterLocations.Add(AlertMonster, new List<Vector2>());
        monsterDictionary.Add(StalkerMonster, new List<GameObject>());
        monsterLocations.Add(StalkerMonster, new List<Vector2>());

        source = GetComponent<AudioSource>();
    }
    public void generateMaze(int height, int width, double secondP , double thirdP, Vector3Int monsters)
    {
        maze = MazeGenerator.generateMaze(height, width, secondP, thirdP);

        /*
         *  MAZE
        */
        for (int i =0; i < maze.mHeight; i++)
        {
            for (int j=0; j<maze.mWidth; j++)
            {
                Vector2Int tmp = new Vector2Int(i, j);
                Instantiate(mazeArray[maze.getValue(tmp)], convertMazeSqToPos(tmp),
                            mazeArray[maze.getValue(tmp)].transform.rotation).transform.parent = transform;
            }
        }
        /*
         *  Start and End
        */
        Vector2Int startGrid = new Vector2Int(maze.mHeight / 2, maze.mWidth / 2);
        Instantiate(startloc, new Vector2(0, 0), Quaternion.identity).transform.parent = transform;
        Vector2Int end = BFS.getFurthest(maze, startGrid, (int) Random.Range((maze.mHeight* maze.mWidth)/10, (maze.mHeight*maze.mWidth)/4));
        Instantiate(endloc, convertMazeSqToPos(end), 
                    Quaternion.identity).transform.parent = transform;


        /*
         *  MONSTERS!
        */
        //Get all spaces where we can insert Objects (remove those too close to the start)
        List<Vector2Int> occupied = maze.getOccupied();
        removeCloseTo(occupied, startGrid);

        Vector2 temp;
        //Load the Stalker first
        for (int i = 0; i < monsters.z; i++)
        {
            //Break if no space for monsters
            if (occupied.Count <= 0)  break; 
            //Get a random location for the monster
            int rand = Random.Range(0, occupied.Count);
            temp = convertMazeSqToPos(occupied[rand]);
            //Add Monster to monsterDictionary and location to monsterLocations
            monsterLocations[StalkerMonster].Add(temp);
            GameObject monster = Instantiate(StalkerMonster, temp, Quaternion.identity);
            monster.transform.parent = transform;
            monsterDictionary[StalkerMonster].Add(monster);
            removeCloseTo(occupied, occupied[rand]);
        }
        //Load the AlertMonster
        for (int i = 0; i < monsters.y; i++)
        {
            //Break if no space for monsters
            if (occupied.Count <= 0)  break; 
            //Get a random location for the monster
            int rand = Random.Range(0, occupied.Count);
            temp = convertMazeSqToPos(occupied[rand]);
            //Add Monster to monsterDictionary and location to monsterLocations
            monsterLocations[AlertMonster].Add(temp);
            GameObject monster = Instantiate(AlertMonster, temp, Quaternion.identity);
            monster.transform.parent = transform;
            monsterDictionary[AlertMonster].Add(monster);
            removeCloseTo(occupied, occupied[rand]);
        }
        //Load the CommonMonster
        for (int i = 0; i < monsters.x; i++)
        {
            //Break if no space for monsters
            if (occupied.Count <= 0)  break; 
            //Get a random location for the monster
            int rand = Random.Range(0, occupied.Count);
            temp = convertMazeSqToPos(occupied[rand]);
            //Add Monster to monsterDictionary and location to monsterLocations
            monsterLocations[CommonMonster].Add(temp);
            GameObject monster = Instantiate(CommonMonster, temp, Quaternion.identity);
            monster.transform.parent = transform;
            monsterDictionary[CommonMonster].Add(monster);
            removeCloseTo(occupied, occupied[rand]);
        }
    }
    private void removeCloseTo(List<Vector2Int> lst, Vector2Int loc)
    {
        int count = 0;
        while (count < lst.Count)
        {
            if ((lst[count] - loc).sqrMagnitude <= 4) lst.RemoveAt(count);
            else count++; 
        }
    }
    public void addBloodStain(Vector3 location)
    {
        GameObject blood = Instantiate(Blood, location, Quaternion.identity);
        blood.transform.parent = transform;
        bloodstains.Add(blood);
    }
    public void resetMonsters()
    {
        foreach (GameObject key in monsterDictionary.Keys)
        {
            foreach (GameObject monster in monsterDictionary[key])
            {
                if (monster != null) Object.Destroy(monster); 
            }
            foreach (Vector2 v in monsterLocations[key])
            {
                GameObject monster = Instantiate(key, v, Quaternion.identity);
                monster.transform.parent = transform;
                monsterDictionary[key].Add(monster);
            }
        }
        foreach (GameObject blood in bloodstains)
        {
            if (blood != null) Object.Destroy(blood);
        }
        bloodstains.Clear();
    }
    public void clearMaze()
    {
        foreach (Transform child in transform){ Object.Destroy(child.gameObject); }
        foreach (GameObject key in monsterDictionary.Keys)
        {
            monsterDictionary[key].Clear();
            monsterLocations[key].Clear();
        }

    }
    public void playClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
