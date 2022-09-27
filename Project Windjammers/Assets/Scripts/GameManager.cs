using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    private InputManager inputManager;
    private GameState gameState;
    private float counter = 0;

    private bool pressed = false;
    // Utiliser GameState.[qqch] pour récuperer le joueur par exemple pour le frisbee
    // Ca serait bien qu'il est que cette classe qui est le droit de modifier ces objets
        
    // Intégrer ici je pense : Les déplacement de joueur, le lancé de frisbee ainsi que les 
    // le rebou de celui-ci sur le mur. Le catch.
    //      

    private void Start()
    {
        gameState = GameState.Instance;
      
     
    }

    void FixedUpdate()
    {
        MovePlayer(gameState.j1);

        if (pressed)
        {
            pressed = false;
            RandomMove(gameState.j2,true);
        }
    }

    

    private void Update()
    {
        

        if (Input.GetKeyUp(KeyCode.A) )
        {
            pressed = true;
        }
        
    }

    
    private void MovePlayer(Joueur joueur)
    {
        float y = joueur.transform.position.y;
        joueur.setDirection(new Vector2(Input.GetAxisRaw("Horizontal") ,
            Input.GetAxisRaw("Vertical") * (y >=8.66f && Input.GetAxisRaw("Vertical") > 0  ? 0 : 1) *  (y <= -8 && Input.GetAxisRaw("Vertical") < 0 ? 0 : 1)));
        
        joueur.transform.position += (Vector3) joueur.getDirection().normalized * joueur.moveSpeed * Time.deltaTime;
        
    }
/*
    void RandomMove(Joueur joueur)
    {  


        float t = 3;
        joueur.setDirection(new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f)));
        Debug.Log("launch start");
        while (t > 0)
        {
            Debug.Log(t);
            t -= Time.deltaTime;
            joueur.getRigibody2D().velocity = joueur.getDirection().normalized * joueur.moveSpeed;
        }
        
    }
*/
/*
    void RandomMove(GameState gameState, Joueur joueur)
    {
        float counter = Random.Range(0.1f,0.4f);
        joueur.setDirection(
            new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f))
        );

        while (counter > 0)
        {
            
        }


    }
*/
    
     void RandomMove(Joueur joueur,bool f)
    {
        //  float counter = Random.Range(0.1f,0.4f);
        /* joueur.setDirection(
             new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f))
         );
 */
        float counter =0.4f;
        joueur.setDirection(
            new Vector2(1,1)
        );

        while (counter >= 0)
        {
            counter -= Time.deltaTime;

            //joueur.transform.Translate(joueur.getDirection().normalized * joueur.moveSpeed);
            joueur.transform.position = joueur.getDirection().normalized * joueur.moveSpeed * Time.deltaTime;
          
        }
        
        //StartCoroutine(RandomMove(joueur));
    }

    IEnumerator RandomMove(Joueur joueur)
    {
      //  float counter = Random.Range(0.1f,0.4f);
       /* joueur.setDirection(
            new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f))
        );
        */
        float counter =0.3f;
            joueur.setDirection(
            new Vector2(1,1)
        );

        while (counter >= 0)
        {
            counter -= Time.deltaTime;

            joueur.transform.position = joueur.getDirection().normalized * joueur.moveSpeed * Time.deltaTime;
            
            yield return new WaitForEndOfFrame();    
        }
        
        //StartCoroutine(RandomMove(joueur));
    }



}