using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playtestanim : MonoBehaviour
{
    public float score = 100;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (score == 200)
        {
            anim.SetTrigger("1");   
        }
        else if (score == 500)
        {
            anim.SetTrigger("2");
        }
        else if (score == 800)
        {
            anim.SetTrigger("3");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
