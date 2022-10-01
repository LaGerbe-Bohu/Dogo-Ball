using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{


    
    // Start is called before the first frame update
    void Start()
    {
       GameObject[] go =  GameObject.FindGameObjectsWithTag("Music");
       GameObject[] s =  GameObject.FindGameObjectsWithTag("SoundTransition");

       for (int i = 0; i < s.Length; i++)
       {
           Destroy(s[i].gameObject);
       }
       
       if (go.Length > 1)
       {
           Destroy(this.gameObject);

       }
       
       
       Debug.Log("tu ne pas pas en fait si ?");
       DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
