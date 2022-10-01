using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class Frisbee
{
    public float Speed;
    
    private Vector2 position;
    private Vector2 direction;
    private Joueur joueur;
    private bool isCatched;
    private Vector2 sDirection;
    
    public Frisbee(Vector2 position,Vector2 direction,float speed,Joueur joueur)
    {
        this.position = position;
        this.direction = direction;
        this.joueur = joueur;
        this.Speed = speed;
        isCatched = false;
        sDirection = direction * this.Speed;
    }

    public Frisbee(Frisbee frisbee)
    {
        
        this.position = frisbee.position;
        this.direction = frisbee.direction;
        this.joueur = frisbee.joueur;
        this.Speed = frisbee.Speed;
        this.isCatched = frisbee.isCatched;
        sDirection =  direction * this.Speed;
    }   
    
    public Joueur getJoueur()
    {
        return this.joueur;
    }
    
    public void setJoueur(Joueur j)
    {
        this.joueur = j;
    }


    public bool getIsCatched()
    {
        return isCatched;
    }

    public void setCatched(bool s)
    {
        this.isCatched = s;
    }


    public Vector2 getSpeededDirection()
    {
        return this.sDirection;
    }

    public Vector2 getPosition()
    {
        return this.position;
    }

    public Vector2 GetDirection()
    {
        return this.direction;
    }
    
    public void setPosition(Vector2 pos)
    {

        this.position = pos;
    }
    public void setDirection(Vector2 dir)
    {
        this.direction = dir.normalized;
        this.sDirection = this.direction * this.Speed;
    }

    

}
