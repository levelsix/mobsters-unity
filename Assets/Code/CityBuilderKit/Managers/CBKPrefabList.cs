using UnityEngine;
using System.Collections;

public class CBKPrefabList : MonoBehaviour {

	public static CBKPrefabList instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public CBKMoneyPickup moneyPrefab;

	public ParticleSystem gemMatchSparkle;

	/// <summary>
	/// Match particles for each color.
	/// MANUALLY COORDINATED with the Gem Types array in PuzzleManager
	/// </summary>
	public PZMatchParticle[] matchParticle;
}
