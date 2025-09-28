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
         if(score == 200){
            anim.SetTrigger("Open1");
        }else if(score == 500){
            anim.SetBool("open2",true);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
