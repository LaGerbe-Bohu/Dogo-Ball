using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Party : MonoBehaviour
{
    public TMP_Text gameTimetxt;
    public TMP_Text player1score;
    public TMP_Text player2score;
    public TMP_Text countdowntxt;
    public GameObject Button;
    public float roundTime;
    private Score j1;
    private Score j2;
    private float countdown;
    private float gameTime;
    private bool game;
    private bool endgame;
    private string victoryphrase;
    private GameObject g;
    private GameObject button;
    
    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        button = Button;
        button.SetActive(false);
        
        gameState = InterfaceGameState.instance.gameState;
        j1 = gameState.j1.getScore();
        j2 = gameState.j2.getScore();
        gameTime = roundTime;
        game = false;
        countdown = 4;
        g = GameObject.Find("Gameplay");
        g.SetActive(false);
        player1score.enabled=false;
        player2score.enabled=false;
        gameTimetxt.enabled=false;
        endgame = false;
    
        if (gameState.j1.getScore().setCount >= 3)
        {
            victoryphrase = "Vous avez gagne avec un score de : " + gameState.j1.getScore().setCount + " a " + gameState.j2.getScore().setCount;
            endgame = true;
        }
        if (gameState.j2.getScore().setCount >= 3)
        {
            victoryphrase = "Vous avez perdu avec un score de : " + gameState.j2.getScore().setCount + "a " + gameState.j1.getScore().setCount;
            endgame = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        if (game) {
             if (gameTime > 0)
             {
                 gameTime -= Time.deltaTime;
             }
            else {
                gameTime = 0;
            }
            gameTimetxt.text =((int)(gameState.timer)).ToString();
            player1score.text = j1.setCount.ToString()+" / "+j1.points.ToString();
            player2score.text = j2.setCount.ToString()+" / "+j2.points.ToString();
        }
        else {
            if (!endgame)
            {
                if (countdown > 0)
                {
                    countdown -= Time.deltaTime;

                  
                        if (countdown >= 1)
                        {
                            if (InterfaceGameState.J1set + InterfaceGameState.J2set > 0)
                            {    
                                countdowntxt.text = InterfaceGameState.J1set + " -" + InterfaceGameState.J2set;
                            }
                            else
                            {
                                countdowntxt.text = ((int)countdown).ToString();
                            }
                        }
                        else
                        {
                            countdowntxt.text = "GO!";
                        }
                    
                    
                  
                }
                else
                {
                    countdowntxt.enabled = false;
                    player1score.enabled = true;
                    player2score.enabled = true;
                    gameTimetxt.enabled = true;
                    game = true;
                    g.SetActive(true);
                }
            }
            else
            {
                countdowntxt.text = victoryphrase;
                countdowntxt.fontSize = 10;
                button.SetActive(true);
            }
            
        }
    }
}
