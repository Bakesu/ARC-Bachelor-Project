using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using UnityEngine;


public class GazeDrawBehaviour : DrawAbstract
{
    [SerializeField] GameObject handSphere;
    private bool userCanDraw;
    private Vector3 startHandPosition;
    private bool lookingAtSphere;
    private readonly int threshold = 600;
    private const double MaxHandDist = 0.07;

    // Update is called once per frame
    public override void Update()
    {
        GetGazeData();
        Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit gazeRaycast, 10f);
        pinchPose = RightHandPinching(Handedness.Right);
        userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose thumbTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose indexTip);
        if (indexTip == null || thumbTip == null) return;
        handSphere.transform.position = Vector3.Lerp(indexTip.Position, thumbTip.Position, 0.5f);
        //bool handIsStill = Vector3.Distance(startHandPosition, thumbTip.Position) < MaxHandDist;

        // Du er ved at implementere kig på objektet for at tegne i denne her teknik

        if (!userIsPinching && gazeRaycast.collider == null)
        {
            m_FirstLook = true;
        }

        if (focusingOnHand)
        {
            indicatorRenderer.gameObject.SetActive(true);
        }
        else if (!userCanDraw && !userIsPinching)
        {
            indicatorRenderer.gameObject.SetActive(false);
        }
        //Check if the user is looking at the sphere
        if (gazeRaycast.collider != null)
        {
            lookingAtSphere = gazeRaycast.collider.gameObject == handSphere;
        }
        else
        {
            lookingAtSphere = false;
            m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        //if its the first gaze collide with the sphere, start timer for focus
        if (lookingAtSphere && m_FirstLook)
        {
            //start timer for dwell time
            m_startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            m_FirstLook = false;
        }

        focusingOnHand = DateTimeOffset.Now.ToUnixTimeMilliseconds() - m_startTime > threshold;

        if (lookingAtSphere && focusingOnHand && userIsPinching)
        {
            userCanDraw = true;
            if (m_FirstPinch)
            {
                DrawLine();
                indicatorRenderer.material.color = Color.blue;

            }

        }

        //Inititating pinch
        if (!userIsPinching)
        {
            if (m_FirstPinch == false)
            {
                m_LinePoints.Clear();
                userCanDraw = false;
                indicatorRenderer.material.color = Color.white;
            }

            m_FirstPinch = true;
            return;
        }


        //time delay
        if (userIsPinching && userCanDraw)
        {
            m_Timer -= Time.deltaTime;
            m_LinePoints.Add(pinchPose.Value.Position);
            m_DrawLine.positionCount = m_LinePoints.Count;
            m_DrawLine.SetPositions(m_LinePoints.ToArray());
            m_Timer = timerDelay;

        }

    }

    private bool rightHandInLoS(MixedRealityPose pose)
    {
        gazeOrigin = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        var handPos = pose.Position;
        var toHandVector = (handPos - gazeOrigin).normalized;
        gazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        var handToEyeAngle = Vector3.Angle(toHandVector, gazeDirection);
        var maxAngle = 8;

        //Debug.Log(isLookingCloseToHand);
        //Debug.Log("udregnet vinkel" + handToEyeAngle);
        if (handToEyeAngle > maxAngle)
        {
            return false;
        }

        return true;
    }
}