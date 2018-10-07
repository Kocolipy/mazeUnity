using UnityEngine;
using System.Collections;

public class StalkerMonster : Monster
{
    public AudioClip chaseclip;

    private int chaseDistance = 5;
    private float baseSpeed = 10;
    private float chaseSpeed = 25;

    protected bool chase = false;

    protected override void Start()
    {
        base.Start();
        health = 3;
    }

    // Update is called once per frame
    protected override void getNextPath()
    {
        //If close to the player, it will constantly refresh its search path to find the target.
        if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < chaseDistance*2) //Actual space
        {
            path = BFS.getShortest(maze, MazeVisual.instance.convertPosToMazeSq(transform.position),
                        MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position), chaseDistance); //grid distance
            //if found
            if (path.Count > 0)
            {
                if (speed != chaseSpeed) source.PlayOneShot(chaseclip);
                speed = chaseSpeed;
                next = path[0];
                path.Clear();
                return;
            }
        }
        speed = baseSpeed;
        if (path.Count <= 0)
        {
            //Look for player (worldwide search)
            path = BFS.getShortest(maze, MazeVisual.instance.convertPosToMazeSq(transform.position),
                MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position), 1000); //grid distance
            //if found
            if (path.Count > 0)
            {
                next = path[0];
                path.RemoveAt(0);
                return;
            }
            //Did not find player (too far), so revert to wandering
            base.getNextPath();
        }
        next = path[0];
        path.RemoveAt(0);
    }
    protected override Vector2 Move()
    {
        currentPosition = MazeVisual.instance.convertPosToMazeSq(transform.position);
        if (chase)
        {
            if (currentPosition != MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position))
            {
                //Player ran away, resume stalking mode
                chase = false; 
                path.Clear();
                getNextPath();
            } else {
                movement = PlayerController.instance.transform.position - transform.position;
                return movement.normalized;
            }
        }
        else if (reachGridPosition(next))
        {
            if (currentPosition == MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position))
            {
                chase = true;
                movement = PlayerController.instance.transform.position - transform.position;
                return movement.normalized;
            }
            else getNextPath(); 
        }
        movement = MazeVisual.instance.convertMazeSqToPos(next) - transform.position; 
        return movement.normalized;
    }
}
