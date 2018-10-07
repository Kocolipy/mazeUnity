using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {
    public static GameControl instance;

    //Maze Generation Properties
    private int currHeight = 7;
    private int currWidth = 7;
    private double secondProb = 0.6;
    private double thirdProb = 0.275; 
    private Vector3Int monsters = new Vector3Int(1,0,0);

    //Level
    public Text floorText;
    private int floor = 0;

    //Health of Player
    private int health = 3;
    public GameObject Heart0;
    public GameObject Heart1;
    public GameObject Heart2;
    private GameObject[] hearts;

    void Awake()
    {
        if (instance == null)  instance = this; 
        else if (instance != this)  Destroy(this); 
        hearts = new GameObject[] { Heart0, Heart1, Heart2 };
    }
	// Initialization
	void Start () {
        IncrementFloorText();
        MazeVisual.instance.generateMaze(currHeight, currWidth, secondProb, thirdProb, monsters);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void LoseHealth()
    {
        if (--health <= 0) gameOver(); 
        else  restartLevel(); 
        hearts[health].SetActive(false);
    }
    public void gameOver()
    {
        //Game Over Screen
        PlayerController.instance.gameObject.SetActive(false);
        MazeVisual.instance.clearMaze();
    }
    public void restartLevel()
    {
        //Reset Monsters positions
        MazeVisual.instance.resetMonsters();
    }
    public void LevelClear()
    {
        MazeVisual.instance.clearMaze();
        IncrementFloorText();
        PlayerController.instance.ResetPosition();

        upgradeMazeSize();
        upgradeMonsters();
        upgradeCamera();

        MazeVisual.instance.generateMaze(currHeight, currWidth, secondProb, thirdProb, monsters);
    }
    public void IncrementFloorText()
    {
        floorText.text = "B" + (++floor).ToString();
    }
    private void upgradeMazeSize()
    {
        currHeight += 2;
        currWidth += 2;
    }
    private void upgradeMonsters()
    {
        monsters = monsters + new Vector3Int(3, 0, 0);
        if (floor%2 == 0) monsters = monsters + new Vector3Int(0, 1, 0);
        if (floor%3== 0) monsters = monsters + new Vector3Int(0, 0, 1);
        if (floor%5 == 1) monsters = monsters + new Vector3Int(1, 1, 1);
    }
    private void upgradeCamera()
    {
        if (Camera.main.orthographicSize > 1.75)
        {
            if (floor%2 == 0) Camera.main.orthographicSize *= 0.95f;
        }
    }
}
