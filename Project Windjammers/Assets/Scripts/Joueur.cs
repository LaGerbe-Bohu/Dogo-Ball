using System;
using UnityEngine;


[System.Serializable]
public class Joueur 
{
    private Vector2 position;
    private Vector2 direction;
    public Score score;
    public float moveSpeed;
    public float counter;

    public Joueur(Vector2 position, Vector2 direction,float moveSpeed,Score score)
    {
        this.position = position;
        this.direction = direction;
        this.score = score;
        this.moveSpeed = moveSpeed;
    }

    public Joueur(Joueur joueur)
    {
        this.position = joueur.position;
        this.direction = joueur.direction;
        this.score = joueur.score;
        this.moveSpeed = joueur.moveSpeed;
    }
    
    
    public Score getScore()
    {
        return this.score;
    }
    
    public  void setDirection(Vector2 dir)
    {
        this.direction = dir;   
    }
    
    public Vector2 getPosition()
    {
        return this.position;
    }
    
    public void setPosition( Vector2 vec)
    {
        this.position = vec;
    }
    
    public Vector2 getDirection()
    {
        return this.direction;
    }
    
    
    private void Start()
    {

    }
    
    
    
}
