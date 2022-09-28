using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
        public List<BoxCollider2D> walls;

        public float MaxSpeed;
        private InputManager inputManager;
        private GameState gameState;

        private Coroutine co;
        private bool InputPress = false;
        private bool movePlayer = false;
        private bool moveRandom = false;
        private bool freezeInput = false;

        private bool gameStarted = false;

        private Joueur oldThrower;
        private Vector2 initialposf;
        private bool endResetGame = true;
        
        // Utiliser GameState.[qqch] pour récuperer le joueur par exemple pour le frisbee
        // Ca serait bien qu'il est que cette classe qui est le droit de modifier ces objets
        
        // Intégrer ici je pense : Les déplacement de joueur, le lancé de frisbee ainsi que les 
        // le rebou de celui-ci sur le mur. Le catch.
        // 

        private void Start()
        {
                gameState = GameState.Instance;
                freezeInput = true;
                gameStarted = true;
                initialposf = gameState.frisbee.transform.position;
                
        }

        private void Update()
        {
                
                
                if (Input.GetKeyDown(KeyCode.A))
                {
                        SceneManager.LoadScene(0);
                }


                Goal(gameState);

                if (gameState.j1.score.points >= 15)
                {
                        GameState.J1set++;
                        SceneManager.LoadScene(0);
                }
                else if (gameState.j2.score.points >= 15)
                {
                        GameState.J2set++;
                        SceneManager.LoadScene(0);  
                }
                
                

                if (gameState.frisbee.getIsCatched() )
                {
                        if (gameState.frisbee.getJoueur() == gameState.j1 )
                        {
                                
                                if ( (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.Space)) )
                                {
                                        freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee,new Vector2(1,0));
                                }
                        
                                if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow) )
                                {       
                                        freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee, new Vector2(1, -1));
                                }
                        
                                if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.UpArrow) )
                                {
                                        freezeInput = false;
                                        gameState.frisbee.setCatched(false);
                                        throwFrisbee(gameState.frisbee,new Vector2(1,1));
                                }   
                        }
                        else
                        {
                                
                                gameState.frisbee.setCatched(false);
                              
                         
                                moveRandom = true;
                                throwFrisbee(gameState.frisbee,new Vector2(-1,Random.Range(-1,2)));
                        }
                      
                     
                      
                }
                
                
                
                if (isCatch(gameState.j1, gameState.frisbee) && oldThrower != gameState.j1 && endResetGame  )
                {
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.setJoueur(gameState.j1);
                        gameState.frisbee.setPosition(gameState.j1.getPosition());
                        oldThrower = gameState.j1;
                        movePlayer = false;

                }
                else if (isCatch(gameState.j2, gameState.frisbee)  && oldThrower != gameState.j2 && endResetGame  )
                {
                        //  ManageCatch();
                        StopAllCoroutines();
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.setJoueur(gameState.j2);
                        gameState.frisbee.setPosition(gameState.j2.getPosition());
                        oldThrower = gameState.j2;
                        
                }
                
                
                if (!gameState.frisbee.getIsCatched())
                {
                        gameState.frisbee.setCatched(false);
                        movePlayer = true;

                      
                }

                if (gameState.frisbee.getIsCatched() && gameStarted)
                {
                        freezeInput = false;
               
                        
                        moveRandom = true;
                        gameStarted = false;
                } 
             
             

                Debug.Log(gameState.frisbee.getIsCatched()+"&&"+ gameStarted);
        }

        
        public void resetGame()
        {
                freezeInput = true;
                oldThrower = null;
                gameStarted = false;
                
                StopAllCoroutines();
                StartCoroutine(replace());

                gameStarted = true;


        }
        
        IEnumerator replace()
        {
                endResetGame = false;
                while (
                        gameState.j1.getPosition() != new Vector2(-15, 0) ||
                       gameState.j2.getPosition() != new Vector2(15, 0)
                       )
                {
                        gameState.j1.transform.position = Vector2.MoveTowards(gameState.j1.transform.position,
                                new Vector2(-15, 0), 0.02f * 10f);
                        
                        gameState.j2.transform.position = Vector2.MoveTowards(gameState.j2.transform.position,
                                new Vector2(15, 0), 0.02f * 10f);
                        
                        
                        yield return null;
                }
                
                float random = Random.Range(0, 1f);
                endResetGame = true;
                if (random > 0.5f)
                {
                        gameState.frisbee.setDirection( (gameState.j1.transform.position - gameState.frisbee.transform.position).normalized);
                }
                else
                {
                        gameState.frisbee.setDirection( (gameState.j2.transform.position - gameState.frisbee.transform.position).normalized);
                }
                
        }

        private void FixedUpdate()
        {

                if (movePlayer)
                {
                        MovePlayer(gameState.j1);
                }

                if ( moveRandom )
                {
                        StopAllCoroutines();
                        co = StartCoroutine(RandomMove(gameState.j2, true));
                        moveRandom = false;
                }
                
                MoveFrisbee(gameState.frisbee);
                
            
        }


        private bool Goal(GameState gameState)
        {
                Frisbee frisbee = gameState.frisbee;
                Score scoreJ1 = gameState.j1.getScore();
                Score scoreJ2 = gameState.j2.getScore();

                bool goal = false;
                
                if (frisbee.getPosition().x >= 19 && frisbee.getPosition().x <= 21) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ1.points+=3;
                               
                        }
                        else {
                                scoreJ1.points+=5;
                        }
                        frisbee.setPosition(initialposf);
                        frisbee.setDirection(new Vector2(0,0));
                        gameState.gameManager.resetGame();
                        goal = true;
          
                }
                if (frisbee.getPosition().x <= -19 && frisbee.getPosition().x >= -21) {
                        if (frisbee.getPosition().y >= -6 && frisbee.getPosition().y <= 6) {
                                scoreJ2.points+=3;
                        }
                        else {
                                scoreJ2.points+=5;
                        }
                        frisbee.setPosition(initialposf);
                        frisbee.setDirection(new Vector2(0,0));
                        goal = true;
                        gameState.gameManager.resetGame();
                }

                return goal;
        }
        
        private void MoveFrisbee(Frisbee frisbee)
        {
                frisbee.frisbeeTransform.transform.position += (Vector3)frisbee.GetDirection().normalized*0.02f*frisbee.Speed;
                if(gameState.getGameManager().collisionWall(frisbee.frisbeeTransform.position,true))
                {
                        frisbee.setDirection(new Vector2(frisbee.GetDirection().x, -frisbee.GetDirection().y));
                }
        }
        
        private bool isCatch(Joueur j, Frisbee f)
        {
                float distance = Vector2.Distance(j.transform.position, f.transform.position);
                Debug.DrawRay(j.transform.position,Vector3.left*2.5f);
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
                frisbee.gameObject.SetActive(true);
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
                float y = joueur.transform.position.y;
                
                float h = Input.GetAxisRaw("Horizontal");
                float x = joueur.transform.position.x;

                if (h == 0 && v == 0)
                {
                        InputPress = false;
                }
                
                if (InputPress || freezeInput) return;
                

                joueur.setDirection(new Vector2(
                     h,
                     v));


                if (!collisionWall(joueur.transform.position + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                { 
                        joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f;        
                }
                
             
                
        }
        
        IEnumerator RandomMove(Joueur joueur, bool f)
        {
                float counter = Random.Range(0.05f,0.3f);
                joueur.setDirection(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)));
                
                
                
                while (counter >0)
                {
                        counter -= 0.02f;
                        
                        
                        if (!collisionWall(joueur.transform.position + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                        { 
                                joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ;
                        }
                     
                        
                        yield return new WaitForFixedUpdate();
                }
                StartCoroutine( RandomMove(gameState.j2,true)); 
        }

        private void RandomMove(Joueur joueur)
        {
                float counter = Random.Range(0.05f,0.3f);
                joueur.setDirection(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)));

                while (counter >0)
                {
                        counter -= 0.02f;
                        
                        if (!collisionWall(joueur.transform.position + (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ))
                        { 
                                joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ;
                        }
                        
                }
                

        }

        
        
        
}
