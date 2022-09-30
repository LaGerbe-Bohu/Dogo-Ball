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
    private const int NUMBER_TEST = 500;
    private const int NUMBER_SIMULATION = 60;
    
    
    private GameManager GameManager;
    private List<MCTSnode> lstnodes;
    private Joueur joueur;
    
    public Action ComputeMCTS(Joueur joueur,GameState gameState)
    {
        lstnodes = new List<MCTSnode>();
        GameManager = InterfaceGameState.instance.gameManager;
        this.joueur = joueur;
        
        
        
        GameManager.initializeGame(gameState,true);
        gameState.simulation = true;
        
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
            Backpropagation(newNode,victoire);
          
        }

        float ratio = 0;
        MCTSnode n = null;
        for (int i = 0; i < startNode.childrens.Count; i++)
        {
            MCTSnode node = startNode.childrens[i];
            
            if ( (node.numberWins / (float) node.numberSimulations) >= ratio)
            {
                ratio = node.numberWins / (float) node.numberSimulations;
                n = node;
            }
        }
        
        
        Assert.IsNotNull(n);

        return n.action;
    }
    
    public MCTSnode Selection()
    {
        float ex=   Random.Range(0f,1f);
        MCTSnode result = null;
        Assert.AreNotEqual(lstnodes.Count,0);
        if (ex <= 0.8 || lstnodes.Count <= 1)
        {

            result = lstnodes[Random.Range(0, lstnodes.Count)];
        
            
        }
        else
        {
            float maxRatio = 0;
            float c = 0;
            for (int i = 0; i < lstnodes.Count; i++)
            {
                MCTSnode Node = lstnodes[i];
                
                if (Node.numberWins / (float) Node.numberSimulations >= maxRatio )
                {
                    
                    maxRatio = Node.numberWins / (float) Node.numberSimulations;
                    result = Node;
                }
            }
        }

        Assert.IsNotNull(result);
        
        return result;
    }

    public MCTSnode Expland(MCTSnode node)
    {
        
        Assert.IsNotNull(node);
        Action ActionToplay;
        
        do
        {
            ActionToplay = GameManager.actions[Random.Range(0,GameManager.actions.Count)];
        } while (findSameInput(node.parent,ActionToplay));


        SimulateAction(node,ActionToplay); // Pour jouer un coup dans la simulation
        
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
    
    bool findSameInput(MCTSnode root,Action action)
    {
        // cette fonction permet de trouver une action dans les enfant de la node
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
    
    public void SimulateAction(MCTSnode node,Action action)
    {
     
        
        GameState gameState = node.GameState;
        
        Joueur j = node.GameState.j1.PlayerTag == this.joueur.PlayerTag
            ? node.GameState.j1
            : node.GameState.j2;
        
        GameManager.SetAction(j, action);
        int c = 0;
        
        Assert.AreEqual(gameState.simulation,true);

        
        while (j.counter > 0)
        {
            c++;
            
            Assert.AreNotEqual(c,500);
            
            if (c >= 100)
            {
                break;
            }

            GameManager.RunFrame(gameState);
            GameManager.EmulateOneMove(gameState);
        }
        
    }
 
    
    public int SimulateGame(MCTSnode node,Action action)
    {
        int numberVictoire = 0;
        Joueur j = null;

        GameState gameState = InterfaceGameState.instance.CreateInstance(node.GameState);
        
        Assert.AreNotEqual(this.joueur,null);
        
        for (int i = 0; i < NUMBER_SIMULATION; i++)
        {
            
            float c = 0;
            bool finish = false;
            GameManager = InterfaceGameState.instance.getGameManager();
     
          
            while ( !finish)
            {
                c++;
                
                GameManager.RunFrame(gameState);
                GameManager.RunSimulatedMovement(gameState);
                 
                (finish,j) = InterfaceGameState.instance.getGameManager().Goal(gameState);
                
                if (c >= 15000)
                {
                    Debug.LogError("Garde de fou simulation");
                    break;
                    
                }
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
    
        
    public void Backpropagation(MCTSnode node,int victory)
    {
        MCTSnode n = node.parent;

        if (GameManager.Goal(node.GameState).Item1)
        {
            node.FullyExpanded = true;
            lstnodes.Remove(node); 
        }

      
        
        while (n != null)
        {
            bool trouver = false;
            for (int i = 0; i < n.childrens.Count; i++)
            {
                if (!n.childrens[i].FullyExpanded)
                {
                    trouver = true;
                }
            }

            n.FullyExpanded = !trouver;

            if (n.FullyExpanded)
            {
                lstnodes.Remove(node);
            }
            
            n.numberSimulations += NUMBER_SIMULATION;
            n.numberWins += victory;
            n = n.parent;
        }
       
    }


    
    
    
}
