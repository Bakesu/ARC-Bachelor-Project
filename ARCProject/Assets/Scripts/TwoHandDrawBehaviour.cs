using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using UnityEngine;

public class TwoHandDrawBehaviour : DrawAbstract
{
    private bool m_CanDraw = false;
    //private Vector3 m_Origin;
    internal const double MaxDist = 0.15;
    internal const float ThumbMinCurl = 0.35f;

    // Update is called once per frame
    public override void Update()
    {
        MixedRealityPose rPose;
        MixedRealityPose lPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rPose);
        pinchPose = RightHandPinching(Handedness.Right);
        bool userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        bool leftHandThumbOut = HandPoseUtils.ThumbFingerCurl(Handedness.Left) < ThumbMinCurl;
        bool leftHandDoExist = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out lPose);
        bool handsAreCloseTogether = Vector3.Distance(lPose.Position, rPose.Position) < MaxDist;
        ShowFingerObject(leftHandThumbOut, leftHandDoExist, handsAreCloseTogether);


        //Inititating pinch
        if (!userIsPinching)
        {
            if (m_FirstPinch == false)
            {
                m_LinePoints.Clear();                
                m_CanDraw = false;
                followFingerObject.transform.GetComponent<Renderer>().material.color = Color.white;
            }

            m_FirstPinch = true;
            return;
        }


        //If the user is pinching with their right hand, left hand thumb is out and the hands are in LoS,
        //create new drawing object with the default settings (colour, width etc.)
        if (m_FirstPinch && leftHandThumbOut && leftHandDoExist && handsAreCloseTogether)
        {
            DrawLine();
            followFingerObject.transform.GetComponent<Renderer>().material.color = Color.blue;
            m_CanDraw = true;
        }


        //time delay
        if(GestureUtils.IsPinching(Handedness.Right) && m_CanDraw)
        {
            AttachPoints();
        }
        //m_Timer -= Time.deltaTime;
        //if (m_Timer <= 0 && GestureUtils.IsPinching(Handedness.Right) && m_CanDraw)
        //{
        //    m_LinePoints.Add(pinchPose.Value.Position);
        //    m_DrawLine.positionCount = m_LinePoints.Count;
        //    m_DrawLine.SetPositions(m_LinePoints.ToArray());
        //    m_Timer = timerDelay;

        //}
    }

    private void ShowFingerObject(bool leftHandThumbOut, bool leftHandDoExist, bool handsAreCloseTogether)
    {
        if (leftHandThumbOut && leftHandDoExist && handsAreCloseTogether || m_CanDraw)
        {
            followFingerObject.SetActive(true);
        }
        else
        {
            followFingerObject.SetActive(false);
        }
    }
}
