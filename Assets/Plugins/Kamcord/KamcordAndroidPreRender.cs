using UnityEngine;
using System.Collections;

public class KamcordAndroidPreRender : MonoBehaviour {
#if UNITY_ANDROID
	void OnPreRender() {
		Kamcord.BeginDraw();
	}
#endif
}
