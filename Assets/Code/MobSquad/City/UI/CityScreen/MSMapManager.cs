using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapManager : MonoBehaviour {

	[SerializeField]
	MSTaskMap taskMap;

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
	void InitMapTasks(StartupResponseProto response)
	{
		taskMap.StartInit();
	}

}
