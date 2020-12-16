using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{

    public GameObject fishSpawnPoint, plasticSpawnPoint, lastSpawnPoint;
    private int triggerType = 0;
    public GameObject testObj;
    public GameObject spawnPlastic, spawnFish;
    public bool lastScene = false;
    public GameObject lastObj;
 
    void Start()
    {

    }

    

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("STRIGGER")){
        switch (triggerType)
        {
            case 0:
                IntroTrigger();
                triggerType++;
                Destroy(other.gameObject);
                break;
            case 1:
                FirstTrigger();
                triggerType++;
                Destroy(other.gameObject);
                break;
            case 2:
                SecondTrigger();
                triggerType++;
                Destroy(other.gameObject);
                break;
            case 3:
                LastTrigger();
                triggerType++;
                Destroy(other.gameObject);
                break;

        }
        }
    }

    void IntroTrigger()
    {
        GameObject startPoint = GameObject.Find("StartPoint").gameObject;
        Destroy(startPoint);
    }


    void FirstTrigger()
    {
        GameObject tobj = Instantiate<GameObject>(testObj);
        Instantiate<GameObject>(spawnFish, Vector3.zero, Quaternion.identity);
        tobj.transform.position = fishSpawnPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;
        
    }

    void SecondTrigger()
    {
        GameObject tobj = Instantiate<GameObject>(testObj);
        Instantiate<GameObject>(spawnPlastic, Vector3.zero, Quaternion.identity);
        tobj.transform.position = plasticSpawnPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;
    }

    void LastTrigger()
    {
        GameObject tobj = Instantiate<GameObject>(testObj);
        tobj.transform.position = lastSpawnPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;

        lastObj.GetComponent<Transform>().position = transform.position - new Vector3(0, 0, -2);
        lastObj.GetComponent<MeshRenderer>().enabled = true;
        lastObj.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10.0f);
        lastObj.GetComponent<AutoMoving>().enabled = true;
       
  
        lastScene = true;
        GameObject[] plastciObj = GameObject.FindGameObjectsWithTag("PLASTIC");
        for(int i=0; i < plastciObj.Length; i++)
        {
            Destroy(plastciObj[i]);
        }
    }
}
