using UnityEngine;
using System.Collections;

/// <summary>
/// Generic popup
/// </summary>
public class CBKPopup : MonoBehaviour {
	
	/// <summary>
	/// The label.
	/// Set from editor within prefab.
	/// </summary>
	[SerializeField]
	UILabel label;
	
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
		label.text = message;
	}
	
}
