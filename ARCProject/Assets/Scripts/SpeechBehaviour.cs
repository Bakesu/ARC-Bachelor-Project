using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using MRTKExtensions.Gesture;
using UnityEngine;
using UnityEngine.EventSystems;


public class SpeechBehaviour : MonoBehaviour, IMixedRealitySpeechHandler
{
    [SerializeField] private GameObject drawBehaviour;
    [SerializeField] private GameObject palmGazerBehaviour;
    [SerializeField] private GameObject handSphere;
    [SerializeField] private GameObject ToggleButton;
    [SerializeField] private GameObject EyeRayRendererObject;
    private bool m_RightIsPinching;
    private bool m_LeftIsPinching;
    void Start()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
        ToggleButton.SetActive(false);
        EyeRayRendererObject.SetActive(false);
    }

    private void Update()
    {
        m_RightIsPinching = GestureUtils.IsPinching(Handedness.Right);
        m_LeftIsPinching = GestureUtils.IsPinching(Handedness.Left);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch (eventData.Command.KeyCode)
        {
            case KeyCode.H: //two hand
                Debug.Log("two-handed draw behaviour enabled");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = true;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                ToggleButton.SetActive(false);
                break;

            case KeyCode.F: //finger
                Debug.Log("gaze focus behaviour enabled");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = true;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                ToggleButton.SetActive(false);
                break;


            case KeyCode.D: //pinch only
                Debug.Log("Pinch behaviour enabled");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = true;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                ToggleButton.SetActive(false);
                
                break;
            
            case KeyCode.B: //pinch button
                Debug.Log("Pinch behaviour enabled");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = true;
                ToggleButton.SetActive(true);
                break;

            case KeyCode.Alpha1: //gaze
                Debug.Log("Gaze behaviour enabled");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = true;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                ToggleButton.SetActive(false);
                break;

            case KeyCode.Alpha2: // gaze with interval
                Debug.Log("Gaze focus with interval activated");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = true;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                break;

            case KeyCode.Alpha3: //Palm gazer
                Debug.Log("Palm gazer without interval activated");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeBehaviour>().enabled = true;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                break;
                    
            case KeyCode.Alpha4: //Palm gazer with interval
                Debug.Log("Palm gazer with interval activated");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = true;
                drawBehaviour.GetComponent<PalmGazeBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                //palmGazerBehaviour.SetActive(true);
                break;


            case KeyCode.Alpha0: // deactivate all
                Debug.Log("No active technique");
                drawBehaviour.GetComponent<ButtonPinchBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PinchOnlyBehaviour>().enabled = false;
                drawBehaviour.GetComponent<TwoHandDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeDrawBehaviour>().enabled = false;
                drawBehaviour.GetComponent<GazeFocusActivation>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeWithIntervalBehaviour>().enabled = false;
                drawBehaviour.GetComponent<PalmGazeBehaviour>().enabled = false;
                drawBehaviour.GetComponent<HandConstraintPalmUp>().enabled = false;
                //palmGazerBehaviour.SetActive(true);
                break;


            case KeyCode.R: //ray
                Debug.Log("Gaze Ray toggle");
                //if false set active
                if (EyeRayRendererObject.activeSelf)
                { 
                    EyeRayRendererObject.SetActive(false);
                } else {
                  EyeRayRendererObject.SetActive(true);
                }
                break;
            case KeyCode.C: //clear drawings
                if (m_RightIsPinching || m_LeftIsPinching)
                {
                    return;
                    
                }
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("DrawLine");
                    foreach (GameObject line in taggedObjects)
                    {
                        Destroy(line);
                    }
                break;
        }

    }

}