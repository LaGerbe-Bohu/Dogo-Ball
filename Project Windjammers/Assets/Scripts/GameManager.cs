using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Unity.Jobs;
using Random = UnityEngine.Random;


public struct Action
{
        public float time;
        public Vector2 direction;
}

public class GameManager : MonoBehaviour
{
        
        public List<BoxCollider2D> walls;
        public List<Transform> CenterPoint;
        
        [HideInInspector]
        public float HightPlayer = 0;
        [HideInInspector]
        public float WidthPlayer = 0;
        
        private List<Bounds> bounds;
        public Transform j1;
        public Transform j2;
        public Transform frisbee;
        
        private InputManager inputManager;
        private GameState gameState;
        private MCTSManager mcts;
        private Vector2 tempVector;
        
        [HideInInspector]
        public List<Action> actions;

        public Action MCTSaction;

        public AudioSource Hitball;
        public AudioSource Sifflet;
   
        
        private void Start()
        {
                
                gameState = InterfaceGameState.instance.gameState;
                gameState.initialposf = frisbee.transform.position;
                mcts = new MCTSManager();
                actions = new List<Action>();
                bounds = new List<Bounds>();
                gameState.timer = 30;
                
                

                HightPlayer = walls[^1].bounds.size.y; 
                WidthPlayer = walls[0].bounds.size.x;

                gameState.j1.sizeBound = new Vector2(WidthPlayer/2f, HightPlayer);
                gameState.j2.sizeBound = new Vector2(WidthPlayer/2f, HightPlayer);
               
                actions.Add(new Action(){direction = new Vector2(1,0), time = .02f*5 });  
                actions.Add(new Action(){direction = new Vector2(-1,0), time = .02f*5 });  
                actions.Add(new Action(){direction = new Vector2(0,1), time = .02f*5 });  
                actions.Add(new Action(){direction = new Vector2(0,-1), time = .02f*5 });  
                actions.Add(new Action(){direction = new Vector2(0,0), time = .02f*5 });  
                

                foreach (BoxCollider2D boxes in walls)
                {
                        bounds.Add(boxes.bounds);
                }



                initializeGame(gameState,false);
                
        }

        public void initializeGame(GameState gameState, bool simulation)
        {

                gameState.simulation = simulation;
                gameState.freezeInput = true;
                gameState.gameStarted = true;
                gameState.catchedTimer = 3;


                if (!gameState.simulation)
                {
                        gameState.frisbee.setDirection(new Vector2());
                        
                      
                        Sifflet.Play();

                }  
                
                
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
                
                RunFrame(gameState);

        }
        
        private void FixedUpdate()
        {

                RunMovement(gameState);
                ApplyMovement(gameState);

        }

        public void EmulateOneMove(GameState gameState)
        {
                if (gameState.movePlayer)
                {
                        Move(gameState.j1);
                }
                
                if ( gameState.moveRandom )
                {
                        Move(gameState.j2);
                }
                
                MoveFrisbee(gameState.frisbee);
        }
        
