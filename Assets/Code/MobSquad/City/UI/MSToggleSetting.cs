using UnityEngine;
using System.Collections;

public class MSToggleSetting : MonoBehaviour {

	[SerializeField]
	UISprite bg;

	enum Sound
	{
		MUSIC,
		EFFECTS,
	}

	[SerializeField]
	Sound toggleType;

	[SerializeField]
	TweenPosition tween;

	const float X_OFF_SET = 20;
	const float y_OFF_SET = -3;
	const string ACTIVE_BG = "activetoggle";
	const string INACTIVE_BG = "inactivetoggle";

	bool _toggle;

	bool toggle{
		get
		{
			return _toggle;
		}

		set
		{
			_toggle = value;
			if(value)
			{
				bg.spriteName = ACTIVE_BG;
			}
			else
			{
				bg.spriteName = INACTIVE_BG;
			}
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

		if(toggle)
		{
			tween.from = new Vector3(X_OFF_SET, y_OFF_SET, 0f);
			tween.to = new Vector3(-X_OFF_SET, y_OFF_SET, 0f);
		}
		else
		{
			tween.from = new Vector3(-X_OFF_SET, y_OFF_SET, 0f);
			tween.to = new Vector3(X_OFF_SET, y_OFF_SET, 0f);
//			tween.transform.localPosition = new Vector3(tween.transform.localPosition.x - X_OFF_SET, tween.transform.localPosition.y, 0f);
		}

		tween.Sample(0f, false);
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

		if(toggle)
		{
			tween.from = new Vector3(X_OFF_SET, y_OFF_SET, 0f);
			tween.to = new Vector3(-X_OFF_SET, y_OFF_SET, 0f);
		}
		else
		{
			tween.from = new Vector3(-X_OFF_SET, y_OFF_SET, 0f);
			tween.to = new Vector3(X_OFF_SET, y_OFF_SET, 0f);
		}

		tween.ResetToBeginning();
		tween.PlayForward();
	}
}
