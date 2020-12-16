using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public GameObject fadeObj;

    void Update()
    {
        Color col = fadeObj.GetComponent<Material>().color;
        for(float i=0.0f; i <= 1.0f; i++)
        {
            col.a += i;
            fadeObj.GetComponent<Material>().color = col;
        }
        
        
    }
}
