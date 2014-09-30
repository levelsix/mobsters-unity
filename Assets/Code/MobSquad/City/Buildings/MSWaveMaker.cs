using UnityEngine;
using System.Collections;

public class MSWaveMaker : MonoBehaviour 
{
	[SerializeField] float timeBetweenWaves;
	[SerializeField] float timeForWaves;

	[SerializeField] MSWave wavePrefab;

	void OnEnable()
	{
		StartCoroutine(MakeWaves());
	}

	IEnumerator MakeWaves()
	{
		yield return null;
		MSWave wave;
		while(true)
		{
			wave = MSPoolManager.instance.Get<MSWave>(wavePrefab, transform);
			wave.Init(timeForWaves);
			yield return new WaitForSeconds(timeBetweenWaves);
		}
	}
}
