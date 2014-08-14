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

	public const string SOUND_EFFECTS = "soundEffects";
	public const string MUSIC = "music";


#if UNITY_EDITOR
	public bool playSounds;
	public bool playMusic;
#endif


	void Awake()
	{
		playSounds = (PlayerPrefs.GetInt(SOUND_EFFECTS, 1) == 1);
		playMusic = (PlayerPrefs.GetInt(MUSIC, 1) == 1);
		instance = this;
	}

	public bool ToggleSoundEffects()
	{
		playSounds = !playSounds;
		PlayerPrefs.SetInt(SOUND_EFFECTS, playSounds?1:0);
		return playSounds;
	}

	public bool ToggleMusic()
	{
		playMusic = !playMusic;
		PlayerPrefs.SetInt(MUSIC, playMusic?1:0);
		return playMusic;
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
