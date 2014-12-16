using UnityEngine;
using System.Collections;

public class MSFrameCounter : MonoBehaviour {

	float updateInterval = .5f;

	private float accum = 0; //FPS accumulated over the interval
	private int frames = 0; //Frames over the interval
	private float timeLeft = 0; //Time left in the interval

	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	// Update is called once per frame
	void Update () 
	{
		timeLeft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;

		if (timeLeft <= 0.0)
		{
			label.text = (accum/frames).ToString("f2");
			timeLeft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}
	}
}
