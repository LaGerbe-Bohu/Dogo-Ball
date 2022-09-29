using Unity.VisualScripting;
using UnityEngine;


public class GameState 
{
    
    
    public float timer;
    private int setCurrent;
    
    public Joueur j1;
    public Joueur j2;

    public Frisbee frisbee;
    
    public Coroutine co;
    public bool InputPress = false;

    public bool moveRandom;
    public bool gameStarted ;
    public bool movePlayer = false;
    public bool freezeInput = false;
    public bool simulation = false;
    
    public Joueur oldThrower;
    public Vector2 initialposf;
    public bool endResetGame = true;
    public float catchedTimer;
    
    
    public GameState(Joueur j1, Joueur j2,Frisbee frisbee, int timer,int setCurrent)
    {
        this.frisbee = new Frisbee(frisbee);
        this.j1 = new Joueur(j1);
        this.j2 = new Joueur(j2);
        this.timer = timer;
        this.setCurrent = setCurrent;
            
            
        this.j1.getScore().setCount = InterfaceGameState.J1set;
        this.j2.getScore().setCount = InterfaceGameState.J2set;
        
        
        this.moveRandom = false;
        this.gameStarted = false;
        this.oldThrower = null;
        this.endResetGame = true;
        this.movePlayer = false;
        this.freezeInput = false;
        this.simulation = false;
    }
    
    public GameState(GameState gameState)
    {
      
        this.frisbee =  new Frisbee( gameState.frisbee);
        this.j1 =   new Joueur(gameState.j1);
        this.j2 = new Joueur( gameState.j2);
        this.timer = gameState.timer;
        this.setCurrent = gameState.setCurrent;
            
            
        this.j1.getScore().setCount = InterfaceGameState.J1set;
        this.j2.getScore().setCount = InterfaceGameState.J2set;

        this.moveRandom = gameState.moveRandom;
        this.gameStarted = gameState.gameStarted;
        this.oldThrower = gameState.oldThrower;
        this.endResetGame = gameState.endResetGame;
        this.movePlayer = gameState.movePlayer;
        this.freezeInput = gameState.freezeInput;
        this.simulation = gameState.simulation;
    }



}

