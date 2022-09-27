using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAdd : MonoBehaviour
{
    public Transform freesbee;
    public List<Transform> J1;
    public List<Transform> J2;
    private Score scoreJ1;
    private Score scoreJ2;
    private Vector3 initialposf;
    // Start is called before the first frame update
    void Start()
    {
        scoreJ1 = GameObject.Find("Player 1").GetComponent<Score>();
        scoreJ2 = GameObject.Find("Player 2").GetComponent<Score>();
        initialposf = freesbee.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (freesbee.position.x >= 19 && freesbee.position.x <= 21) {
            if (freesbee.position.y >= -6 && freesbee.position.y <= 6) {
                scoreJ1.points+=3;
            }
            else {
                scoreJ1.points+=5;
            }
            freesbee.position=initialposf;
        }
         if (freesbee.position.x <= -19 && freesbee.position.x >= -21) {
            if (freesbee.position.y >= -6 && freesbee.position.y <= 6) {
                scoreJ2.points+=3;
            }
            else {
                scoreJ2.points+=5;
            }
            freesbee.position=initialposf;
        }
    }
}
