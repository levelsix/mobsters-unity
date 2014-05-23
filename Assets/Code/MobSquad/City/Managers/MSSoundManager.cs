using UnityEngine;
using System.Collections;

public class MSSoundManager : MonoBehaviour {

	public static MSSoundManager instance;

	public AudioSource basicSource;
	public AudioSource loopSource;

	public AudioClip[] combos;
	public AudioClip gemPop;
	public AudioClip pistol;
	public AudioClip plane;
	public AudioClip rocket;
	public AudioClip walking;
	public AudioClip wrongMove;

#if UNITY_EDITOR
	public bool playSounds = true;
#endif


	void Awake()
	{
		instance = this;
	}

	public void PlayOneShot(AudioClip clip)
	{
#if UNITY_EDITOR
		if (playSounds)
		{
			basicSource.PlayOneShot(clip);
		}
#else
		basicSource.PlayOneShot(clip);
#endif
	}

	public void Loop(AudioClip clip)
	{
#if UNITY_EDITOR
		if (playSounds)
		{
			loopSource.clip = clip;
			loopSource.Play();
		}
#else
		loopSource.clip = clip;
		loopSource.Play();
#endif
	}

	public void StopLoop()
	{
#if UNITY_EDITOR
		if (playSounds)
		{
			loopSource.Stop();
		}
#else
		loopSource.Stop();
#endif
	}


}
