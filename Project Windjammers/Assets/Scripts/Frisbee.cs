using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Frisbee: MonoBehaviour
{
    public float Speed = 7f;
    
    private Vector2 position;
    private Vector2 direction;
    private Joueur joueur;
    
    

    public Transform frisbeeTransform;
    private GameState gameState;


    public Joueur getJoueur()
    {
        return this.joueur;
    }
    
    public void setJoueur(Joueur j)
    {
        this.joueur = j;
    }
    
    
    
    
    private bool isCatched;

    public bool getIsCatched()
    {
        return isCatched;
    }

    public void setCatched(bool s)
    {
        this.isCatched = s;
    }

    private void Start()
    {
        gameState = GameState.Instance;
        
        
        
        float random = Random.Range(0, 10);
        
     
        
        if (random > 5)
        {
            direction = (gameState.j1.transform.position - this.transform.position).normalized;
        }
        else direction = (gameState.j2.transform.position - this.transform.position).normalized;
        
        direction = (gameState.j1.transform.position - this.transform.position).normalized;
    }
    

    private void Update()
    {

      
    }

  

    public Vector2 getPosition()
    {
        return this.transform.position;
    }

    public Vector2 GetDirection()
    {
        return this.direction;
    }
    
    public void setPosition(Vector2 pos)
    {
        this.transform.position = pos;
    }
    public void setDirection(Vector2 dir)
    {
        this.direction = dir;
    }

    

}
