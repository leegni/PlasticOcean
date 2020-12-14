using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{

    public GameObject fishSpwanPoint, plasticSpawnPoint, lastSpawnPoint;
    private int triggerType = 0;
    public GameObject testObj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter(Collider other)
    {
        
        switch (triggerType)
        {
            case 0:
                IntroTrigger();
                triggerType++;
                break;
            case 1:
                FirstTrigger();
                triggerType++;
                break;
            case 2:
                SecondTrigger();
                triggerType++;
                break;
            case 3:
                LastTrigger();
                triggerType++;
                break;

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
        tobj.transform.position = fishSpwanPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;
    }

    void SecondTrigger()
    {
        GameObject tobj = Instantiate<GameObject>(testObj);
        tobj.transform.position = plasticSpawnPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;
    }

    void LastTrigger()
    {
        GameObject tobj = Instantiate<GameObject>(testObj);
        tobj.transform.position = lastSpawnPoint.transform.position;
        tobj.transform.rotation = Quaternion.identity;
    }
}
