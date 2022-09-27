using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{


    public static GameState Instance;
    
    private int timer;
    private int setCurrent;
    
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
        Instance = this;

        j1.getScore().setCount = J1set;
        j2.getScore().setCount = J2set;

    }

    private void FixedUpdate()
    {
  
    }


}

