using Unity.VisualScripting;
using UnityEngine;


public class GameState 
{
    
    
    public float timer;
    private int setCurrent;
    
    public Joueur j1;
    public Joueur j2;

    public Frisbee frisbee;
    
    public bool InputPress = false;

    public bool moveAgent; // Dit sit mon agent peut bouger
    public bool gameStarted ; // dit si la partie à commencé
    public bool movePlayer = false; // dit sir mon player peut bouger
    public bool simulation = false; // dit si ce gamestage est une simulation 
    public bool endResetGame = true; // dit si les joueurs ont fini de se mettre en place
    
    public Joueur oldThrower; // ancien tireur du frisbee pour éviter que le joueur puisse taper plusieurs fois dans la balle
    public Vector2 initialposf; // position de base de frisbee
  
    
    public GameState(Joueur j1, Joueur j2,Frisbee frisbee, int timer,int setCurrent)
    {
        this.frisbee = new Frisbee(frisbee);
        this.j1 = new Joueur(j1);
        this.j2 = new Joueur(j2);
        this.timer = timer;
        this.setCurrent = setCurrent;
            
            
        this.j1.getScore().setCount = InterfaceGameState.J1set;
        this.j2.getScore().setCount = InterfaceGameState.J2set;
        
        
        this.moveAgent = false;
        this.gameStarted = false;
        this.oldThrower = null;
        this.endResetGame = true;
        this.movePlayer = false;
        this.simulation = false;
    }
    
    // constructeur par copie
    public GameState(GameState gameState)
    {
      
        this.frisbee =  new Frisbee( gameState.frisbee);
        this.j1 =   new Joueur(gameState.j1);
        this.j2 = new Joueur( gameState.j2);
        this.timer = gameState.timer;
        this.setCurrent = gameState.setCurrent;
            
            
        this.j1.getScore().setCount = InterfaceGameState.J1set;
        this.j2.getScore().setCount = InterfaceGameState.J2set;

        this.moveAgent = gameState.moveAgent;
        this.gameStarted = gameState.gameStarted;
        this.oldThrower = gameState.oldThrower;
        this.endResetGame = gameState.endResetGame;
        this.movePlayer = gameState.movePlayer;
        this.simulation = gameState.simulation;

    }



}

