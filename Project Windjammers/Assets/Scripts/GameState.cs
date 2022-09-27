using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{


    public static GameState Instance;
    
    private int timer;
    private int setCurrent;
    
    public Joueur j1;
    public Joueur j2;
    
   public Frisbee frisbee;

    public GameManager gameManager;

    public GameManager getGameManager()
    {
        return this.gameManager;
    }
    
    private void Awake()
    {
        Instance = this;
        
    }

    private void FixedUpdate()
    {
  
    }


}

