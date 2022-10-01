using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceGameState : MonoBehaviour
{
    public static InterfaceGameState instance;
    public GameState gameState;
    private int timer;
    private int setCurrent;

    public static bool IA;
    public Joueur j1;
    public Joueur j2;
    
    public static int J1set;
    public static int J2set;
    
    public Frisbee frisbee;

    public GameManager gameManager;

    public GameManager getGameManager()
    {
        return this.gameManager;
    }
    
    private void Awake()
    {
        instance = this;

        j1 = new Joueur(gameManager.j1.position, Vector2.zero, j1.moveSpeed, j1.PlayerTag,this.gameManager.CenterPoint[0].position,new Vector2(gameManager.WidthPlayer/2f,gameManager.HightPlayer),new Score(0,0));
        j2 = new Joueur(gameManager.j2.position, Vector2.zero, j2.moveSpeed,j2.PlayerTag,this.gameManager.CenterPoint[1].position,new Vector2(gameManager.WidthPlayer/2f,gameManager.HightPlayer),new Score(0,0));
        frisbee = new Frisbee((Vector2) gameManager.frisbee.transform.position, Vector2.zero,frisbee.Speed, j1);
            
        gameState = CreateInstance();

    }

    public GameState CreateInstance()
    {
        GameState g = new GameState(j1,j2,frisbee,timer,setCurrent);
        
        return g;
    }
    
    public GameState CreateInstance(GameState gameState)
    {
        GameState g = new GameState(gameState);

        return g;
    }
    
}
