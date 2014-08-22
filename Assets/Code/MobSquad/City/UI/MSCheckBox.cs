using UnityEngine;
using System;
using System.Collections;

public class MSCheckBox : MonoBehaviour {

	[SerializeField]
	UISprite CheckMark;

	bool _checkMarked;

	public bool checkMarked
	{
		set
		{
			_checkMarked = value;
			CheckMark.gameObject.SetActive(value);
			if(OnToggle != null)
			{
				OnToggle(value);
			}
		}
		get
		{
			return _checkMarked;
		}
	}

	/// <summary>
	/// The bool is if the button is being set to checked
	/// </summary>
	public Action<bool> OnToggle;

	public void OnClick(){
		checkMarked = !checkMarked;
	}
}
