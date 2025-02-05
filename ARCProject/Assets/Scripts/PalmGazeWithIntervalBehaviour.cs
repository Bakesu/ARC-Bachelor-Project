using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmGazeWithIntervalBehaviour : DrawAbstract
{
    [SerializeField] private GameObject handSphere;
    private bool userCanDraw;
    private bool lookingAtSphere;
    private bool drawingWithoutFocusAllowed;
    private float actionTimer;
    private bool allowDrawingOnDisengage;
    private bool canDrawWhilePinching;
    private readonly float drawingAllowedWindow = 3f;

    public override void Update()
    {
        GetGazeData();
        Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit gazeRaycast, 10f);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose palmPose);
        pinchPose = RightHandPinching(Handedness.Right);
        userIsPinching = GestureUtils.IsPinching(Handedness.Right);

        if (palmPose == null) return;

        if (gazeRaycast.collider != null)
        {
            lookingAtSphere = gazeRaycast.collider.gameObject == handSphere;

        }
        else
        {
            lookingAtSphere = false;
        }

        //If we are not looking at the hand, reset the timer
        if (!lookingAtSphere && allowDrawingOnDisengage)
        {
            OnFocusLost();
        }

        //if we can draw , show the indicator, else hide it
        if (userCanDraw || drawingWithoutFocusAllowed)
        {
            indicatorRenderer.gameObject.SetActive(true);
        }
        else if (!userCanDraw && !userIsPinching || !drawingWithoutFocusAllowed && !userIsPinching || !canDrawWhilePinching )
        {
            indicatorRenderer.gameObject.SetActive(false);
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
            userCanDraw = true;
            allowDrawingOnDisengage = true;
        }


        //If allowed to draw without focus, draw
        if (drawingWithoutFocusAllowed)
        {
            //if actiontimer is greater than 0 and user is pinching, draw
            if (m_FirstPinch && !userCanDraw && userIsPinching)
            {
                Debug.Log("drawing new line without focus");
                DrawLine();
                m_FirstPinch = false;
                canDrawWhilePinching = true;
                indicatorRenderer.material.color = Color.blue;

            }
            //decrease timer
            if (actionTimer > 0)
            {
                actionTimer -= Time.deltaTime;
            }
            // Time window has elapsed
            else
            {
                drawingWithoutFocusAllowed = false;
                indicatorRenderer.material.color = Color.white;
                followFingerObject.SetActive(false);
                Debug.Log("Time has elapsed");
            }
        }

        //Reset first pinch and manage colour of indicator
        if (!userIsPinching)
        {
            if (m_FirstPinch == false)
            {
                canDrawWhilePinching = false;
                m_LinePoints.Clear();
                indicatorRenderer.material.color = Color.white;
            }

            m_FirstPinch = true;
            return;
        }
        else if (userCanDraw && userIsPinching || drawingWithoutFocusAllowed && userIsPinching)
        {
            indicatorRenderer.material.color = Color.blue;
        }

        //if things above work out, handle drawing
        if (m_FirstPinch && userCanDraw)
        {
            DrawLine();
            canDrawWhilePinching = true;
            m_FirstPinch = false;
        }

        //Makes sure that the line is drawn every frame
        if (canDrawWhilePinching && !m_FirstPinch && userIsPinching)
        {
            AttachPoints();
        }


    }
    public void OnFocusLost()
    {
        //create timer
        drawingWithoutFocusAllowed = true;
        userCanDraw = false;
        allowDrawingOnDisengage = false;
        actionTimer = drawingAllowedWindow;
        m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Debug.Log("focus lost");

    }

}
