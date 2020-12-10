using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class SwimmingController : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;


    [SerializeField] private float swimmingForce;
    [SerializeField] private float resistanceForce;
    [SerializeField] private float deadZone;
    [SerializeField] private float interval;
    [SerializeField] private Transform trackingSpace;


    private float currentWaitTime;
    private new Rigidbody rigidbody;
    private Vector3 currentDirection;

  

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
          currentWaitTime += Time.deltaTime;
        
          Vector3 leftHandDirection = leftHand.GetComponent<SteamVR_Behaviour_Pose>().GetVelocity();
          Vector3 rightHandDirection = rightHand.GetComponent<SteamVR_Behaviour_Pose>().GetVelocity();
          Vector3 localVelocity = leftHandDirection + rightHandDirection;

            localVelocity *= -1f;
            if (localVelocity.sqrMagnitude > deadZone * deadZone)
            {
                AddSwimmingForce(localVelocity);
                currentWaitTime = 0;
            }
            ApplyReststanceForce();



    }

    private void ApplyReststanceForce()
    {
        if (rigidbody.velocity.sqrMagnitude > 0.01f && currentDirection != Vector3.zero)
        {
            rigidbody.AddForce(-rigidbody.velocity * resistanceForce, ForceMode.Acceleration);
        }
        else
        {
            currentDirection = Vector3.zero;
        }
    }

    private void AddSwimmingForce(Vector3 localVelocity)
    {
        Vector3 worldSpaceVelocity = trackingSpace.TransformDirection(localVelocity);
        rigidbody.AddForce(worldSpaceVelocity * swimmingForce, ForceMode.Acceleration);
        currentDirection = worldSpaceVelocity.normalized;
    }
}