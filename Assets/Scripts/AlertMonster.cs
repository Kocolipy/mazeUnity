using UnityEngine;

public class AlertMonster : Monster
{
    public AudioClip chaseclip;

    private int alertDistance = 3;
    private float baseSpeed = 20;
    private float chaseSpeed = 27.5f;
    
    private bool chase = false;

    protected override void Start()
    {
        base.Start();
        health = 2;
    }

    protected override void getNextPath()
    {
        //Look for player within alertdistance (will lose track of player soon)
        if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < alertDistance) //Actual space
        {
            path = BFS.getShortest(maze, MazeVisual.instance.convertPosToMazeSq(transform.position),
                        MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position), alertDistance); //grid distance
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
        //Did not find player, so revert to wandering
        base.getNextPath();
    }
    protected override Vector2 Move()
    {
        currentPosition = MazeVisual.instance.convertPosToMazeSq(transform.position);
        if (chase)
        {
            if (currentPosition != MazeVisual.instance.convertPosToMazeSq(PlayerController.instance.transform.position))
            {
                //Player ran away, resume normal mode
                chase = false;
                path.Clear();
                getNextPath();
            }
            else
            {
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
