using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Party : MonoBehaviour
{
    public TMP_Text gameTimetxt;
    public TMP_Text player1score;
    public TMP_Text player2score;
    public float roundTime;
    private float gameTime;
    // Start is called before the first frame update
    void Start()
    {
        gameTime = roundTime;
    }

    // Update is called once per frame
    void Update()
    {
       if (gameTime > 0) {
            gameTime -= Time.deltaTime;
        }
        else {
            gameTime = 0;
        }
        gameTimetxt.text = gameTime.ToString("F2");
    }
}
