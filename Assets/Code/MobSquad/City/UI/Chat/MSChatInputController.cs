#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
#define MOBILE
#endif

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TweenPosition))]
public class MSChatInputController : MonoBehaviour 
{

	TweenPosition tweenPos;
	
	bool isItIn = false;
	
	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
	}

#if MOBILE
	void Update()
	{
		if (UIInput.mKeyboard != null && TouchScreenKeyboard.visible && !isItIn)
		{
			tweenPos.to = new Vector3(0, 
         		-TouchScreenKeyboard.area.yMin * Screen.height/640f);
			tweenPos.PlayForward();
			isItIn = true;
		}
		else if ((UIInput.mKeyboard == null || !TouchScreenKeyboard.visible) && isItIn)
		{
			tweenPos.PlayReverse();
			isItIn = false;
		}
	}
#endif
}
