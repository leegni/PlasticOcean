using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticMoving : MonoBehaviour
{
    public GameObject player;
    private Transform pTr;
    private float dis = 20.0f;
    public float speed = 0.03f;

    void Start()
    {
        pTr = player.GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pp = player.transform.position;
        Vector3 tp = transform.position;

        if (pp.z - tp.z <= dis)
        {
            tp += pp * speed * Time.deltaTime;
        }
    }
}
