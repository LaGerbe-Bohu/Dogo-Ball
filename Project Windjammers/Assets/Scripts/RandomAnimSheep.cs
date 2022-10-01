using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class RandomAnimSheep : MonoBehaviour
{

    private Animator anim;
    

    private float count;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        count = Random.Range(0.1f, 1.5f);
        anim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        count -= Time.deltaTime;

        if (count <= 0)
        {
            anim.enabled = true;
            Destroy(this);
        }
        
    }
}
