using UnityEngine;
using System.Collections;

public class CBKStretchPanelToScreenWidth : MonoBehaviour {

	UIPanel scrollPanel;

	void Awake()
	{
		scrollPanel = GetComponent<UIPanel>();
	}

	void Start () 
	{
		scrollPanel.baseClipRegion = new Vector4(scrollPanel.baseClipRegion.x, scrollPanel.baseClipRegion.y, 
		                                         640f * Screen.width / Screen.height, scrollPanel.baseClipRegion.w);
	}

}
