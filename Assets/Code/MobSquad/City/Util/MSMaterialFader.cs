using UnityEngine;
using System.Collections;

public class MSMaterialFader : MonoBehaviour {

	public float fadeTime;

	Material mat;

	Color start = Color.white;

	Color end = new Color(1, 1, 1, 0);

	public void Awake()
	{
		mat = renderer.material;
	}

	public void Fade()
	{
		StartCoroutine(RunFade());
	}

	IEnumerator RunFade()
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			mat.color = Color.Lerp(start, end, t/fadeTime);
			yield return null;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			Fade();
		}
	}
}
