﻿using UnityEngine;
using System.Collections;

public class MSBottomChat : MonoBehaviour {

	public static MSBottomChat instance;

	[SerializeField]
	Transform[] bottomChats;

	[SerializeField]
	UISprite[] dots;

	[SerializeField]
	UITweener[] hideTweens;

	[SerializeField]
	UITweener[] unhideTweens;

	[SerializeField]
	UITweener[] openTweens;

	[SerializeField]
	UITweener[] closeTweens;

	[SerializeField]
	UICenterOnChild centerer;

	[SerializeField]
	MSChatPopup chatPopup;

	[SerializeField]
	GameObject noPrivateChats;

	[SerializeField]
	UIGrid blockGrid;

	int currentChatIndex;

	float currDrag;

	long startTime;

	[SerializeField]
	float dragThreshold = 100;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		blockGrid.cellWidth = GetComponent<UISprite>().width;
		blockGrid.Reposition();
		SetDots();
	}

	public void Hide()
	{
		foreach (var item in hideTweens) 
		{
			item.PlayForward();
		}
	}

	public void Unhide()
	{
		foreach (var item in hideTweens) 
		{
			item.PlayReverse();
		}
	}
	
	public void Open()
	{
		foreach (var item in openTweens) 
		{
			item.ResetToBeginning();
			item.Play();
		}
	}

	public void Close()
	{
		foreach (var item in closeTweens) 
		{
			item.ResetToBeginning();
			item.Play();
		}
	}

	void OnClick()
	{
		if (Mathf.Abs(currDrag) < dragThreshold)
		{
			MSActionManager.Popup.OnPopup(chatPopup.GetComponent<MSPopup>());
			chatPopup.Init(bottomChats[currentChatIndex].GetComponent<MSBottomChatBlock>().chatMode);
		}
	}

	void OnPress(bool isDown)
	{
		if (isDown)
		{
			currDrag = 0;
		}
	}

	void OnDrag(Vector2 drag)
	{
		if (Mathf.Abs (currDrag) < dragThreshold)
		{
			currDrag += drag.x;
			if (currDrag < -dragThreshold)
			{
				SlideRight();
			}
			else if (currDrag > dragThreshold)
			{
				SlideLeft();
			}
		}
	}

	void SlideLeft()
	{
		if (currentChatIndex > 0)
		{
			centerer.CenterOn(bottomChats[--currentChatIndex]);
		}
		SetDots();
	}

	void SlideRight()
	{
		if (currentChatIndex < bottomChats.Length-1)
		{
			centerer.CenterOn(bottomChats[++currentChatIndex]);
		}
		SetDots();
	}

	void SetDots()
	{
		for (int i = 0; i < dots.Length; i++) 
		{
			if (i == currentChatIndex)
			{
				dots[i].spriteName = "activechatline";
			}
			else
			{
				if (dots[i].spriteName == "activechatline")
				{
					dots[i].spriteName = "inactivechatline";
				}
			}
			dots[i].MakePixelPerfect();
		}
	}

	public void AlertDot(MSValues.ChatMode chatType)
	{
		if (chatType != MSValues.ChatMode.GLOBAL && dots[(int)chatType].spriteName != "activechatline")
		{
			dots[(int)chatType].spriteName = "newchatline";
			dots[(int)chatType].MakePixelPerfect();
		}
	}
}
