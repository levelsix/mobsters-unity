using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UITexture))]
public class MSTextureAnimaiton : MonoBehaviour {
	UITexture texture;
	public int framerate = 20;
	public bool ignoreTimeScale = true;
	public Texture[] frames;
	float mUpdate = 0f;
	int mIndex = 0;

	void Start()
	{
		texture = GetComponent<UITexture>();
		if (framerate > 0) mUpdate = (ignoreTimeScale ? RealTime.time : Time.time) + 1f / framerate;
	}

	void Update ()
	{
		if (framerate != 0 && frames != null && frames.Length > 0)
		{
			float time = ignoreTimeScale ? RealTime.time : Time.time;
			
			if (mUpdate < time)
			{
				mUpdate = time;
				mIndex = NGUIMath.RepeatIndex(framerate > 0 ? mIndex + 1 : mIndex - 1, frames.Length);
				mUpdate = time + Mathf.Abs(1f / framerate);
				texture.nextTexture = frames[mIndex];
			}
		}
	}
}
