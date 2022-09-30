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
    public Vector2 center;
    public Vector2 sizeBound;
    
    public Joueur(Vector2 position, Vector2 direction,float moveSpeed,string PlayerTag,Vector2 center, Vector2 sizeBound,Score score)
    {
        this.position = position;
        this.direction = direction;
        this.score = new Score(score);
        this.moveSpeed = moveSpeed;
        this.PlayerTag = PlayerTag;
        this.counter = 0;
        this.center = center;
        this.sizeBound = sizeBound;
    }

    public Joueur(Joueur joueur)
    {
        this.position = joueur.position;
        this.direction = joueur.direction;
        this.score = new Score(joueur.score);
        this.moveSpeed = joueur.moveSpeed;
        this.PlayerTag = joueur.PlayerTag;
        this.counter = 0;
        this.center = joueur.center;
        this.sizeBound = joueur.sizeBound;
    }
    
    
    public Score getScore()
    {
        return this.score;
    }
    
    public  void setDirection(Vector2 dir)
    {
        this.direction = dir.normalized;   
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
