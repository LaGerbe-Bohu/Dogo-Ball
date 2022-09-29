using System;
using UnityEngine;


[System.Serializable]
public class Joueur 
{
    private Vector2 position;
    private Vector2 direction;
    [HideInInspector]
    public Score score;
    public float moveSpeed;
    [HideInInspector]
    public float counter;
    public string PlayerTag;
    
    public Joueur(Vector2 position, Vector2 direction,float moveSpeed,string PlayerTag,Score score)
    {
        this.position = position;
        this.direction = direction;
        this.score = new Score(score);
        this.moveSpeed = moveSpeed;
        this.PlayerTag = PlayerTag;
        this.counter = 0;
    }

    public Joueur(Joueur joueur)
    {
        this.position = joueur.position;
        this.direction = joueur.direction;
        this.score = new Score(joueur.score);
        this.moveSpeed = joueur.moveSpeed;
        this.PlayerTag = joueur.PlayerTag;
        this.counter = 0;
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
