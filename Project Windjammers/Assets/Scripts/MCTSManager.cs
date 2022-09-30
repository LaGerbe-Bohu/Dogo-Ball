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
    private const int NUMBER_TEST = 1000;
    private const int NUMBER_SIMULATION = 30;
    
    
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
            Assert.AreNotEqual(selectedNode.FullyExpanded,true);
            MCTSnode newNode = Expland(selectedNode);

            if (newNode == null)
            {
                continue;
            }
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
        if (ex <= 0.7f || lstnodes.Count <= 1)
        {
            int c = 0;
            do
            {
                c++;
              
                result = lstnodes[Random.Range(0, lstnodes.Count)];
            } while (result.FullyExpanded);



        }
        else
        {
            float maxRatio = 0;
            float c = 0;
            for (int i = 0; i < lstnodes.Count; i++)
            {
                MCTSnode Node = lstnodes[i];
                
                if (Node.numberWins / (float) Node.numberSimulations >= maxRatio && !Node.FullyExpanded )
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
        
        
        int c = 0;
        do
        {
            c++;

            if (c >= 25)
            {
                return null;
            }
            
           
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
                
              //  Assert.AreNotEqual(c,500);
                
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
        
        Joueur jmcts = node.GameState.j1.PlayerTag == this.joueur.PlayerTag
            ? node.GameState.j1
            : node.GameState.j2;
        
        Joueur Jplayer = node.GameState.j1.PlayerTag != this.joueur.PlayerTag
            ? node.GameState.j1
            : node.GameState.j2;
        
        GameManager.SetAction(jmcts, action);
        GameManager.SetRandomAction(Jplayer,GameManager.actions);
        int c = 0;
        
        Assert.AreEqual(gameState.simulation,true);

        
        while (jmcts.counter > 0)
        {
            c++;
            
            Assert.AreNotEqual(c,500);
          

            GameManager.RunFrame(gameState);
            GameManager.EmulateOneMove(gameState);
        }
        
    }
 
    
    public int SimulateGame(MCTSnode node,Action action)
    {
        int numberVictoire = 0;
        string j = "";

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
                
                
                Assert.AreNotEqual(c,15000);
                
            }

            if (j == this.joueur.PlayerTag)
            {
                numberVictoire++;
            }
            else if(numberVictoire > 0 && j != "goal")
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
           // lstnodes.Remove(node); 
        }


        int c = 0;
        while (n != null)
        {
          
            bool trouver = false;
            int i = 0;
            int j = 0;

            
            if (n.childrens.Count >= GameManager.actions.Count && n.parent != null)
            {
                
                bool v = true;
                for (int k = 0; k < n.childrens.Count; k++)
                {
                    v = v && n.childrens[k].FullyExpanded;
                }

                n.FullyExpanded = v;
            }
            
            c++;
            Assert.AreNotEqual(c,lstnodes.Count);
            
         

            if (n.FullyExpanded)
            {
                //lstnodes.Remove(n);
            }
            
            n.numberSimulations += NUMBER_SIMULATION;
            n.numberWins += victory;
            n = n.parent;
        }
       
    }


    
    
    
}
