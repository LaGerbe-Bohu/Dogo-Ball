using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.PostProcessing;

public struct Action
{
        public float time;
        public Vector2 direction;
}

public class GameManager : MonoBehaviour
{
        // Gestion du jeu 
        public Transform j1;
        public Transform j2;
        public Transform frisbee;
        private GameState gameState;
        private MCTSManager mcts;
        public Action MCTSaction; // Action trouvé par le MCTS 
        public List<Action> actions; // listes des actions possibles
        private float DELTA_TIME = 0.02f; // Mon DELTA_TIME Custom
        
        
        
        
        public List<BoxCollider2D> walls; // les murs pour la collisions, ils me servent à récupérer la taille de la zone de jeu
        public List<Transform> CenterPoint; // me permet de trouver les point central pour la calcul des collisions, vu qu'il ne sagit pas du centre du monde ni de la position initial des joueurs
        
        
        [HideInInspector]
        public float HeightPlayer = 0; // 
        [HideInInspector]
        public float WidthPlayer = 0;
        
        
        public AudioSource Hitball; // son de frappe de la balle
        public AudioSource Sifflet; // son du sifflet
        public AudioClip AC; // son des moutons 
        private AudioSource[] LstAC;
                
        
        public GameObject PanelPause; // panel de pause que j'enable

        
        private Vector2 tempVector; // variable temp pour éviter de devoir faire un new dans la simulation
        private Action tempAction; // variable temp pour éviter de devoir faire un new dans la simulation
        
        
        private void Start()
        {
                
                gameState = InterfaceGameState.instance.gameState;
                gameState.initialposf = frisbee.transform.position;
                mcts = new MCTSManager();
                actions = new List<Action>();
                gameState.timer = 30;
                
                DontDestroyOnLoad(Sifflet.gameObject); // je ne let destroy pas on load parce que je n'ai pas envie d'avoir de coupure dans le son , mais je destroy tout au menu pour pas avoir des gameobject qui s'empile à l'infini
                
                
                // je créer un tableau d'audio source avec le bruit de mouton, comme ca dès qu'il à un goal je joue pleins de son en même temps
                LstAC = new AudioSource[5];
                for (int i = 0; i < 5; i++)
                {
                        AudioSource AS =   Sifflet.AddComponent<AudioSource>();
                        AS.clip = AC;
                        LstAC[i] = AS;
                }

                HeightPlayer = walls[1].bounds.size.y; 
                WidthPlayer = walls[0].bounds.size.x;

                gameState.j1.sizeBound = new Vector2(WidthPlayer/2f, HeightPlayer);
                gameState.j2.sizeBound = new Vector2(WidthPlayer/2f, HeightPlayer);
               
                
                // les coups possibles de mon agent MCTS
                
                actions.Add(new Action(){direction = new Vector2(1,0), time = DELTA_TIME*5 });  
                actions.Add(new Action(){direction = new Vector2(-1,0), time = DELTA_TIME*5 });  
                actions.Add(new Action(){direction = new Vector2(0,1), time = DELTA_TIME*5 });  
                actions.Add(new Action(){direction = new Vector2(0,-1), time = DELTA_TIME*5 });  
                actions.Add(new Action(){direction = new Vector2(0,0), time = DELTA_TIME*5 });  
                
                
                // j'initialise mon gamestate

                initializeGame(gameState,false);
                
        }

        public void initializeGame(GameState gameState, bool simulation)
        {

                gameState.simulation = simulation;
                gameState.gameStarted = true;

                if (!gameState.simulation)
                {
                        gameState.frisbee.setDirection(new Vector2());
                        Sifflet.Play();
                }  
                
                
                if (gameState.frisbee.GetDirection() == new Vector2())      // lance la balle entre les deux joueurs
                {
                        float random = Random.Range(0, 1f);
                        if (random > .5f) { gameState.frisbee.setDirection((gameState.j1.getPosition() -gameState.frisbee.getPosition()).normalized);}
                        else { gameState.frisbee.setDirection((gameState.j2.getPosition() -gameState.frisbee.getPosition()).normalized); } 
                }
             
        }
        

        private void Update()
        {
                
                
                if (Input.GetKeyDown(KeyCode.R)) // ptit touche de reset
                {
                        SceneManager.LoadScene(0);
                }
                
                if (Input.GetKeyUp(KeyCode.Escape)) // la pause
                {


                        if (DELTA_TIME <= 0)
                        {
                                DELTA_TIME = 0.02f;
                                PanelPause.SetActive(false);
                        }
                        else
                        {
                                DELTA_TIME = 0;
                                PanelPause.SetActive(true);
                                
                        }
                }
                
              

                
                RunFrame(gameState); // run du jeu à chaque frame

        }
        
        private void FixedUpdate()
        {

                RunMovement(gameState); // run les mouvement
                ApplyMovement(gameState); // aplique les position au transform

        }

        public void EmulateOneMove(GameState gameState)
        {
                // cette fonction est appellé pour bouger dans le Expand seulement pour un coup
                if (gameState.movePlayer)
                {
                        Move(gameState.j1);
                }
                
                if ( gameState.moveAgent )
                {
                        Move(gameState.j2);
                }
                
                MoveFrisbee(gameState.frisbee);
        }
        
