using UnityEngine;
using System;
using System.Collections;
using com.lvl6.proto;

public class MSBarEvent : MonoBehaviour {

	public Element currElement;

	[SerializeField] Color darkYellow;
	[SerializeField] Color yellow;
	[SerializeField] Color lightYellow;

	[SerializeField] Color darkGreen;
	[SerializeField] Color green;
	[SerializeField] Color lightGreen;

	[SerializeField] Color darkBlue;
	[SerializeField] Color blue;
	[SerializeField] Color lightBlue;

	[SerializeField] Color darkPurple;
	[SerializeField] Color purple;
	[SerializeField] Color lightPurple;

	[SerializeField] Color darkRed;
	[SerializeField] Color red;
	[SerializeField] Color lightRed;

	[SerializeField] Color darkGrey;
	[SerializeField] Color grey;
	[SerializeField] Color lightGrey;


	Color _darkColor;
	public Color darkColor
	{
		get
		{
			return _darkColor;
		}
	}
	Color _color;
	public Color color
	{
		get
		{
			return _color;
		}
	}
	Color _lightColor;
	public Color lightColor
	{
		get
		{
			return _lightColor;
		}
	}

	[SerializeField]
	MSTextureAnimaiton textureAnimation;

	[SerializeField]
	UILabel eventName;

	[SerializeField]
	UISprite tag;

	[SerializeField]
	UILabel timeLeft;

	[SerializeField]
	UILabel requirements;

	[SerializeField]
	UISprite timer;

	[SerializeField]
	UISprite bg;

	[SerializeField]
	Texture greyCakeKid;

	[SerializeField]
	MSGoonScreen goonScreen;

	FullTaskProto task;

	void Awake()
	{
		bg = GetComponent<UISprite>();
	}

	[ContextMenu("updateColor")]
	public void UpdateColors()
	{
		switch(currElement)
		{
		case Element.LIGHT:
			_darkColor = darkYellow;
			_color = yellow;
			_lightColor = lightYellow;
			break;
		case Element.EARTH:
			_darkColor = darkGreen;
			_color = green;
			_lightColor = lightGreen;
			break;
		case Element.WATER:
			_darkColor = darkBlue;
			_color = blue;
			_lightColor = lightBlue;
			break;
		case Element.DARK:
			_darkColor = darkPurple;
			_color = purple;
			_lightColor = lightPurple;
			break;
		case Element.FIRE:
			_darkColor = darkRed;
			_color = red;
			_lightColor = lightRed;
			break;
		default:
			_darkColor = darkGrey;
			_color = grey;
			_lightColor = lightGrey;
			break;
		}

		eventName.gradientBottom = _lightColor;
		eventName.effectColor = _darkColor;

		timeLeft.gradientTop = _darkColor;
		timeLeft.gradientBottom = _color;

		//rock is used as like an off button
		if(currElement == Element.NO_ELEMENT)
		{
			bg.spriteName = "greyeventbg";
			tag.spriteName = "greyeventtag";
			textureAnimation.GetComponent<UITexture>().mainTexture = greyCakeKid;

			requirements.gameObject.SetActive(true);
			timer.gameObject.SetActive(false);
		}
		//dark sprites use 'night' not 'dark'
		else if(currElement == Element.DARK)
		{
			bg.spriteName = "nighteventbg";
			tag.spriteName = "nighteventtag";
			timer.spriteName = "nighteventtimer";
			
			requirements.gameObject.SetActive(false);
			timer.gameObject.SetActive(true);
		}
		else
		{
			bg.spriteName = currElement.ToString().ToLower() + "eventbg";
			tag.spriteName = currElement.ToString().ToLower() + "eventtag";
			timer.spriteName = currElement.ToString().ToLower() + "eventtimer";

			requirements.gameObject.SetActive(false);
			timer.gameObject.SetActive(true);
		}
	}

	public void Init(PersistentEventProto pEvent)
	{
		task = MSDataManager.instance.Get<FullTaskProto>(pEvent.taskId);
		float minutes = pEvent.startHour * 60 + pEvent.eventDurationMinutes - DateTime.Now.Hour * 60 + DateTime.Now.Minute;
		float hours = Mathf.Floor(minutes / 60);
		minutes -= Mathf.Floor(hours * 60);
		timeLeft.text = hours + "H " + minutes + "M";
		
		eventName.text = MSDataManager.instance.Get<FullTaskProto>(pEvent.taskId).name;

		LoadAnimation(pEvent);

		currElement = pEvent.monsterElement;

		if(MSBuildingManager.enhanceLabs[0] == null || MSBuildingManager.enhanceLabs[0].combinedProto.structInfo.level < 5)
		{
			currElement = Element.NO_ELEMENT;
		}

		UpdateColors();

	}

	void LoadAnimation(PersistentEventProto pEvent)
	{
		string colorName;

		switch(pEvent.monsterElement)
		{
		case Element.DARK:
			colorName = "purple";
			break;
		case Element.EARTH:
			colorName = "green";
			break;
		case Element.FIRE:
			colorName = "red";
			break;
		case Element.LIGHT:
			colorName = "yellow";
			break;
		case Element.WATER:
			colorName = "blue";
			break;
		default:
			colorName = "";
			break;
		}

		if(pEvent.type == PersistentEventProto.EventType.ENHANCE)
		{
			textureAnimation.frames = new Texture[13];
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("fat_boy_" + colorName, "", AddFrame);
		}
		else if(pEvent.type == PersistentEventProto.EventType.EVOLUTION)
		{
			//animation is made up of breath breath blink breath breath turn
			textureAnimation.frames = new Texture[13+13+13+13+13+17];
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "breach", AddFrame);
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "breach", AddFrame);
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "blink", AddFrame);
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "breach", AddFrame);
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "breach", AddFrame);
			MSSpriteUtil.instance.RunForEachTypeInBundle<Texture>("scientist_" + colorName, "turn", AddFrame);
		}
	}

	void AddFrame(Texture sprite)
	{
		for(int i  = 0; i < textureAnimation.frames.Length; i++)
		{
			if(textureAnimation.frames[i] == null)
			{
				textureAnimation.frames[i] = sprite;
				return;
			}
		}

		Debug.LogError("adding a sprite went wrong : " + sprite.name);
	}

	public void OnClick()
	{
		if(currElement != Element.NO_ELEMENT && currElement != Element.ROCK)
		{
			goonScreen.EnterEventScreen();
		}
	}
}