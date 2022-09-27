using System;
using UnityEngine;
using Random = System.Random;

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
                joueur.transform.position += (Vector3)joueur.getDirection().normalized * joueur.moveSpeed * Time.deltaTime;

             
                
        }

        private void RandomMove(Joueur joueur)
        {
                
        }

        
        
        
}
