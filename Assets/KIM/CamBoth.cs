using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBoth : MonoBehaviour
{
    Camera targ;
    // Start is called before the first frame update
    void Start()
    {
        targ = GetComponent<Camera>();

        StartCoroutine(CamBothSetting());
    }

    IEnumerator CamBothSetting()
    {
        
        targ.stereoTargetEye = StereoTargetEyeMask.Left;
        yield return new WaitForSeconds(0.5f);
        targ.stereoTargetEye = StereoTargetEyeMask.Both;
        yield return null;
    }

}
