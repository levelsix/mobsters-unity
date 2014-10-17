using UnityEngine;
using System.Collections;

public abstract class MSBuildingFrame : MonoBehaviour {

	public UISprite bubbleIcon;

	protected MSBuilding building;

	public abstract void CheckTag ();

	protected void Awake()
	{
		building = GetComponent<MSBuilding>();
		bubbleIcon = building.bubbleIcon;
		if(building == null || bubbleIcon == null)
		{
			Debug.LogError(gameObject.name + "could not aquire all required components for bubble icons");
		}
	}

	/// <summary>
	/// True if bubbleIcon should show
	/// </summary>
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
