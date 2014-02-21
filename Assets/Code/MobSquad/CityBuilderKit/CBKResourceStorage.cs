using UnityEngine;
using System.Collections;

public class CBKResourceStorage : MonoBehaviour {

	CBKBuilding building;

	CBKBuildingUpgrade buildingUpgrade;

	Animator animator;

	void Awake()
	{
		building = GetComponent<CBKBuilding>();
		buildingUpgrade = GetComponent<CBKBuildingUpgrade>();
		animator = building.sprite.GetComponent<Animator>();
	}

	public void SetAmount(float resource)
	{
		if (animator != null && animator.runtimeAnimatorController != null)
		{
			animator.SetFloat("Amount", resource/building.combinedProto.storage.capacity);
		}
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
