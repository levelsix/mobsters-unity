using UnityEngine;
using System.Collections;

public class CBKMenuSlideButton : MonoBehaviour {
	
	public enum Direction {LEFT, RIGHT};
	
	[SerializeField]
	Direction slideDirection;
	
	[SerializeField]
	TweenPosition slidingOut;
	
	[SerializeField]
	TweenPosition slidingIn;
	
	[SerializeField]
	GameObject popup;
	
	[SerializeField]
	bool closeOut = false;
	
	bool isTweening = false;
	
	const float SLIDE_TIME = 0.5f;
	
	public virtual void Slide()
	{
		if (!isTweening)
		{
			isTweening = true;
			if (popup != null && !popup.activeSelf)
			{
				CBKEventManager.Popup.OnPopup(popup);
			}
			
			slidingOut.from = Vector3.zero;
			slidingIn.to = Vector3.zero;
			
			switch(slideDirection)
			{
			case Direction.LEFT:
				slidingOut.to = new Vector3(-Screen.width, 0, 0);
				slidingIn.from = new Vector3(Screen.width, 0, 0);
				break;
			case Direction.RIGHT:
				slidingOut.to = new Vector3(Screen.width, 0, 0);
				slidingIn.from = new Vector3(-Screen.width, 0, 0);
				break;
			}
			
			slidingIn.duration = SLIDE_TIME;
			slidingOut.duration = SLIDE_TIME;

			slidingIn.ResetToBeginning();
			slidingIn.PlayForward();
			slidingOut.ResetToBeginning();
			slidingOut.PlayForward();
			
			StartCoroutine(WaitForFinish());
			
		}
	}
	
	IEnumerator WaitForFinish()
	{
		yield return new WaitForSeconds(SLIDE_TIME);
		if (closeOut)
		{
			CBKEventManager.Popup.CloseTopPopupLayer();
		}
		isTweening = false;
	}
}
