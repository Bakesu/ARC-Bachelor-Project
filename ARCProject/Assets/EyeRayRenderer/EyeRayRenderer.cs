using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeRayRenderer : MonoBehaviour
{
    private IMixedRealityEyeGazeProvider eyeGazeProvider; 
    private LineRenderer lineRenderer;

    void OnEnable()
    {
        // Ensure that the Gaze Pointer is always on as it might otherwise
        // turn off when the hands appear...
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOn);
    }

    void Start()
    {
        // Get the EyeGazeProvider instance.
        eyeGazeProvider = CoreServices.InputSystem.EyeGazeProvider;
        // Get the LineRenderer Component.
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    void Update()
    {
        // Set the ray/line origin to 10cm below the the user's eyes so it
        // becomes visible.
        lineRenderer.SetPosition(0, eyeGazeProvider.GazeOrigin - new Vector3(0f, 0.1f, 0f));
        // Use the GazeCursor.Position as end point to not point too far.
        lineRenderer.SetPosition(1, eyeGazeProvider.GazeCursor.Position);
    }
}
