using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

interface IScriptInterface
{
    MixedRealityPose? HandlePinch(Handedness handedness);

    void DrawLine();
    
    
}
