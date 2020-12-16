using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPlasticAction : MonoBehaviour
{
    public GameObject playerObj;
    private Rigidbody rb;
    private MeshRenderer m;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        m = gameObject.GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {

            m.enabled = true;
            rb.AddForce(Vector3.forward * 10.0f);
            gameObject.GetComponent<AutoMoving>().enabled = true;
        }
    }
}