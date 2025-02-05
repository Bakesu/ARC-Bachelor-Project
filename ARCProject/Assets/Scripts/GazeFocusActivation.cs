using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using System;
using UnityEngine;

public class GazeFocusActivation : DrawAbstract
{
    [SerializeField] private GameObject handSphere;
    private bool userCanDraw;
    private bool lookingAtSphere;
    private bool drawingWithoutFocusAllowed;
    private readonly float drawingAllowedWindow = 3f;
    private float actionTimer = 0f;
    private bool allowDrawingOnDisengage;
    private bool canDrawWhilePinching;

    public bool timerStarted { get; private set; }

    public override void Update()
    {
        GetGazeData();
        Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit gazeRaycast, 10f);
        pinchPose = RightHandPinching(Handedness.Right);
        userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose indexTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose thumbTip);
        if (indexTip == null || thumbTip == null) return;
        //get the midpoint between thumbtip and indextip
        handSphere.transform.position = Vector3.Lerp(indexTip.Position, thumbTip.Position, 0.5f);


        //If raycast does not hit the sphere and user is not pinching, set first look to true
        if (!userIsPinching && gazeRaycast.collider == null)
        {
            m_FirstLook = true;
        }

        //Check if the user is looking at the sphere
        if (gazeRaycast.collider != null)
        {
            lookingAtSphere = gazeRaycast.collider.gameObject == handSphere;
        }
        else
        {
            lookingAtSphere = false;
        }

        if (userCanDraw && focusingOnHand || drawingWithoutFocusAllowed)
        {
            indicatorRenderer.gameObject.SetActive(true);
        }
        else if (!userCanDraw && !userIsPinching || !drawingWithoutFocusAllowed && !userIsPinching | !canDrawWhilePinching)
        {
            indicatorRenderer.gameObject.SetActive(false);
        }

        if (focusingOnHand && lookingAtSphere)
        {
            userCanDraw = true;
            allowDrawingOnDisengage = true;
        }

        //if its the first gaze collide with the sphere, start timer for focus
        if (lookingAtSphere && m_FirstLook)
        {
            //start timer for dwell time
            m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            m_FirstLook = false;
            Debug.Log("first look");

        }


        focusingOnHand = DateTimeOffset.Now.ToUnixTimeMilliseconds() - m_startTime > focusThreshold;

        if (!lookingAtSphere && allowDrawingOnDisengage)
        {
            OnFocusLost();
        }

        //If allowed to draw without focus, draw
        if (drawingWithoutFocusAllowed)
        {
            //if actiontimer is greater than 0 and user is pinching, draw
            if (m_FirstPinch && !userCanDraw && userIsPinching )
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
                Debug.Log("Time has elapsed");
            }
        }

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

        if (m_FirstPinch && userCanDraw)
        {
            DrawLine();
            canDrawWhilePinching = true;
            m_FirstPinch = false;
        }

        //If allowed to draw and pinching, attach points to the line
        if (canDrawWhilePinching && !m_FirstPinch && userIsPinching)
        {
            AttachPoints();
        }
    }

    //On focus lost, allow the user to draw for a small interval of time
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
