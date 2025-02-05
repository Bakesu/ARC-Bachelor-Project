using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmGazeBehaviour : DrawAbstract
{
    [SerializeField] private GameObject handSphere;
    private bool userCanDraw;
    private bool lookingAtSphere;
    private bool canDrawWhilePinching;

    public override void Update()
    {
        GetGazeData();
        Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit gazeRaycast, 10f);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose indexTipPose);
        pinchPose = RightHandPinching(Handedness.Right);
        userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        if (indexTipPose == null) return;

        if (gazeRaycast.collider != null)
        {
            lookingAtSphere = gazeRaycast.collider.gameObject == handSphere;

        }
        else
        {
            lookingAtSphere = false;
        }
                
        //If raycast does not hit the sphere and user is not pinching, set first look to true
        if (gazeRaycast.collider == null)
        {
            m_FirstLook = true;
            m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        //If we are looking at the hand, start the timer
        if (lookingAtSphere && m_FirstLook)
        {
            m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            m_FirstLook = false;
        }

        focusingOnHand = DateTimeOffset.Now.ToUnixTimeMilliseconds() - m_startTime > focusThreshold;

        if (focusingOnHand && lookingAtSphere)
        {  
            indicatorRenderer.gameObject.SetActive(true);            
            userCanDraw = true;

            if (m_FirstPinch && userIsPinching)
            {
                DrawLine();
                canDrawWhilePinching = true;
            }
        }
        else
        {
            userCanDraw = false;
        }

        //if we can draw , show the indicator, else hide it
        if (userCanDraw || canDrawWhilePinching && !m_FirstPinch)
        {
            indicatorRenderer.gameObject.SetActive(true);
        }
        else if (!userCanDraw && !userIsPinching)
        {
            indicatorRenderer.gameObject.SetActive(false);
        }

        //Reset first pinch and manage colour of indicator
        if (!userIsPinching)
        {
            if (m_FirstPinch == false)
            {
                m_LinePoints.Clear();
                indicatorRenderer.material.color = Color.white;
                canDrawWhilePinching = false;
            }

            m_FirstPinch = true;
            return;
        }
        else if (userCanDraw && userIsPinching)
        {
            indicatorRenderer.material.color = Color.blue;
        }


        //Makes sure that the line is drawn every frame
        if (canDrawWhilePinching && userIsPinching)
        {
            AttachPoints();
        }


    }

}
