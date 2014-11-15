using UnityEngine;
using System.Collections;

public abstract class MSBuildingFrame : MonoBehaviour {

	public const long SECONDS_IN_MINUTE = 60;
	public const long MILISECONDS_IN_MINUTE = 1000 * SECONDS_IN_MINUTE;

	public readonly Vector3 buildingAngle = new Vector3(45f,45f,0f);
	public readonly Vector3 buildingScale = new Vector3(0.02f, 0.02f, 0.02f);

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

		return true;
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
