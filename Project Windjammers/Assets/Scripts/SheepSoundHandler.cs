using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SheepSoundHandler : MonoBehaviour
{
    public AudioSource As;

    public float Timer = 3f;

    private float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        counter = Timer;
    }

    // Update is called once per frame
    void Update()
    {
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            As.Play();
            As.pitch = Random.Range(0.85f, 1.25f);
            counter = Timer;
        }
    }
}
