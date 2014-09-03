using UnityEngine;
using System.Collections;

public class MSCameraShake : MonoBehaviour {

	[SerializeField] float shakeAmount;

	[SerializeField] float shakeTime;

	[ContextMenu ("Shake")]
	public void Shake()
	{
		StartCoroutine(DoTheShake());
	}

	public Coroutine RunShake()
	{
		return StartCoroutine(DoTheShake());
	}

	IEnumerator DoTheShake()
	{
		if (shakeTime <= 0)
		{
			yield break;
		}

		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / shakeTime;
			transform.localPosition = new Vector3((1-t) * shakeAmount * Random.value,
			                                      (1-t) * shakeAmount * Random.value);
			yield return null;
		}

		transform.localPosition = Vector3.zero;
	}
}
