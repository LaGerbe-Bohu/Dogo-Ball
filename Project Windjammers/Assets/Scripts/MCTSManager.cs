using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MCTSManager
{
    private GameManager GameManager;
    
    public bool SimulateGame(GameState gameState)
    {
        float c = 0;
        bool finish = false;
        GameManager = InterfaceGameState.instance.getGameManager();
        
        GameManager.initializeGame(gameState,true);
        gameState.simulation = true;
        while ( !finish)
        {
            c++;

            if (c >= 10000)
            {
                Debug.LogError("Simulation à pris plus de 10k tics");
                break;
                
            }
            
            GameManager.RunFrame(gameState);
            GameManager.RunSimulatedMovement(gameState);
            finish = InterfaceGameState.instance.getGameManager().Goal(gameState);
            
            Debug.Log("J1 ==> " + gameState.j1.getPosition() + " |" + "J2 ==> " + gameState.j2.getPosition() + " |"+ "Frisbee ==> " + gameState.frisbee.getPosition() + " |");
          
        }
        
        return finish;
    }
}
