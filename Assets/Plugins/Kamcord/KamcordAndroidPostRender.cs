using UnityEngine;
using System.Collections;

public class KamcordAndroidPostRender : MonoBehaviour {
#if UNITY_ANDROID
	void OnPostRender() {
		Kamcord.EndDraw();
	}
#endif
}
