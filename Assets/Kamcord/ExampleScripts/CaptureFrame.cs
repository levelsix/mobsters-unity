using UnityEngine;
using System.Collections;

public class CaptureFrame : MonoBehaviour {
    // Attach this script onto your HUD camera to enable HUD-less recording.
#if UNITY_IPHONE || UNITY_ANDROID
    void OnPreRender()
    {
        Kamcord.CaptureFrame();    
    }
#endif
}