        public void RunSimulatedMovement(GameState gameState)
        {
                // cette fonction est spécialement faite pour la simulation, car elle les deux agents bougent de manière random
                
                if (!gameState.simulation) return;
                
                if (gameState.movePlayer)
                {
                        SetRandomAction(gameState.j1,actions);
                        Move(gameState.j1);
                }
                
                if ( gameState.moveAgent )
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
                
                if ( gameState.moveAgent )
                {
                        if (InterfaceGameState.IA && DELTA_TIME > 0)
                        {
                                SetMCTSACTION(gameState.j2,gameState,MCTSaction); // Appelle du cmts
                        }
                        else
                        {
                                SetRandomAction(gameState.j2,this.actions);
                        }
                      
                        Move(gameState.j2);
                  
                }
                
                
                MoveFrisbee(gameState.frisbee);
             
        }
        
        
        public void RunFrame(GameState gameState)
        {
                bool b = false;
                
                if (!b && gameState.endResetGame)
                {
                        ManageCatch(gameState);
                }
                
               Goal(gameState);

                gameState.timer -= DELTA_TIME / 3f;
                
                
                
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
                                gameState.moveAgent = true;
                                throwFrisbee(gameState.frisbee,new Vector2(-1,gameState.j2.getRawDirection().y));
                        }
                        else
                        {
                                gameState.frisbee.setCatched(false);
                                gameState.movePlayer = true;
                                throwFrisbee(gameState.frisbee,new Vector2(1,this.gameState.j1.getRawDirection().y));
                        }
                }
                
                
           
                if (isCatch(gameState.j1, gameState.frisbee) && gameState.oldThrower != gameState.j1  )
                {
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.setJoueur(gameState.j1);
                        gameState.frisbee.setPosition(gameState.j1.getPosition());
                        gameState.oldThrower = gameState.j1;
                        gameState.movePlayer = false;
                        

                }
 
                if (isCatch(gameState.j2, gameState.frisbee)  && gameState.oldThrower != gameState.j2  )
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
                        gameState.gameStarted = false;
                        gameState.moveAgent = true;
                }
                
        }
        
        public void resetGame(GameState gameState)
        {
      
                gameState.oldThrower = null;
                gameState.frisbee.setCatched(false);
                gameState.gameStarted = false;
                gameState.movePlayer = false;
                gameState.moveAgent = false;
    
                
                for (int i = 0; i < 5; i++)
                {
                        
                        LstAC[i].Play();
                        LstAC[i].pitch = Random.Range(0.9f, 1.7f);
                        LstAC[i].volume = Random.Range(1, 1.5f);
                }
                
                StopAllCoroutines();
                StartCoroutine(replace(gameState));
             
                gameState.gameStarted = true;
          
               
        }
        
        IEnumerator replace(GameState gameState)
        {
                float c = 1f;
                while (
                        gameState.j1.getPosition() != new Vector2(-15, 0) ||
                       gameState.j2.getPosition() != new Vector2(15, 0) || c >= 0
                       )
                {
                        c -= Time.deltaTime;
                        gameState.endResetGame = false;
                        gameState.frisbee.setDirection(new Vector2(0,0));
                        gameState.j1.setPosition( Vector2.MoveTowards(gameState.j1.getPosition(),
                                new Vector2(-15, 0), DELTA_TIME * 15f));
                        
                        gameState.j2.setPosition( Vector2.MoveTowards(gameState.j2.getPosition(),
                                new Vector2(15, 0), DELTA_TIME * 15f));
                        
                        
                        yield return new WaitForFixedUpdate();
                }
                
                Sifflet.Play();
                Sifflet.pitch = Random.Range(0.8f, 1.2f);
                
      
                gameState.frisbee.setDirection(new Vector2(0,0));
                float random = Random.Range(0, 1f);
             
                if (random > 0.5f)
                {
                        gameState.frisbee.setDirection( (gameState.j1.getPosition() - gameState.frisbee.getPosition()).normalized);
                }
                else
                {
                        gameState.frisbee.setDirection( (gameState.j2.getPosition() - gameState.frisbee.getPosition()).normalized);
                }
                
                gameState.endResetGame = true;
                
        }
        

        public (bool,string) Goal(GameState gameState)
        {
                // permet de dire si le joeuur à fini la partie ou non
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

                        return (true, "Timer");
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
                frisbee.setPosition( frisbee.getPosition() + frisbee.getSpeededDirection() *DELTA_TIME);
              
                if(IsCollide(frisbee.getPosition(),Vector2.zero, WidthPlayer,HeightPlayer))
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
             // permet de lancer la balle
                frisbee.setDirection(direction);
                

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
                
                if (gameState.InputPress || !gameState.endResetGame) return;
                

                joueur.setDirection(new Vector2(
                     h,
                     v));


                if (!IsCollide(
                        (joueur.getPosition() +
                         joueur.getDirection() * DELTA_TIME),
                        CenterPoint[0].position, (WidthPlayer) / 2f, HeightPlayer-1f ))
                {
                        joueur.setPosition( joueur.getPosition() + joueur.getDirection() * DELTA_TIME);        
                }
                
                

        }
        
        public void SetRandomAction(Joueur joueur,List<Action> actions)
        {
                if (joueur.counter > 0) return;
                
                tempAction = actions[Random.Range(0, actions.Count)];
                
                joueur.counter = tempAction.time;
                joueur.setDirection(tempAction.direction);
                
        }

        public void SetMCTSACTION(Joueur joueur,GameState gameState,Action a)
        {
                
                if (joueur.counter > 0) return;
                
                if (!gameState.simulation )
                { 
                       GC.Collect();
                       
                       MCTSaction = mcts.ComputeMCTS(joueur, InterfaceGameState.instance.CreateInstance(gameState)); // <== le truc qui prend 90% des perfs
                      
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
                joueur.counter -= DELTA_TIME;
                
                if (joueur.counter > 0)
                {
                        
                        if (!IsCollide((joueur.getPosition() + joueur.getDirection() * DELTA_TIME ),joueur.center,joueur.sizeBound.x,joueur.sizeBound.y))
                        { 
                                joueur.setPosition(   joueur.getPosition() + joueur.getDirection() * DELTA_TIME) ;
                        }

                    
                }
                
        }
        
        
}
