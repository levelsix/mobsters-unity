using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UIWidget))]
public class PZRainbow : MonoBehaviour {

	[SerializeField]
	float time = .2f;

	[SerializeField]
	float curr;

	UIWidget widget;

	public void Awake()
	{
		widget = GetComponent<UIWidget>();
	}

	public void Play()
	{
		StartCoroutine(Rainbowify());
	}

	IEnumerator Rainbowify()
	{
		curr = 0;
		while (true)
		{
			curr += Time.deltaTime;


			widget.color = new Color(PingPong(curr), PingPong(curr-time/2), PingPong(curr-time));


			yield return null;
		}
	}
	
	//If we've passed time an even number of times, it's the time past the last time increment
	//Otherwise, it's 1 - the time past the last time increment
	float PingPong(float curr)
	{
		if (curr < 0)
		{
			return 0;
		}
		return ((((int)(curr / time)) % 2 == 0) ? curr % time : 1 - (curr % time)) / time;
	}
}
