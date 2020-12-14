using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
     void Start()
    {
        points = GetComponentsInChildren<Transform>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLASTIC"))
        {
            for(int i=1; i<points.Length; i++)
            {
                if (points[i].childCount == 0)
                {
                    other.transform.parent = points[i].transform;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                    other.GetComponent<FloatingObject>().enabled = false;
                    
                }
                else return;
            }
        }
    }

}
