using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField]
   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLASTIC"))
        {
        
                    other.transform.parent = transform;
                    other.transform.position = transform.position+new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.2f, 0.6f), 0.6f);
                    other.GetComponent<Rigidbody>().isKinematic = true;
                    other.GetComponent<FloatingObject>().enabled = false;
                     other.GetComponent<PlasticMoving>().enabled = false;
                
            }
        }
    }

