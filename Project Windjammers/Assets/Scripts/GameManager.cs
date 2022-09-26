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

        private void FixedUpdate()
        {
                MovePlayer(gameState.j1);
                RandomMove(gameState.j2);
        }

        private void MovePlayer(Joueur joueur)
        {
                joueur.setDirection(new Vector2(Input.GetAxisRaw("Horizontal"),
                        Input.GetAxisRaw("Vertical")));

                joueur.getRigibody2D().velocity = joueur.getDirection().normalized * joueur.moveSpeed;
        }

        private void RandomMove(Joueur joueur)
        {
                
        }

        
        
        
}
