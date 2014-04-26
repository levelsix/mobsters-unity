using UnityEngine;
using System.Collections;

public class MSResourceStorage : MonoBehaviour {

	MSBuilding building;

	MSBuildingUpgrade buildingUpgrade;

	Animator animator;

	void Awake()
	{
		building = GetComponent<MSBuilding>();
		buildingUpgrade = GetComponent<MSBuildingUpgrade>();
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
		MSResourceManager.instance.DetermineResourceMaxima();
	}
}
