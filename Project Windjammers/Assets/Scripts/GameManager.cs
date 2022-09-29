using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public struct Action
{
        public float time;
        public Vector2 direction;
}



public class GameManager : MonoBehaviour
{
        public List<BoxCollider2D> walls;
        public Transform j1;
        public Transform j2;
        public Transform frisbee;
        
        private InputManager inputManager;
        private GameState gameState;
        private MCTSManager mcts;
        
        private bool movePlayer = false;

        [HideInInspector]
        public List<Action> actions;

        private float count = 0;
        
        public Action MCTSaction;
        
        // Utiliser GameState.[qqch] pour récuperer le joueur par exemple pour le frisbee
        // Ca serait bien qu'il y ai que cette classe qui ai le droit de modifier ces objets
        
        // Intégrer ici je pense : Les déplacement de joueur, le lancé de frisbee ainsi que les 
        // le rebou de celui-ci sur le mur. Le catch.
        // 

        private void Start()
        {

                gameState = InterfaceGameState.instance.gameState;
                gameState.initialposf = frisbee.transform.position;
                mcts = new MCTSManager();
                actions = new List<Action>();

                gameState.timer = 30;
                
                for (int x = -1; x < 2 ; x++)
                {
                        for (int y = -1; y < 2; y++)
                        {
                                for (float t = 0.05f; t < .3f; t+=0.025f)
                                {
                                        actions.Add(new Action(){direction = new Vector2(x,y), time = t });  
                                }

                              
                        }
                        

                      
                }
                
                


                initializeGame(gameState,false);
                
             
              
        }

        public void initializeGame(GameState gameState, bool simulation)
        {

                gameState.simulation = simulation;
                gameState.freezeInput = true;
                gameState.gameStarted = true;
                gameState.catchedTimer = 3;
                gameState.movePlayer = false;

                if (gameState.frisbee.GetDirection() == new Vector2())      
                {
                        float random = Random.Range(0, 1f);
                        if (random > .5f) { gameState.frisbee.setDirection((gameState.j1.getPosition() -gameState.frisbee.getPosition()).normalized);}
                        else { gameState.frisbee.setDirection((gameState.j2.getPosition() -gameState.frisbee.getPosition()).normalized); } 
                }
             
        }
        

        private void Update()
        {
                
                
                if (Input.GetKeyDown(KeyCode.A))
                {
                        SceneManager.LoadScene(0);
                }
                
                
                if (Input.GetKeyDown(KeyCode.B))
                {
                        
                       // mcts.SimulateGame(InterfaceGameState.instance.CreateInstance(gameState),new Action(){ direction = Vector2.one, time = 0});
                        
                      
                }


                RunFrame(gameState);

        }
        
        private void FixedUpdate()
        {

                RunMovement(gameState);
                ApplyMovement(gameState);

        }

        public void RunSimulatedMovement(GameState gameState)
        {
                if (!gameState.simulation) return;
                
                if (gameState.movePlayer)
                {
                        SetRandomAction(gameState.j1,actions);
                        RandomMove(gameState.j1);
                }
                
                if ( gameState.moveRandom )
                {
                        SetRandomAction(gameState.j2,actions);
                        RandomMove(gameState.j2);
                        
                }
                
                MoveFrisbee(gameState.frisbee);
        }
        
        public void RunMovement(GameState gameState)
        {
                
                if (gameState.movePlayer)
                {
                        MovePlayer(gameState.j1);
                }
                
                if ( gameState.moveRandom )
                {
                       
                   
                        SetMCTSACTION(gameState.j2,MCTSaction);
                        MoveMCTS(gameState.j2);
                      
                        // RandomMove(gameState.j2);
                }
                
                MoveFrisbee(gameState.frisbee);
             
        }

        public void RunFrame(GameState gameState)
        {
                bool b = Goal(gameState).Item1;
                
                if (!b && gameState.endResetGame)
                {
                        ManageCatch(gameState);
                }

                gameState.timer -= 0.02f;
                
                
                
                if (gameState.j1.score.points >= 15 && !gameState.simulation)
                {
                        InterfaceGameState.J1set++;
                        SceneManager.LoadScene(0);
                }
                else if (gameState.j2.score.points >= 15 &&  !gameState.simulation)
                {
                        InterfaceGameState.J2set++;
                        SceneManager.LoadScene(0);  
                }
                
                

        }
        

