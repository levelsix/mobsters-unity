using UnityEngine;
using System.Collections;

public class MSResourceStorage : MonoBehaviour {

	MSBuilding building;

	MSBuildingUpgrade buildingUpgrade;

	Animator animator;

	public float currFill;

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
			currFill = resource/building.combinedProto.storage.capacity;
			animator.SetFloat("Amount", currFill);
		}
	}

	void OnEnable()
	{
		buildingUpgrade.OnFinishUpgrade += OnFinishUpgrade;
		animator.SetFloat("Amount", currFill);
	}

	void OnDisable()
	{
		buildingUpgrade.OnFinishUpgrade -= OnFinishUpgrade;
	}

	[ContextMenu("determineResourceMaxima")]
	void OnFinishUpgrade()
	{
		MSResourceManager.instance.DetermineResourceMaxima();
	}
}
