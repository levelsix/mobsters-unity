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
	
	public bool playSounds;
	public bool playMusic;


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
		if (playSounds)
		{
			basicSource.PlayOneShot(clip);
		}
	}

	public void Loop(AudioClip clip, bool music = false)
	{
		if (music ? playMusic : playSounds)
		{
			loopSource.clip = clip;
			loopSource.Play();
		}
	}

	public void StopLoop()
	{
		loopSource.Stop();
	}


}
