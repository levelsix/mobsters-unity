using UnityEngine;
using System.Collections;

public class KamcordAndroidCameraAttachment : MonoBehaviour {
#if UNITY_ANDROID && (UNITY_4_3 || UNITY_4_2)
	void Awake()
	{
		// Disable the pre- and postrender camera.
		KamcordImplementationAndroid.SetRenderCameraEnabled("Pre", false);
		KamcordImplementationAndroid.SetRenderCameraEnabled("Post", false);
	}

	void OnDestroy()
	{
		// Re-enable the pre- and postrender camera.
		KamcordImplementationAndroid.SetRenderCameraEnabled("Pre", true);
		KamcordImplementationAndroid.SetRenderCameraEnabled("Post", true);
	}

	void OnPreRender() {
		Kamcord.BeginDraw();
	}

	void OnPostRender() {
		Kamcord.EndDraw();
	}
#endif
}

