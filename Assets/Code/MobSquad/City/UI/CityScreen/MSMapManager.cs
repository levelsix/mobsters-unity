using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapManager : MonoBehaviour {

	public static MSMapManager instance;

	[SerializeField]
	MSTaskMap taskMap;

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += InitMapTasks;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= InitMapTasks;
	}

	/// <summary>
	/// Initializes all tasks
	/// </summary>
	public void InitMapTasks(StartupResponseProto response)
	{
		taskMap.StartInit();
	}

}
