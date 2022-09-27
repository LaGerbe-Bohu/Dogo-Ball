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
    public Score j1;
    public Score j2;
    private float countdown;
    private float gameTime;
    private bool game;
    private GameObject g;
    // Start is called before the first frame update
    void Start()
    {
        gameTime = roundTime;
        game = false;
        countdown = 4;
        g = GameObject.Find("Gameplay");
        g.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (game) {
             if (gameTime > 0) {
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
                countdown -= Time.deltaTime;
                countdowntxt.text = ((int)countdown).ToString();
            }
            else {
                countdowntxt.enabled = false;
                game = true;
                g.SetActive(true);
            }
        }
    }
}
