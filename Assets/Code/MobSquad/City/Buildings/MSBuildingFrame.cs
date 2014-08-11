using UnityEngine;
using System.Collections;

public abstract class MSBuildingFrame : MonoBehaviour {

	public UISprite bubbleIcon;

	public abstract void CheckTag ();

	protected bool Precheck()
	{
		if(MSTutorialManager.instance.inTutorial)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// If the building is spawned before all the information we need is ready
	/// call this to wait a single frame
	/// </summary>
	public void FirstFrameCheck(){
		StartCoroutine(WaitThenCheck());
	}

	IEnumerator WaitThenCheck(){
		yield return null;
		 CheckTag();
	}
	
}
