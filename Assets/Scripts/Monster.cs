using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    public AudioClip deathclip;

    protected Rigidbody2D rb2d;
    protected Animator animator;
    protected Vector3 movement;

    protected Maze maze;
    protected Vector2Int startingPoint;
    protected Vector2Int prevStart;
    protected Vector2Int currentPosition;
    protected List<Vector2Int> path;
    protected Vector2Int next;

    protected int health = 1;
    protected float speed = 20;
    protected AudioSource source;

    // Use this for initialization
    protected virtual void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        maze = MazeVisual.instance.getMaze();
        startingPoint = MazeVisual.instance.convertPosToMazeSq(transform.position);

        //Movement Logic
        prevStart = startingPoint;
        path = BFS.getRandomPath(maze, startingPoint, 5);
        next = path[0];
        path.RemoveAt(0);
        movement = MazeVisual.instance.convertMazeSqToPos(next) - transform.position;

        source = GetComponent<AudioSource>();
    }
    protected virtual void getNextPath()
    {
        /* Set next to the next item in path
         * If path is empty, do BFS.getRandomPath to get a new path
         * */
        if (path.Count <= 0)
        { //If path is empty, get a new path
            int tries = 0;
            do
            { //Try to get a different end position (go with same end if fails 3 times)
                path = BFS.getRandomPath(maze, currentPosition, 5);
            } while (path[path.Count - 1] == prevStart && ++tries <= 3);
            prevStart = next;
        }
        next = path[0];
        path.RemoveAt(0);
    }
    protected virtual Vector2 Move()
    {
        currentPosition = MazeVisual.instance.convertPosToMazeSq(transform.position);
        if (reachGridPosition(next)) getNextPath();
        movement = MazeVisual.instance.convertMazeSqToPos(next) - transform.position;
        return movement.normalized;
    }
    // Update is called once per frame
    protected virtual void FixedUpdate() {
        movement = Move();

        //Animations
        Vector3 direction = convertToDirection(movement);
        if (direction.x == 1)  animator.SetTrigger("MonsterRight"); 
        else if (direction.x == -1)  animator.SetTrigger("MonsterLeft"); 
        else if (direction.y == 1) animator.SetTrigger("MonsterUp"); 
        else if (direction.y == -1) animator.SetTrigger("MonsterDown"); 
   
        rb2d.AddForce(movement * speed);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Monster")
        {
            //Monster Fight
            if (--health <= 0) {
                MazeVisual.instance.playClip(deathclip);
                Object.Destroy(gameObject);
                MazeVisual.instance.addBloodStain(transform.position);
            }
        }
    }
    protected Vector3 convertToDirection(Vector3 movement)
    {
        //Conversion of movement into directions
        if (movement.x < 0.5 && movement.x > -0.5) movement.x = 0; 
        else if (movement.x > 0.5)  movement.x = 1; 
        else { movement.x = -1; }

        if (movement.x != 0)  movement.y = 0; 
        else if (movement.y < 0.5 && movement.y > -0.5) movement.y = 0; 
        else if (movement.y > 0.5)  movement.y = 1; 
        else movement.y = -1; 
        return movement;
    }
    protected bool reachGridPosition(Vector2Int next)
    {
        return Vector2.Distance(MazeVisual.instance.convertMazeSqToPos(next), transform.position)< 0.1;
    }
}
