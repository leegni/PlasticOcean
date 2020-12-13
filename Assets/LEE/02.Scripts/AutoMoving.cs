using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoving : MonoBehaviour {

public Transform target;

public float speed = 1f;
 void Start() {
    
}

 void Update() {
     
     transform.position += target.position * speed * Time.deltaTime;
}
 
 
 
 }