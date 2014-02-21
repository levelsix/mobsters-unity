using UnityEngine;
using System.Collections;

public class CBKSnapshot : MonoBehaviour {

	[SerializeField]
	RenderTexture rendTex;

	[SerializeField]
	Camera cam;

	[SerializeField]
	CBKMaterialFader fader;

	[ContextMenu ("Snap")]
	public void Snap()
	{
		float saveSize = cam.orthographicSize;

		//Render the texture
		cam.targetTexture = rendTex;
		cam.orthographicSize *= ((float)Screen.width / Screen.height);
		cam.Render();

		//Start the fade out
		fader.Fade();


		cam.targetTexture = null;
		cam.orthographicSize = saveSize;
	}
}
