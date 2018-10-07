using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public AudioClip deathclip;

    private Rigidbody2D rb2d;
    private float speed= 1.5f;
    private Vector2 startingPosition = Vector2.zero;
    private bool playerDead;

    private Animator animator;

    void Awake()
    {
        if (instance == null)  instance = this; 
        else if (instance != this)  Destroy(this); 
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerDead = false;
    }

    void FixedUpdate()
    {
        
        if (playerDead) return;  

        int moveHorizontal = (int) Input.GetAxisRaw("Horizontal");
        int moveVertical = (int) Input.GetAxisRaw("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            animator.enabled = false;
        }else
        {
            animator.enabled = true;
            if (moveHorizontal != 0)  moveVertical = 0;  //One Direction at once
            if (moveHorizontal == 1)  animator.SetTrigger("playerRight"); 
            else if (moveHorizontal == -1)  animator.SetTrigger("playerLeft"); 
            else if (moveVertical == 1)  animator.SetTrigger("playerUp"); 
            else if (moveVertical == -1)  animator.SetTrigger("playerDown"); 

            Vector3 movement = new Vector3(moveHorizontal, moveVertical);
            rb2d.AddForce(movement * speed);
        }
    }
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "End")
        {
            GameControl.instance.LevelClear();
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Monster")
        {
            if (!playerDead) MazeVisual.instance.playClip(deathclip);
            playerDead = true;
            animator.enabled = true;
            animator.SetTrigger("playerHit");

            //Reset() is called when animation ends 
        }
    }
    public Vector2Int getPosition()
    {
        return MazeVisual.instance.convertPosToMazeSq(transform.position);
    }
    private void LoseHealth()
    {
        GameControl.instance.LoseHealth();
        ResetPosition();
    }
    public void ResetPosition()
    {
        playerDead = false;
        transform.position = startingPosition;
    }
}