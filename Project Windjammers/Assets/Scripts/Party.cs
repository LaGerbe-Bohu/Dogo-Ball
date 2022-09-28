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
    public float roundTime;
    private Score j1;
    private Score j2;
    private float countdown;
    private float gameTime;
    private bool game;
    private GameObject g;

    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Instance;
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
            gameTimetxt.text = gameTime.ToString("F2");
            player1score.text = j1.setCount.ToString()+" / "+j1.points.ToString();
            player2score.text = j2.setCount.ToString()+" / "+j2.points.ToString();
        }
        else {
            if (countdown > 0) {
                countdown -= Time.deltaTime*2f;
                if (countdown >= 1) {
                    countdowntxt.text = ((int)countdown).ToString();
                }
                else {
                    countdowntxt.text = "GO!";
                }
            }
            else {
                countdowntxt.enabled = false;
                player1score.enabled=true;
                player2score.enabled=true;
                gameTimetxt.enabled=true;
                game = true;
                g.SetActive(true);
            }
        }
    }
}
