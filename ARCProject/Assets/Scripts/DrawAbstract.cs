using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DrawAbstract : MonoBehaviour
{
    [SerializeField] internal GameObject followFingerObject;
    public float lineWidth;
    public float timerDelay;
    internal GameObject m_NewLine;
    internal LineRenderer m_DrawLine;
    internal bool m_FirstPinch;
    internal bool userIsPinching;
    internal MixedRealityPose? pinchPose;
    internal List<Vector3> m_LinePoints;
    internal float m_drawingFloat;
    internal float m_Timer;
    internal MixedRealityPose rPose;
    internal Vector3 gazeDirection;
    internal Vector3 gazeOrigin;
    internal Renderer indicatorRenderer;
    internal bool m_FirstLook = false;
    internal long m_startTime;
    internal bool focusingOnHand = false;
    internal readonly int focusThreshold = 600;

    

    // Start is called before the first frame update
    public virtual void Start()
    {
        Debug.Log("start");
        m_LinePoints = new List<Vector3>();
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
        indicatorRenderer = followFingerObject.GetComponent<Renderer>();
        if (!CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled)
        {
            Debug.Log("eye tracking is NOT enabled");
        }
    }

    public virtual void Update()
    {
        pinchPose = RightHandPinching(Handedness.Right);
        if (!pinchPose.HasValue) return; //exception handling
        var userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        followFingerObject.transform.GetComponent<Renderer>().material.color = userIsPinching ? Color.blue : Color.white;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rPose);
        //This has to be here, otherwise a new line is created every frame - having issues moving it to a designated method
        if (!userIsPinching)
        {
            if (m_FirstPinch == false)
            {
                m_LinePoints.Clear();
            }

            m_FirstPinch = true;
            return;
        }
        //If pinch initiated, create new drawing object with the default settings (colour, width etc.)
        if (m_FirstPinch)
        {
            DrawLine();
        }
        //Makes sure that the line is drawn every frame
        AttachPoints();
    }

    internal void GazeUpdate()
    {
        pinchPose = RightHandPinching(Handedness.Right);
        userIsPinching = GestureUtils.IsPinching(Handedness.Right);
        MixedRealityPose rPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rPose);

    }

    internal void AttachPoints()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
        {
            m_LinePoints.Add(pinchPose.Value.Position);
            m_DrawLine.positionCount = m_LinePoints.Count;
            m_DrawLine.SetPositions(m_LinePoints.ToArray());

            m_Timer = timerDelay;
        }
    }

    internal void DrawLine()
    {
        //Create new line object
        m_NewLine = new GameObject();
        m_NewLine.tag = "DrawLine";
        //Turn the line object into a line renderer
        m_DrawLine = m_NewLine.AddComponent<LineRenderer>();
        m_DrawLine.material = new Material(Shader.Find("Sprites/Default"));
        m_DrawLine.startColor = Color.blue;
        m_DrawLine.endColor = Color.blue;
        m_DrawLine.startWidth = lineWidth;
        m_DrawLine.endWidth = lineWidth;
        //Reset the first pinch bool to allow new drawing
        m_FirstPinch = false;
    }
    internal MixedRealityPose? RightHandPinching(Handedness handedness)
    {
        MixedRealityPose thumbPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbPose);
        MixedRealityPose indexPose;
        // Get the position of the user's IndexTip (the user's right index finger tip joint)
        bool foundJoint = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handedness, out indexPose);

        if (!foundJoint)
        {
            return null;
        }

        return indexPose;
    }

    internal void GetGazeData()
    {
        gazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        gazeOrigin = CoreServices.InputSystem.GazeProvider.GazeOrigin;

    }
}
