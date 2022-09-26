using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{
    
    public static int timer;
    public static int setCurrent;
    
    public static Joueur j1;
    public static Joueur j2;
    
    public static Frisbee frisbee;
    
    public static GameState gameState;
    private static GameManager gameManager;
    
    
    private void Awake()
    {
        gameState = this;
        
        
    }
}

