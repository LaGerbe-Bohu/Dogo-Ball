using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator j1;
    public SpriteRenderer sprite1;
    public Animator j2;
    public SpriteRenderer sprite2;

    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = InterfaceGameState.instance.gameState;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Mathf.Abs((gameState.j1.getDirection().x + gameState.j1.getDirection().y)) > 0)
        {
            j1.SetBool("move",true);
        }
        else
        {
            j1.SetBool("move",false);
        }
        
        
        if ( Mathf.Abs((gameState.j2.getDirection().x + gameState.j2.getDirection().y)) > 0)
        {
            j2.SetBool("move",true);
        }
        else
        {
            j2.SetBool("move",false);
        }


        if (gameState.j1.getDirection().x < 0)
        {
            sprite1.flipX = true;
        }
        else
        {
            sprite1.flipX = false;
        } 
        
        if (gameState.j2.getDirection().x < 0)
        {
            sprite2.flipX = false;
        }
        else
        {
            sprite2.flipX = true;
        }
        
    }
}