        public void ManageCatch(GameState gameState)
        {
           
                
                if (gameState.frisbee.getIsCatched() )
                {
                        
                        if (gameState.frisbee.getJoueur() == gameState.j1 && !gameState.simulation )
                        {
                                gameState.catchedTimer -= 0.02f;
                                if ( (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.Space)) || gameState.catchedTimer<=0)
                                {
                                        gameState.freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee,new Vector2(1,0));
                                        gameState.catchedTimer = 3;
                                        gameState.movePlayer = true;
                                     
                                        
                                }
                        
                                if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow) )
                                {       
                                        gameState.freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee, new Vector2(1, -1));
                                        gameState.catchedTimer = 3;
                                        gameState.movePlayer = true;
                                        MCTSaction = mcts.ComputeMCTS(this.gameState.j2, InterfaceGameState.instance.CreateInstance(gameState));
                                }
                        
                                if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.UpArrow) )
                                {
                                        gameState.freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee,new Vector2(1,1));
                                        gameState.catchedTimer = 3;
                                        gameState.movePlayer = true;
                                        MCTSaction = mcts.ComputeMCTS(this.gameState.j2, InterfaceGameState.instance.CreateInstance(gameState));
                                }
                                
                        }
                        else if(gameState.frisbee.getJoueur() == gameState.j2)
                        {
                                
                                gameState.frisbee.setCatched(false);
                                gameState.moveRandom = true;
                                throwFrisbee(gameState.frisbee,new Vector2(-1,Random.Range(-1,2)));
                        }
                        else
                        {
                                gameState.frisbee.setCatched(false);
                                gameState.movePlayer = true;
                                throwFrisbee(gameState.frisbee,new Vector2(1,Random.Range(-1,2)));
                        }
                }
                
                
                if (isCatch(gameState.j1, gameState.frisbee) && gameState.oldThrower != gameState.j1 && gameState.endResetGame  )
                {
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.setJoueur(gameState.j1);
                        gameState.frisbee.setPosition(gameState.j1.getPosition());
                        gameState.oldThrower = gameState.j1;
                        gameState.movePlayer = false;
                        

                }
 
                if (isCatch(gameState.j2, gameState.frisbee)  && gameState.oldThrower != gameState.j2 && gameState.endResetGame  )
                {
                        //  ManageCatch();
                        StopAllCoroutines();
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.setJoueur(gameState.j2);
                        gameState.frisbee.setPosition(gameState.j2.getPosition());
                        gameState.oldThrower = gameState.j2;
                        
                }
                
                if (gameState.frisbee.getIsCatched() && gameState.gameStarted)
                {
                        gameState.movePlayer = true;
                        gameState.freezeInput = false;
                        gameState.gameStarted = false;
                        gameState.moveRandom = true;
                }
                
        }
        
        public void resetGame(GameState gameState)
        {
                gameState.freezeInput = true;
                gameState.oldThrower = null;
                gameState.gameStarted = false;
                gameState.movePlayer = false;
                gameState.moveRandom = false;
               
             
                
                StopAllCoroutines();
                StartCoroutine(replace(gameState));
                
                gameState.gameStarted = true;
                gameState.endResetGame = false;
               
        }
        
        IEnumerator replace(GameState gameState)
        {
          
                while (
                        gameState.j1.getPosition() != new Vector2(-15, 0) ||
                       gameState.j2.getPosition() != new Vector2(15, 0)
                       )
                {
                        gameState.j1.setPosition( Vector2.MoveTowards(gameState.j1.getPosition(),
                                new Vector2(-15, 0), 0.02f * 15f));
                        
                        gameState.j2.setPosition( Vector2.MoveTowards(gameState.j2.getPosition(),
                                new Vector2(15, 0), 0.02f * 15f));
                        
                        
                        
                        
                        yield return new WaitForFixedUpdate();
                }
                
                float random = Random.Range(0, 1f);
                gameState.endResetGame = true;
                if (random > 0.5f)
                {
                        gameState.frisbee.setDirection( (gameState.j1.getPosition() - gameState.frisbee.getPosition()).normalized);
                }
                else
                {
                        gameState.frisbee.setDirection( (gameState.j2.getPosition() - gameState.frisbee.getPosition()).normalized);
                }
                
                
        }
        

        public (bool,Joueur) Goal(GameState gameState)
        {
                Frisbee frisbee = gameState.frisbee;
                Score scoreJ1 = gameState.j1.getScore();
                Score scoreJ2 = gameState.j2.getScore();

                Joueur j = null;
                bool goal = false;



                if (gameState.timer <= 0)
                {
                        frisbee.setPosition(gameState.initialposf);
                        frisbee.setDirection(new Vector2(0,0));
                        InterfaceGameState.instance.getGameManager().resetGame(gameState);
                        this.gameState.timer = 30;
                        return (true, new Joueur(Vector2.zero,Vector2.zero, 0,"personne",new Score(0,0)));
                }
                
                
                
                if (frisbee.getPosition().x >= 19 ) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ1.points+=3;
                               
                        }
                        else {
                                scoreJ1.points+=5;
                        }
                        frisbee.setPosition(gameState.initialposf);
                        frisbee.setDirection(new Vector2(0,0));
                        InterfaceGameState.instance.getGameManager().resetGame(gameState);
                        
                        goal = true;

                        j = gameState.j1;

                }
                if (frisbee.getPosition().x <= -19 ) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ2.points+=3;
                        }
                        else {
                                scoreJ2.points+=5;
                        }
                        frisbee.setPosition(gameState.initialposf);
                        frisbee.setDirection(new Vector2(0,0));
                        goal = true;
                        InterfaceGameState.instance.getGameManager().resetGame(gameState);
                        j = gameState.j2;
                }

                return (goal,j);
        }


        private void ApplyMovement(GameState gameState)
        {
                frisbee.transform.position = gameState.frisbee.getPosition();
                j1.transform.position = gameState.j1.getPosition();
                j2.transform.position = gameState.j2.getPosition();
        }
        
        private void MoveFrisbee(Frisbee frisbee)
        {
                frisbee.setPosition( (Vector3)frisbee.getPosition() + (Vector3)frisbee.GetDirection().normalized*0.02f*frisbee.Speed);
              
                if(InterfaceGameState.instance.getGameManager().collisionWall(frisbee.getPosition(),true))
                {    
                        frisbee.setDirection(new Vector2(frisbee.GetDirection().x, -frisbee.GetDirection().y));
            
                }
        }
        
        private bool isCatch(Joueur j, Frisbee f)
        {
                float distance = Vector2.Distance(j.getPosition(), f.getPosition());
                
                if(distance < 2.5f)
                {
                        return true;
                }
                return false;
        }

        public bool collisionWall(Vector3 pos)
        { 
                bool find = false;
                foreach (BoxCollider2D boxes in walls)
                {
                        if (boxes.bounds.Contains(pos))
                        {
                                find = true;
                        }
                }

                return find;
        }


        private void throwFrisbee(Frisbee frisbee, Vector2 direction)
        {
             
                frisbee.setDirection(direction);
                

        }       
        
        public bool collisionWall(Vector3 pos,bool t)
        {
                bool find = false;
                foreach (BoxCollider2D boxes in walls)
                {
                        if (boxes.bounds.Contains(pos) && boxes.name != "MiddleWall")
                        {
                                find = true;
                        }
                }

                return find;
        }
        
        private void MovePlayer(Joueur joueur)
        {
                float v = Input.GetAxisRaw("Vertical") ;
                float y = joueur.getPosition().y;
                
                float h = Input.GetAxisRaw("Horizontal");
                float x = joueur.getPosition().x;

                if (h == 0 && v == 0)
                {
                        gameState.InputPress = false;
                }
                
                if (gameState.InputPress || gameState.freezeInput) return;
                

                joueur.setDirection(new Vector2(
                     h,
                     v));

                if (!collisionWall((Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                { 
                      joueur.setPosition( (Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f);        
                }
                

        }
        
        IEnumerator RandomMove(Joueur joueur, bool f)
        {
                float counter = Random.Range(0.05f,0.3f);
                joueur.setDirection(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)));
                
                
                while (counter >0)
                {
                        counter -= 0.02f;
                        
                        
                        if (!collisionWall((Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                        { 
                                joueur.setPosition(   (Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f) ;
                        }
                        
                        
                        yield return new WaitForFixedUpdate();
                }
                StartCoroutine( RandomMove(gameState.j2,true)); 
        }
        
        private void SetRandomAction(Joueur joueur,List<Action> actions)
        {
                if (joueur.counter > 0) return;
                
                Action a = actions[Random.Range(0, actions.Count)];
                
                joueur.counter = a.time;
                joueur.setDirection(a.direction);
                
        }

        public void SetMCTSACTION(Joueur joueur,Action a)
        {
              
             
                if (joueur.counter > 0) return;
                
                if (!gameState.simulation)
                {
                        if (!gameState.frisbee.getIsCatched() && gameState.frisbee.getJoueur().PlayerTag == gameState.j1.PlayerTag)
                        {
                                MCTSaction = mcts.ComputeMCTS(joueur, InterfaceGameState.instance.CreateInstance(gameState));
                        } 
                }
                
                
                
                joueur.counter = a.time;
                joueur.setDirection(a.direction);
        }
        
        public void SetAction(Joueur joueur,Action a)
        {
          
                joueur.counter = a.time;
                joueur.setDirection(a.direction);
                
        }

        private bool MoveMCTS(Joueur joueur)
        {
                joueur.counter -= 0.02f;
                
                if (joueur.counter > 0)
                {
                        if (!collisionWall((Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                        { 
                                joueur.setPosition(   (Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f) ;
                        }

                        return true;
                }

                return false;
        }
        
        
        private bool RandomMove(Joueur joueur)
        {
                joueur.counter -= 0.02f;
                
                if (joueur.counter > 0)
                {
                        if (!collisionWall((Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                        { 
                                joueur.setPosition(   (Vector3)joueur.getPosition() + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f) ;
                        }

                        return true;
                }
                
                return false;
        }
        
        
        
        
        

        
        
        
}
