using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


public class MCTSnode
{
    public MCTSnode parent;
    public List<MCTSnode> childrens;
    public GameState GameState;
    public Action action;
    public int numberWins;
    public int numberSimulations;
    public bool FullyExpanded;
}


public class MCTSManager
{
    private const int NUMBER_TEST = 100;
    private const int NUMBER_SIMULATION = 20;
    
    
    public MCTSManager()
    {
        
    }
    
    private GameManager GameManager;
    private List<MCTSnode> lstnodes;
    private Joueur joueur;
    
    public Action ComputeMCTS(Joueur joueur,GameState gameState)
    {
        lstnodes = new List<MCTSnode>();
        GameManager = InterfaceGameState.instance.gameManager;
        this.joueur = joueur;
        
        MCTSnode startNode = new MCTSnode()
        {
            parent = null,
            childrens = new List<MCTSnode>(),
            GameState = gameState,
            numberWins = 0,
            numberSimulations = 0,
            FullyExpanded = false
        };
        
        lstnodes.Add(startNode);
        
        for (int i = 0; i < NUMBER_TEST; i++)
        {
            MCTSnode selectedNode = Selection();
            MCTSnode newNode = Expland(selectedNode);
            int victoire = SimulateGame(newNode, newNode.action);
            Backpropagation(newNode.parent,victoire);
            
        }

        float ratio = 0;
        MCTSnode n = null;
        foreach (var node in startNode.childrens)
        {
            if ( (node.numberWins / (float) node.numberSimulations) >= ratio)
            {
                ratio = node.numberWins / (float) node.numberSimulations;
                n = node;
            }
        }
        
        Assert.IsNotNull(n);

        return n.action;
    }


    public void Backpropagation(MCTSnode node,int victory)
    {
        MCTSnode n = node;
        while (n != null)
        {
            n.numberSimulations += NUMBER_SIMULATION;
            n.numberWins += victory;
            n = n.parent;
        }
       
    }
    
    bool findSameInput(MCTSnode root,Action action)
    {
        bool trouver = false;
        int c = 0;
        if (root != null)
        {
            
            int i = 0;
            while (!trouver && i < root.childrens.Count)
            {
                c++;
                if (c >= 1000)
                {
                    Debug.LogError("Boucle infini");
                    break;
                }
                
                if ( (root.childrens[i].action.direction == action.direction) && (Math.Abs(root.childrens[i].action.time - action.time) < float.Epsilon) )
                {
                    trouver = true;
                    
                }
                else
                {
                    i++;
                }
            }
        }
        
        return trouver;
    }
    
    
    public MCTSnode Expland(MCTSnode node)
    {
        
        Assert.IsNotNull(node);
        Action ActionToplay;
        
        do
        {
            ActionToplay = GameManager.actions[Random.Range(0,GameManager.actions.Count)];
        } while (findSameInput(node.parent,ActionToplay));
        
        
        MCTSnode n =  new MCTSnode()
        {
            parent = node,
            childrens = new List<MCTSnode>(),
            GameState = InterfaceGameState.instance.CreateInstance(node.GameState),
            numberWins = 0,
            numberSimulations = 0,
            FullyExpanded = false,
            action = ActionToplay
        };
        
        node.childrens.Add(n);
        lstnodes.Add(n);

        return n;
    }

    public MCTSnode Selection()
    {
        float ex=   Random.Range(0f,1f);
        MCTSnode result = null;
        Assert.AreNotEqual(lstnodes.Count,0);
        if (ex >= .8 || lstnodes.Count <= 1)
        { 
            result = lstnodes[Random.Range(0, lstnodes.Count)];
        }
        else
        {
            float maxRatio = 0;
            foreach (var Node in lstnodes)
            {
                if (Node.numberWins / (float) Node.numberSimulations >= maxRatio)
                {
                    maxRatio = Node.numberWins / (float) Node.numberSimulations;
                    result = Node;
                }
            }
        }

        Assert.IsNotNull(result);
        
        return result;
    }
    
    
    public int SimulateGame(MCTSnode node,Action action)
    {
        int numberVictoire = 0;
        Joueur j = null;

        GameState gameState = node.GameState;
        
        Assert.AreNotEqual(this.joueur,null);
        
        for (int i = 0; i < NUMBER_SIMULATION; i++)
        {
            
            float c = 0;
            bool finish = false;
            GameManager = InterfaceGameState.instance.getGameManager();
          //  DebugDrawer debug = DebugDrawer.instance;

            GameManager.initializeGame(gameState,true);
            gameState.simulation = true;
            GameManager.SetAction(this.joueur, action);
            //debug.DrawSphere(gameState.j1.getPosition(),1.5f,PrimitiveType.Cylinder);
            //debug.DrawSphere(gameState.j2.getPosition(),1.5f,PrimitiveType.Capsule);
            //debug.DrawSphere(gameState.frisbee.getPosition(),1.5f,PrimitiveType.Sphere);
          
            while ( !finish)
            {
                c++;

       
            
               
                
                GameManager.RunFrame(gameState);
                GameManager.RunSimulatedMovement(gameState);
                 
                (finish,j) = InterfaceGameState.instance.getGameManager().Goal(gameState);
                
                if (c >= 25000)
                {
                    Debug.LogError("Simulation à pris plus de  tics");
                    break;
                    
                }
                    
                //   Debug.DrawLine( gameState.j1.getPosition()-new Vector2(0,.1f),gameState.j1.getPosition()+new Vector2(0,.1f),Color.red,10f);
                // Debug.DrawLine( gameState.j2.getPosition()-new Vector2(0,.1f),gameState.j2.getPosition()+new Vector2(0,.1f),Color.green,10f);
                //Debug.DrawLine( gameState.frisbee.getPosition()-new Vector2(0,.1f),gameState.frisbee.getPosition()+new Vector2(0,.1f),Color.blue,10f);

                //  debug.DrawSphere(gameState.j1.getPosition(),0.5f,PrimitiveType.Cylinder);
                // debug.DrawSphere(gameState.j2.getPosition(),0.5f,PrimitiveType.Capsule);
                //debug.DrawSphere(gameState.frisbee.getPosition(),0.5f,PrimitiveType.Sphere);
                
               // Debug.Log("J1 ==> " + gameState.j1.getPosition() + " |" + "J2 ==> " + gameState.j2.getPosition() + " |"+ "Frisbee ==> " + gameState.frisbee.getPosition() + " |");
              
            }
            
            

            if (j.PlayerTag == this.joueur.PlayerTag)
            {
                numberVictoire++;
            }
            else if(numberVictoire > 0)
            {
                numberVictoire--;
            }
        
        }

        node.numberSimulations = NUMBER_SIMULATION;
        node.numberWins = numberVictoire;
        
        return numberVictoire;
    }
    
    
    
    
}
