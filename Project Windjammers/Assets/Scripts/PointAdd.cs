using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAdd : MonoBehaviour
{
    private Frisbee frisbee;

    private Score scoreJ1;
    private Score scoreJ2;
    private Vector3 initialposf;

    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState =InterfaceGameState.instance.gameState;
        scoreJ1 = gameState.j1.getScore();
        scoreJ2 = gameState.j2.getScore();
        frisbee = gameState.frisbee;
        initialposf = frisbee.getPosition();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
