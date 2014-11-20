using UnityEngine;
using System.Collections;

public class MSSoundManager : MonoBehaviour {

	public static MSSoundManager instance;

	[SerializeField] AudioSource basicSource;

	[SerializeField] AudioSource loopSource;

	[SerializeField] AudioSource loopMusic;
	
	public AudioClip battleMusic;
	public AudioClip gameplayMusic;

	public AudioClip genericPopup;

	public AudioClip generalClick;

	#region Puzzle Sfx

	public AudioClip[] combos;

	public AudioClip gemSwap;
	public AudioClip wrongMove;

	public AudioClip gemPop;
	public AudioClip rocket;
	public AudioClip gemExplode;
	public AudioClip[] rainbowParticleFire;
	public AudioClip comboFire;

	public AudioClip pistol;
	public AudioClip machineGun;
	public AudioClip meleeHit;
	public AudioClip damageClick;
	public AudioClip characterDie;

	public AudioClip makeItRain;
	public AudioClip plane;
	public AudioClip bombDrop;

	public AudioClip walking;

	public AudioClip boardSlideIn;
	public AudioClip swapCharacterSlideIn;
	public AudioClip clickSwap;
	
	public AudioClip task_win;
	public AudioClip task_lose;

	#endregion

	#region City Sfx

	public AudioClip buildingSelect;
	public AudioClip buildingMove;
	public AudioClip buildingDrop;
	public AudioClip buildingComplete;
	public AudioClip buildingFinishNow;
	public AudioClip buildingCantPlace;
	public AudioClip buildingCancel;
	public AudioClip clickDeselect;

	public AudioClip collectCash;
	public AudioClip collectOil;
	
	#endregion

	#region Tutorial Sfx
	
	public AudioClip jump;
	public AudioClip dialogueBox;
	public AudioClip boatScene;

	#endregion
	
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

	public void PlayOneShot(AudioClip[] clips)
	{
		PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);
	}

	public void PlayOneShot(AudioClip clip)
	{
		if (playSounds)
		{
			Debug.Log("Playing sound: " + clip.name);
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
