using UnityEngine;
using System.Collections;

public class MSPrefabList : MonoBehaviour {

	public static MSPrefabList instance;
	
	void Awake()
	{
		instance = this;
	}

	public MSAchievementEntry achievementEntry;

	public ParticleSystem gemMatchSparkle;

	/// <summary>
	/// Match particles for each color.
	/// MANUALLY COORDINATED with the Gem Types array in PuzzleManager
	/// </summary>
	public PZMatchParticle[] matchParticle;

	public MSSimplePoolable characterDieParticle;

	public MSSimplePoolable flinchParticle;

	public MSSimplePoolable grenadeParticle;

	public MSSimplePoolable molotovParticle;

	public MSSimplePoolable orbBlowUpParticle;

	public MSSimplePoolable sparkleParticle;

	public MSSimplePoolable bombDropParticle;

	public MSSimplePoolable cratePrefab;

	public MSSimplePoolable planePrefab;

	public MSSimplePoolable bombPrefab;

	public MSSimplePoolable progressBar;
}
