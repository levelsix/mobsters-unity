using UnityEngine;
using System.Collections;

public class CBKResourceStorage : MonoBehaviour {

	CBKBuilding building;

	CBKBuildingUpgrade buildingUpgrade;

	void Awake()
	{
		buildingUpgrade = GetComponent<CBKBuildingUpgrade>();
	}

	void OnEnable()
	{
		buildingUpgrade.OnFinishUpgrade += OnFinishUpgrade;
	}

	void OnDisable()
	{
		buildingUpgrade.OnFinishUpgrade -= OnFinishUpgrade;
	}

	void OnFinishUpgrade()
	{
		CBKResourceManager.instance.DetermineResourceMaxima();
	}
}
