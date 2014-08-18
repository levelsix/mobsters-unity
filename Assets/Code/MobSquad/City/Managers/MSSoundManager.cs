using UnityEngine;
using System.Collections;

public class MSSoundManager : MonoBehaviour {

	public static MSSoundManager instance;

	[SerializeField] AudioSource basicSource;

	[SerializeField] AudioSource loopSource;

	[SerializeField] AudioSource loopMusic;

	public AudioClip[] combos;
	public AudioClip gemPop;
	public AudioClip pistol;
	public AudioClip plane;
	public AudioClip rocket;
	public AudioClip walking;
	public AudioClip wrongMove;

	public AudioClip task_win;
	public AudioClip task_lose;

	public AudioClip battleMusic;
	public AudioClip gameplayMusic;

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

	void OnEnable()
	{
		MSActionManager.Scene.OnCity += delegate { LoopMusic(gameplayMusic); };
		MSActionManager.Scene.OnPuzzle += delegate { LoopMusic(battleMusic); };
	}

	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= delegate { LoopMusic(gameplayMusic); };
		MSActionManager.Scene.OnPuzzle -= delegate { LoopMusic(battleMusic); };
	}

	public bool ToggleSoundEffects()
	{
		playSounds = !playSounds;
		PlayerPrefs.SetInt(SOUND_EFFECTS, playSounds?1:0);

		if(!playSounds)
		{
			StopLoop();
		}

		return playSounds;
	}

	public bool ToggleMusic()
	{
		playMusic = !playMusic;
		PlayerPrefs.SetInt(MUSIC, playMusic?1:0);

		if(playMusic)
		{
			switch(MSWhiteboard.currSceneType)
			{
			case MSWhiteboard.SceneType.CITY:
				LoopMusic(gameplayMusic);
				break;
			case MSWhiteboard.SceneType.PUZZLE:
				LoopMusic(gameplayMusic);
				break;
			}
		}
		else
		{
			StopMusic();
		}

		return playMusic;
	}

	public void PlayOneShot(AudioClip clip)
	{
		if (playSounds)
		{
			basicSource.PlayOneShot(clip);
		}
	}

	public void Loop(AudioClip clip)
	{

		if (playSounds)
		{
			loopSource.clip = clip;
			loopSource.Play();
		}
	}

	public void StopLoop()
	{
			loopSource.Stop();
	}

	public void LoopMusic(AudioClip music)
	{
		if(playMusic)
		{
			loopMusic.clip = music;
			loopMusic.Play();
		}
	}

	public void StopMusic()
	{
		loopMusic.Stop ();
	}


}
