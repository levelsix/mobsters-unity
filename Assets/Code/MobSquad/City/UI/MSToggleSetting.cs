using UnityEngine;
using System.Collections;

public class MSToggleSetting : MonoBehaviour {

	[SerializeField]
	UILabel stateLabel;

	enum Sound
	{
		MUSIC,
		EFFECTS,
	}

	[SerializeField]
	Sound toggleType;

	bool _toggle;

	bool toggle{
		get
		{
			return _toggle;
		}

		set
		{
			_toggle = value;
			stateLabel.text = value?"ON":"OFF";
		}
	}

	void Awake()
	{
		if(toggleType == Sound.MUSIC)
		{
			toggle = MSSoundManager.instance.playMusic;
		}
		else
		{
			toggle = MSSoundManager.instance.playSounds;
		}
	}

	public void OnClick()
	{
		if(toggleType == Sound.MUSIC)
		{
			toggle = MSSoundManager.instance.ToggleMusic();
		}
		else
		{
			toggle = MSSoundManager.instance.ToggleSoundEffects();
		}
	}
}
