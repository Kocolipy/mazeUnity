using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance;
    private Vector3 prevPosition;
    private Vector3 offset;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(this); }
    }
	// Use this for initialization
	void Start () {
        offset = transform.position - PlayerController.instance.transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        prevPosition = transform.position;
        transform.position = PlayerController.instance.transform.position + offset;

        double leftBoundary = MazeVisual.instance.convertMazeSqToPos(new Vector2Int(0, 0)).x - 0.75 + Camera.main.orthographicSize * Camera.main.aspect;
        double rightBoundary = MazeVisual.instance.convertMazeSqToPos(new Vector2Int(MazeVisual.instance.getHeight() -1 , MazeVisual.instance.getWidth() - 1)).x + 0.75 - Camera.main.orthographicSize * Camera.main.aspect;
        double upBoundary = MazeVisual.instance.convertMazeSqToPos(new Vector2Int(0, 0)).y + 0.75 - Camera.main.orthographicSize;
        double downBoundary = MazeVisual.instance.convertMazeSqToPos(new Vector2Int(MazeVisual.instance.getHeight() - 1, MazeVisual.instance.getWidth() - 1)).y - 0.75 + Camera.main.orthographicSize;

        if (transform.position.x < leftBoundary || transform.position.x > rightBoundary)
        {
            transform.position = new Vector3(prevPosition.x, transform.position.y, -1);
        }
        if (transform.position.y < downBoundary || transform.position.y > upBoundary)
        {
            transform.position = new Vector3(transform.position.x, prevPosition.y, -1);
        }
    }
}
