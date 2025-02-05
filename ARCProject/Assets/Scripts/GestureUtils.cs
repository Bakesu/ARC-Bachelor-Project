using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
 
// https://localjoost.github.io/Basic-hand-gesture-recognition-with-MRTK-on-HoloLens-2/
namespace MRTKExtensions.Gesture
{
    public static class GestureUtils
    {
        private const float PinchThreshold = 0.7f;
        private const float GrabThreshold = 0.4f;
          
        public static bool IsPinching(Handedness handedness)
        {
            if (HandJointUtils.FindHand(handedness) == null)
                return false;
 
            return (HandPoseUtils.CalculateIndexPinch(handedness) > PinchThreshold);
        }
 
        public static bool IsGrabbing(Handedness handedness)
        {
            if (HandJointUtils.FindHand(handedness) == null)
                return false;
 
            return !IsPinching(handedness) &&
                   HandPoseUtils.MiddleFingerCurl(handedness) > GrabThreshold &&
                   HandPoseUtils.RingFingerCurl(handedness) > GrabThreshold &&
                   HandPoseUtils.PinkyFingerCurl(handedness) > GrabThreshold &&
                   HandPoseUtils.ThumbFingerCurl(handedness) > GrabThreshold;
        }
    }
}