using System;
using UnityEngine;

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
    public Vector2 getPosition()
    {
        return this.position;
    }

    private Rigidbody2D rigidbody2D;
    
    
    private void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        position = this.transform.position;
    }
}
