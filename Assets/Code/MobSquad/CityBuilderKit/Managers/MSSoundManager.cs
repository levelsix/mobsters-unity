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

	void Awake()
	{
		instance = this;
	}

	public void PlayOneShot(AudioClip clip)
	{
		basicSource.PlayOneShot(clip);
	}

	public void Loop(AudioClip clip)
	{
		loopSource.clip = clip;
		loopSource.Play();
	}

	public void StopLoop()
	{
		loopSource.Stop();
	}


}
