using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class byscore : MonoBehaviour
{
    public float score = 100;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (score == 200)
        {
            anim.SetTrigger("t1");   
        }
        else if (score == 500)
        {
            anim.SetTrigger("t2");
        }
        else if (score == 800)
        {
            anim.SetTrigger("t3");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
