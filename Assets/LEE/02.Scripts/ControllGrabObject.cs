using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllGrabObject : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;

    private GameObject collidingObject;
    private GameObject objectInHand;

    void Start()
    {
        
    }

    void Update()
    {
        if (grabAction.GetLastStateDown(handType))
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }
        if (grabAction.GetLastStateUp(handType))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    //충돌이 시작되는 순간
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }
    //충돌중일 때
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }
    //충돌이 끝나는 순간
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    //충돌중인 객체로 설정
    private void SetCollidingObject(Collider col)
    {
        if(collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    //객체를 잡음
    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    //FixedJoint => 객체들을 하나로 묶어 고정시켜줌
    //breakForce => 조인트가 제거되도록 하기 위한 필요한 힘의 크기
    //breakTorque => 조인트가 제거되도록 하기 위한 필요한 토크
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    //객체를 놓음
    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            //컨트롤러의 속도
            objectInHand.GetComponent<Rigidbody>().velocity =
                controllerPose.GetVelocity();
            //컨트롤러의 가속도
            objectInHand.GetComponent<Rigidbody>().angularVelocity =
                controllerPose.GetAngularVelocity();
        }
        objectInHand = null;
    }
}
