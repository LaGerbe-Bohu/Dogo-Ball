using System;
using UnityEngine;


public class Joueur : MonoBehaviour
{
    private Vector2 position;
    private Vector2 direction;
    private Score score;

    public float moveSpeed;

    public  void setDirection(Vector2 dir)
    {
        this.direction = dir;   
    }
    
    public Vector2 getPosition()
    {
        return this.transform.position;
    }
    
    public Vector2 getDirection()
    {
        return this.direction;
    }
    
    private Rigidbody2D rigidbody2D;

    public Rigidbody2D getRigibody2D()
    {
        return rigidbody2D;
    }
    
    private void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }
    
    
    
}
