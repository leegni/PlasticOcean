using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{

        public float FloatStrenght;
       public float RandomRotationStrenght;
       private Rigidbody rb;
       private Transform tr;
         void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }

     
     void Update () {
         rb.AddForce(Vector3.down *FloatStrenght);
        tr.Rotate(RandomRotationStrenght,RandomRotationStrenght,RandomRotationStrenght);
     }
}
