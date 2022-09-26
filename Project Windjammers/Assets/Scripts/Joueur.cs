using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Joueur : MonoBehaviour
{
    public float moveSpeed;
    
    private Vector2 position;
    private Vector2 direction;
    private Score score;

    public  void setDirection(Vector2 dir)
    {
        this.direction = dir;   
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
        score = this.GetComponent<Score>();
    }
    
    
    
}
