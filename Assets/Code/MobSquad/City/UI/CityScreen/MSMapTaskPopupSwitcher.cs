using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class MSMapTaskPopupSwitcher : MonoBehaviour {

	MSPopupSwapper swapper;
	
	[SerializeField]
	MSTaskMap map;

	[SerializeField]
	MSMapTaskPopup mapTaskA;

	[SerializeField]
	MSMapTaskPopup mapTaskB;

	[SerializeField]
	MSMapTaskPopup evolutionEvent;

	[SerializeField]
	MSMapTaskPopup enhanceEvent;

	enum Popup
	{
		NONE,
		A,
		B,
		EVO,
		ENHANCE
	}

	Popup curPopup = Popup.NONE;

	void Awake()
	{
		swapper = GetComponent<MSPopupSwapper>();
	}

	void OnEnable()
	{
		MSActionManager.Map.OnMapTaskClicked += clickedMapTask;
		MSActionManager.Popup.CloseAllPopups += swapper.Close;
		MSActionManager.Popup.CloseAllPopups += delegate { curPopup = Popup.NONE; };
	}

	public void EndOfTween()
	{
		if(MSTutorialManager.instance.inTutorial)
		{
			map.taskButtons[2].OnClick();
		}
		else if(!activateEventPopup())
		{
			map.SelectNextTask();
		}
	}

	void OnDisable()
	{
		MSActionManager.Map.OnMapTaskClicked -= clickedMapTask;
		MSActionManager.Popup.CloseAllPopups -= swapper.Close;
		MSActionManager.Popup.CloseAllPopups -= delegate { curPopup = Popup.NONE; };
	}

	public bool activateEventPopup()
	{
		foreach(PersistentEventProto pEvent in MSEventManager.instance.GetActiveEvents())
		{
			swapEvent(pEvent);
			return true;
		}
		return false;
	}

	void clickedMapTask(TaskMapElementProto proto, MSMapTaskButton.TaskStatusType type)
	{
		if(curPopup != Popup.A)
		{
			mapTaskA.init(proto, type);
			swapper.SwapIn(mapTaskA.gameObject, delegate {}, ZeroAnchors(mapTaskA.GetComponent<UISprite>()));
			curPopup = Popup.A;
		}
		else
		{
			mapTaskB.init(proto, type);
			swapper.SwapIn(mapTaskB.gameObject, delegate {}, ZeroAnchors(mapTaskB.GetComponent<UISprite>()));
			curPopup = Popup.B;
		}
	}

	void swapEvent(PersistentEventProto pEvent)
	{
		switch(pEvent.type)
		{
		case PersistentEventProto.EventType.ENHANCE:
			if(curPopup != Popup.ENHANCE)
			{
				enhanceEvent.init(pEvent);
				swapper.SwapIn(enhanceEvent.gameObject, delegate {}, ZeroAnchors(enhanceEvent.GetComponent<UISprite>()));
				curPopup = Popup.ENHANCE;
			}
			break;
		case PersistentEventProto.EventType.EVOLUTION:
			if(curPopup != Popup.EVO)
			{
				evolutionEvent.init(pEvent);
				swapper.SwapIn(evolutionEvent.gameObject, delegate {}, ZeroAnchors(evolutionEvent.GetComponent<UISprite>()));
				curPopup = Popup.EVO;
			}
			break;
		default:
			Debug.LogError("An unknown event type was detected!Type:" + pEvent.type.ToString());
			break;
		}
	}

	bool ZeroAnchors(UIWidget widget)
	{
		bool wrong = widget.rightAnchor.absolute != 0 || widget.leftAnchor.absolute != 0;
		//Debug.Log("before " + widget.rightAnchor.absolute + ":" + widget.leftAnchor.absolute);
		widget.rightAnchor.absolute = 0;
		widget.leftAnchor.absolute = 0;
		widget.ResetAnchors();
		widget.UpdateAnchors();
		//Debug.Log("after " + widget.rightAnchor.absolute + ":" + widget.leftAnchor.absolute);

		return wrong;
	}

	/// <summary>
	/// debug function
	/// </summary>
	/// <returns>String for day of the week.</returns>
	/// <param name="day">Day.</param>
	string IntToDay(int day)
	{
		switch(day)
		{
		case 0:
			return "SUNDAY";
		case 1:
			return "MONDAY";
		case 2:
			return "TUESDAY";
		case 3:
			return "WEDNESDAY";
		case 4:
			return "THURSDAY";
		case 5:
			return "FRIDAY";
		case 6:
			return "SATURDAY";
		default:
			return "nope";
		}
	}
}
