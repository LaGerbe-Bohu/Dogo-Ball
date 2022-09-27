using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
        public float MaxSpeed;
        private InputManager inputManager;
        private GameState gameState;
        
        // Utiliser GameState.[qqch] pour récuperer le joueur par exemple pour le frisbee
        // Ca serait bien qu'il est que cette classe qui est le droit de modifier ces objets
        
        // Intégrer ici je pense : Les déplacement de joueur, le lancé de frisbee ainsi que les 
        // le rebou de celui-ci sur le mur. Le catch.
        // 

        private void Start()
        {
                gameState = GameState.Instance;
           
               
        }

        private void Update()
        {
                MovePlayer(gameState.j1);

                if (Input.GetKeyUp(KeyCode.A))
                { 
                        StartCoroutine( RandomMove(gameState.j2,true)); 
                       //RandomMove(gameState.j2);
                }
                
        }

        private void FixedUpdate()
        {
             
               // RandomMove(gameState.j2);
        }
        
        
        private void MovePlayer(Joueur joueur)
        {
                float v = Input.GetAxisRaw("Vertical") ;
                float y = joueur.transform.position.y;
                
                float h = Input.GetAxisRaw("Horizontal");
                float x = joueur.transform.position.x;
                                        
                joueur.setDirection( new Vector2( 
                        h * ( (x <= -20 && h<0) || (x >= -1.5 && h>0) ? 0:1) ,
                        v* ( (y >= 8.66f && v>0) || (y <= -8.20f && v<0)  ? 0:1))
                );
                
                
                

                Debug.Log(  (x <= -20.66f) +"&&"+ (h<0));
                joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f;

             
                
        }
        
        IEnumerator RandomMove(Joueur joueur, bool f)
        {
                float counter = Random.Range(0.1f,0.6f);
                joueur.setDirection(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)));

                while (counter >0)
                {
                        counter -= 0.02f;
                        
                        joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f ;

                        CollapaseJ2(joueur);
                        
                        yield return new WaitForEndOfFrame();
                }
                StartCoroutine( RandomMove(gameState.j2,true)); 
        }

        private void RandomMove(Joueur joueur)
        {
                float counter = Random.Range(0.1f,0.4f);
                joueur.setDirection(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)));

                while (counter >0)
                {
                        counter -= 0.02f;
                        
                        joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * 0.02f;
                        
                }
                

        }



        private void CollapaseJ2(Joueur joueur)
        {
                float x = joueur.transform.position.x;
                float y = joueur.transform.position.y;

                if (x > 20)
                {
                        joueur.transform.position = new Vector3(20,joueur.transform.position.y,0);
                }
                
                if (x < 1)
                {
                        joueur.transform.position = new Vector3(1,joueur.transform.position.y,0);
                }
                
                if (y > 8.66)
                {
                        joueur.transform.position = new Vector3(joueur.transform.position.x,8.66f,0);
                }
                
                if (y < -8.2f)
                {
                        joueur.transform.position = new Vector3(joueur.transform.position.x,-8.2f,0);
                }

                
                
        }
        
        
        
}
