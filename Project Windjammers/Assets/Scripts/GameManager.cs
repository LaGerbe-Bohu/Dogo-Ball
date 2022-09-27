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
        
        // Utiliser GameState.[qqch] pour récuperer le joueur par exemple pour le frisbee
        // Ca serait bien qu'il est que cette classe qui est le droit de modifier ces objets
        
        // Intégrer ici je pense : Les déplacement de joueur, le lancé de frisbee ainsi que les 
        // le rebou de celui-ci sur le mur. Le catch.
        // 

        private void Start()
        {
                gameState = GameState.Instance;
                co =  StartCoroutine( RandomMove(gameState.j2,true)); 
               
        }

        private void Update()
        {
                
                if (Input.GetKeyDown(KeyCode.A))
                {
                        SceneManager.LoadScene(0);
                }
                

                if (Input.GetKeyUp(KeyCode.A))
                { 
                        
                       RandomMove(gameState.j2);
                }


                if (gameState.frisbee.getIsCatched())
                {
                        if (gameState.frisbee.getJoueur() == gameState.j1)
                        {
                                if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.RightArrow) )
                                {
                                        throwFrisbee(gameState.frisbee,new Vector2(1,0));
                                        InputPress = true;
                                }
                        
                                if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow) )
                                {
                                        throwFrisbee(gameState.frisbee,new Vector2(1,-1));
                                        InputPress = true;
                                }
                        
                                if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.UpArrow) )
                                {
                                        throwFrisbee(gameState.frisbee,new Vector2(1,1));
                                        InputPress = true;
                                }   
                        }
                        else
                        {
                                throwFrisbee(gameState.frisbee,new Vector2(-1,Random.Range(-1,2))); 
                                co =  StartCoroutine( RandomMove(gameState.j2,true));
                        }
                      
                }
                
                if (isCatch(gameState.j1, gameState.frisbee) && !gameState.frisbee.getIsCatched() )
                {
                        gameState.frisbee.setDirection(new Vector2(0, 0));
                        gameState.frisbee.gameObject.SetActive(false);
                        gameState.frisbee.setCatched(true);
                        gameState.frisbee.setJoueur(gameState.j1);
                        movePlayer = false;

                }
                else if (!isCatch(gameState.j1, gameState.frisbee))
                {
                        gameState.frisbee.setCatched(false);
                      
                        movePlayer = true;
                }
                
                if (isCatch(gameState.j2, gameState.frisbee)  && !gameState.frisbee.getIsCatched() )
                {
                        gameState.frisbee.setJoueur(gameState.j2);
                        gameState.frisbee.gameObject.SetActive(false);
                        gameState.frisbee.setCatched(true);
                        StopAllCoroutines();
                        

                }
                else if (!isCatch(gameState.j1, gameState.frisbee))
                { 
                        
                }

              
        }

        private void FixedUpdate()
        {

                if (movePlayer)
                {
                        MovePlayer(gameState.j1);
                }
                
                MoveFrisbee(gameState.frisbee);
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
                if(distance < 2)
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
                frisbee.setDirection( new Vector2( frisbee.GetDirection().x,frisbee.GetDirection().y));
                
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
                
                if (InputPress) return;
                

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
                     
                        
                        yield return new WaitForEndOfFrame();
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
