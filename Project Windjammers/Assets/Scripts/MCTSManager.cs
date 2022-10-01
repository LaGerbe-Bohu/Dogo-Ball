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

    public MCTSnode()
    {
        this.childrens = new List<MCTSnode>();
    }
    
}


public class MCTSManager
{
    private const int NUMBER_TEST = 2500; // 1500
    private const int NUMBER_SIMULATION = 30;

    private MCTSnode[] lstNodes;
    private GameManager GameManager;
    //private List<MCTSnode> lstnodes;
    private Joueur joueur;
    private int currentidx;
    public MCTSManager()
    {
        lstNodes = new MCTSnode[NUMBER_TEST + 500] ;
        
        for (int i = 0; i < lstNodes.Length; i++)
        {
            lstNodes[i] = new MCTSnode();
        }
        
    }

    public void ClearNode()
    {
        
        for (int i = 0; i < lstNodes.Length; i++)
        {
            lstNodes[i].GameState = null;
            lstNodes[i].childrens = new List<MCTSnode>();
            lstNodes[i].FullyExpanded = false;
            lstNodes[i].parent = null;
            lstNodes[i].numberWins = 0;
            lstNodes[i].numberSimulations = 0;
       
        }
    }
    

    
    public Action ComputeMCTS(Joueur joueur,GameState gameState)
    {
        
        ClearNode();
        GameManager = InterfaceGameState.instance.gameManager;
        this.joueur = joueur;
        
        
        
        GameManager.initializeGame(gameState,true);
        gameState.simulation = true;

        Assert.AreNotEqual(lstNodes[0],null);

        lstNodes[0].GameState = gameState;
        
        currentidx = 0;
        for (int i = 0; i < NUMBER_TEST; i++)
        {
            if (lstNodes[0].FullyExpanded) break;
            
            MCTSnode selectedNode = Selection();
            
            Assert.AreNotEqual(selectedNode.childrens.Count,GameManager.actions.Count);
            Assert.AreNotEqual(selectedNode.FullyExpanded,true);
            MCTSnode newNode = Expland(selectedNode,currentidx);
            
            int victoire = SimulateGame(newNode, newNode.action);
            Backpropagation(newNode,victoire);
            currentidx++;
        }

        float ratio = float.MinValue;
        MCTSnode n = null;
        for (int i = 0; i < lstNodes[0].childrens.Count; i++)
        {
            MCTSnode node = lstNodes[0].childrens[i];
            
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


        if (ex <= 0.75f || currentidx <= 1)
        {
            int c = 0;
            do
            {
                c++;
                if (c >= (currentidx+1)*20)
                {
                    Debug.LogError("ca");
                    break;
                    
                }
                
                Assert.AreNotEqual(c,(currentidx+1)*20);
                result = lstNodes[Random.Range(0, currentidx)];
            } while (result.FullyExpanded || result.childrens.Count >= GameManager.actions.Count);

           
        }
        else
        {
            float maxRatio = float.MinValue;
            float c = 0;
            for (int i = 0; i < currentidx; i++)
            {
                MCTSnode Node = lstNodes[i];
                
                if (Node.numberWins / (float) Node.numberSimulations >= maxRatio && !Node.FullyExpanded && Node.childrens.Count < GameManager.actions.Count )
                {
                    
                    maxRatio = Node.numberWins / (float) Node.numberSimulations;
                    result = Node;
              
                }
            }
        }
        
        
        return result;
    }

    public MCTSnode Expland(MCTSnode node,int current)
    {
        
        Assert.IsNotNull(node);
        Assert.AreNotEqual(node.FullyExpanded,true);
        Assert.AreNotEqual(node.childrens.Count,GameManager.actions.Count);
        Action ActionToplay;
        
        
        int c = 0;
        do
        {
            c++;

      
            
            ActionToplay = GameManager.actions[Random.Range(0,GameManager.actions.Count)];
            
            if (c >= 10000)
            {
                Debug.Log("");
                Debug.LogError("c'est ca");
               // Debug.LogError("garde fou");
                break;
            }
            
        } while (findSameInput(node,ActionToplay));
        
        
        SimulateAction(node,ActionToplay); // Pour jouer un coup dans la simulation
        

        lstNodes[current+1].parent = node;
        lstNodes[current+1].GameState = InterfaceGameState.instance.CreateInstance(node.GameState);
        lstNodes[current+1].action = ActionToplay;
        node.childrens.Add(lstNodes[current+1]);
        

        return  lstNodes[current+1];
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
                
                if (c >= 500)
                {
                    Debug.LogError("garde fou");
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
            
            if (c >= 500)
            {
                Debug.LogError("garde fou");
                break;
            }
          

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
                
                if (c >= 1000)
                {
                    Debug.LogError("garde fou");
                    break;
                }
                
                GameManager.RunFrame(gameState);
                GameManager.RunSimulatedMovement(gameState);
                 
                (finish,j) = InterfaceGameState.instance.getGameManager().Goal(gameState);
                
                
                Assert.AreNotEqual(c,15000);
                
            }

            if (j == this.joueur.PlayerTag)
            {
                numberVictoire++;
            }
            else if(j != "goal")
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
        }


        int c = 0;
        while (n != null)
        {
          
            bool trouver = false;
            int i = 0;
            int j = 0;
            
            
            
            if (n.childrens.Count >= GameManager.actions.Count )
            {
                
                bool v = true;
                for (int k = 0; k < n.childrens.Count; k++)
                {
                    v = v && n.childrens[k].FullyExpanded;
                }

                n.FullyExpanded = v;
            }
            
            if (c >= (currentidx+1)*2)
            {
                Debug.LogError("garde fou");
                break;
            }
            
            c++;
            

            n.numberSimulations += NUMBER_SIMULATION;
            n.numberWins += victory;
            n = n.parent;
        }
       
    }


    
    
    
}
