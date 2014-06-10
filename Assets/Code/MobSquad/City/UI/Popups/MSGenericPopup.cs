using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Generic popup
/// </summary>
public class MSGenericPopup : MonoBehaviour {
	
	/// <summary>
	/// The label.
	/// Set from editor within prefab.
	/// </summary>
	[SerializeField]
	UILabel label;

	[SerializeField]
	UILabel title;
	
	[SerializeField]
	MSActionButton[] buttons;
	
	const float BUTTON_WIDTH = 190;
	
	/// <summary>
	/// Init the specified message.
	/// TODO: Expands the sliced sprite to size appropriately
	/// TODO: Add handling for procedurally creating buttons with callbacks
	/// </summary>
	/// <param name='message'>
	/// Message to display
	/// </param>
	public void Init(string message)
	{		
		Init("", message, new string[]{"Okay"}, new string[]{"greymenuoption"}, new Action[]{MSActionManager.Popup.CloseTopPopupLayer});
	}

	public void Init(string title, string message)
	{
		Init(title, message, new string[]{"Okay"}, new string[]{"greymenuoption"}, new Action[]{MSActionManager.Popup.CloseTopPopupLayer});
	}
	
	public void Init(string message, string[] buttonLabels, string[] buttonSprites, Action[] buttonActions)
	{
		Init("", message, buttonLabels, buttonSprites, buttonActions);
	}
	
	public void Init(string title, string message, string[] buttonLabels, string[] buttonSprites, Action[] buttonActions)
	{
		if (buttonLabels.Length != buttonActions.Length)
		{
			throw new Exception("Length mismatch.");
		}
		label.text = message;
		this.title.text = message;
		float xOffset = (BUTTON_WIDTH * buttonLabels.Length) / 2;
		int i;
		for (i = 0; i < buttonLabels.Length; i++) 
		{
			buttons[i].gameObject.SetActive(true);
			buttons[i].transform.localPosition = new Vector3((i + 0.5f) * BUTTON_WIDTH - (xOffset), 
			                                                 buttons[i].transform.localPosition.y, buttons[i].transform.localPosition.z);
			buttons[i].label.text = buttonLabels[i];
			buttons[i].onClick = buttonActions[i];
		}
		for (; i < buttons.Length; i++) 
		{
			buttons[i].gameObject.SetActive(false);
		}
	}
	
}
