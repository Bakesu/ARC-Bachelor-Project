using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements.Experimental;

public class FingerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject followFingerObject;
    // Update is called once per frame

    private void Start()
    {
        followFingerObject.SetActive(false);
    }

    void Update()
    {
        var realityPose = FindIndexFinger(Handedness.Right);
        if (!realityPose.HasValue) return; //exception handling
        var indexTipPos = realityPose.Value.Position;
        
        followFingerObject.transform.position = indexTipPos;
    }

    public MixedRealityPose? FindIndexFinger(Handedness handedness)
    {
        MixedRealityPose pose;
        bool indexTip = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handedness, out pose);
        
        if (!indexTip) {
            return null;
        }

        return pose;
    }
}
