using UnityEngine;
using System.Collections;

public class MSColorMatch : MonoBehaviour {
	
	public UIWidget WidgetToMatch;

	public bool alphaMatch = false;

	void Update()
	{
		UIWidget thisWidget = GetComponent<UIWidget>();
		if(alphaMatch)
		{
			thisWidget.color = WidgetToMatch.color;
		}
		else
		{
			thisWidget.color = new Color(WidgetToMatch.color.r, WidgetToMatch.color.g, WidgetToMatch.color.b, thisWidget.color.a);
		}
	}
}