        public void RunSimulatedMovement(GameState gameState)
        {
                if (!gameState.simulation) return;
                
                if (gameState.movePlayer)
                {
                        SetRandomAction(gameState.j1,actions);
                        Move(gameState.j1);
                }
                
                if ( gameState.moveRandom )
                {
                        SetRandomAction(gameState.j2,actions);
                        Move(gameState.j2);
                        
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
                       
                   
                        SetMCTSACTION(gameState.j2,gameState,MCTSaction);
                        Move(gameState.j2);
                  
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

                gameState.timer -= 0.02f / 3f;
                
                
                
                if (gameState.j1.score.points >= 15 && !gameState.simulation)
                {
                        InterfaceGameState.J1set++;
                        SceneManager.LoadScene(1);
                }
                else if (gameState.j2.score.points >= 15 &&  !gameState.simulation)
                {
                        InterfaceGameState.J2set++;
                        SceneManager.LoadScene(1);  
                }


                if (!gameState.simulation && gameState.timer <= 0)
                {
                        if (gameState.j1.score.points > gameState.j2.score.points )
                        {
                                InterfaceGameState.J1set++;
                                SceneManager.LoadScene(1);
                        }
                        else if(gameState.j1.score.points < gameState.j2.score.points)
                        {
                                InterfaceGameState.J2set++;
                                SceneManager.LoadScene(1);  
                        }
                        else
                        {
                                InterfaceGameState.J1set++;
                                InterfaceGameState.J2set++;
                                SceneManager.LoadScene(1);  
                        }
                }
                
                

        }
        

        public void ManageCatch(GameState gameState)
        {
           
                
                if (gameState.frisbee.getIsCatched() )
                {
                        if (!gameState.simulation)
                        {
                                Hitball.Play();
                                Hitball.pitch = Random.Range(1.1f, 1.2f);
                        }
                        
                        if(gameState.frisbee.getJoueur() == gameState.j2)
                        {
                             
                                
                                gameState.frisbee.setCatched(false);
                                gameState.moveRandom = true;
                                throwFrisbee(gameState.frisbee,new Vector2(-1,gameState.j2.getRawDirection().y));
                        }
                        else
                        {
                                gameState.frisbee.setCatched(false);
                                gameState.movePlayer = true;
                                throwFrisbee(gameState.frisbee,new Vector2(1,this.gameState.j1.getRawDirection().y));
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
                
                Sifflet.Play();
                Sifflet.pitch = Random.Range(0.8f, 1.2f);
                
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
        

        public (bool,string) Goal(GameState gameState)
        {
                Frisbee frisbee = gameState.frisbee;
                Score scoreJ1 = gameState.j1.getScore();
                Score scoreJ2 = gameState.j2.getScore();

                string j = "";
                bool goal = false;

                
                
                if (gameState.timer <= 0)
                {
                     
                        frisbee.setDirection(new Vector2(0,0));
                        
                        if (!gameState.simulation)
                        {
                                frisbee.setPosition(gameState.initialposf);
                                gameState.timer = 30;
                                InterfaceGameState.instance.getGameManager().resetGame(gameState);
                                
                        }

                        return (true, "Goal");
                }
                
                
                if (frisbee.getPosition().x >= 19 ) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ1.points+=3;
                               
                        }
                        else {
                                scoreJ1.points+=5;
                        }
                        
                        frisbee.setDirection(new Vector2(0,0));
                        if (!gameState.simulation)
                        {
                                frisbee.setPosition(gameState.initialposf);
                              
                                InterfaceGameState.instance.getGameManager().resetGame(gameState);
                        }
                        
                        goal = true;

                        j = gameState.j1.PlayerTag;
                        
                }
                if (frisbee.getPosition().x <= -19 ) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ2.points+=3;
                        }
                        else {
                                scoreJ2.points+=5;
                        }
                       
                        goal = true;
                        
                        frisbee.setDirection(new Vector2(0,0));
                        if (!gameState.simulation)
                        {
                                frisbee.setPosition(gameState.initialposf);
                                
                                InterfaceGameState.instance.getGameManager().resetGame(gameState);
                        }
                        
                        j = gameState.j2.PlayerTag;
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
                frisbee.setPosition( frisbee.getPosition() + frisbee.getSpeededDirection() *0.02f);
              
                if(InterfaceGameState.instance.getGameManager().IsCollide(frisbee.getPosition(),Vector2.zero, WidthPlayer,HightPlayer))
                {
                        tempVector.x = frisbee.GetDirection().x;
                        tempVector.y = -frisbee.GetDirection().y;
                        frisbee.setDirection(tempVector);
            
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

                for (int i = 0; i < bounds.Count; i++)
                {
                        if(bounds[i].Contains(pos) )
                        {
                                find = true;
                        }
                }

                return find;
        }

        public bool IsCollide(Vector2 originPoint, Vector2 center, float width,float height)
        {
                
                
                if (Math.Abs(originPoint.x - center.x) > width / 2 || Math.Abs(originPoint.y - center.y) > height / 2)
                {
                        return true;
                }

                return false;
        }
        

        private void throwFrisbee(Frisbee frisbee, Vector2 direction)
        {
             
                frisbee.setDirection(direction);
                

        }       
        
        public bool collisionWall(Vector3 pos,bool t)
        {
                bool find = false;

                for (int i = 0; i < bounds.Count; i++)
                {
                        if(bounds[i].Contains(pos) &&  walls[i].name != "MiddleWall")
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


                if (!IsCollide(
                        (joueur.getPosition() +
                         joueur.getDirection() * 0.02f),
                        CenterPoint[0].position, (WidthPlayer) / 2f, HightPlayer-1f ))
                {
                        joueur.setPosition( joueur.getPosition() + joueur.getDirection() * 0.02f);        
                }
                
                

        }
        
        public void SetRandomAction(Joueur joueur,List<Action> actions)
        {
                if (joueur.counter > 0) return;
                
                gameState.randomAction = actions[Random.Range(0, actions.Count)];
                
                joueur.counter = gameState.randomAction.time;
                joueur.setDirection(gameState.randomAction.direction);
                
        }

        public void SetMCTSACTION(Joueur joueur,GameState gameState,Action a)
        {
              
             
                if (joueur.counter > 0) return;
                
                if (!gameState.simulation )
                { 
                        GC.Collect();
                       MCTSaction = mcts.ComputeMCTS(joueur, InterfaceGameState.instance.CreateInstance(gameState));
                      
                }

                joueur.counter = a.time;
                joueur.setDirection(a.direction);
        }
        
        
        public void SetAction(Joueur joueur,Action a)
        {
          
                joueur.counter = a.time;
                joueur.setDirection(a.direction);
                
        }
        
        private void Move(Joueur joueur)
        {
                joueur.counter -= 0.02f;
                
                if (joueur.counter > 0)
                {
                        
                        if (!IsCollide((joueur.getPosition() + joueur.getDirection() * 0.02f ),joueur.center,joueur.sizeBound.x,joueur.sizeBound.y))
                        { 
                                joueur.setPosition(   joueur.getPosition() + joueur.getDirection() * 0.02f) ;
                        }

                    
                }
                
        }
        
        
}
