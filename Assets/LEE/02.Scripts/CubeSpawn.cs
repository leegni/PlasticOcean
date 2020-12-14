using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawn : MonoBehaviour
{
public GameObject ObjectToSpawn;   
public float RateOfSpawn = 1;
public int count = 50;
private float nextSpawn = 0;
 
    // Update is called once per frame
    void Start() {
        
            for(int i=0; i<count; i++){
             Vector3 rndPosWithin;
            rndPosWithin = new Vector3(Random.Range(-2f, 3f), Random.Range(-4f, 3f), Random.Range(-2f, 4f));
            rndPosWithin = transform.TransformPoint(rndPosWithin * .5f);
            GameObject fish =  Instantiate(ObjectToSpawn, rndPosWithin, transform.rotation);      
            fish.transform.parent = this.transform;
            
            }
            // Random position within this transform
    }
   
}