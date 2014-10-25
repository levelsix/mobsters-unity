using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDeployPopup : MonoBehaviour {

	[SerializeField]
	PZDeployCard[] slots;

	[SerializeField]
	PZSwapButton swapButton;

	[SerializeField]
	Camera cam;
	
	const int NUM_SLOTS = 3;

	UITweener tween;

	public static bool acting = false;

	void Awake()
	{
		tween = GetComponent<UITweener>();
	}

	void OnEnable()
	{
		MSActionManager.Controls.OnAnyTap[0] += OnGlobalTap;
		MSActionManager.Puzzle.OnDeploy += OnDeploy;
	}
	
	void OnDisable()
	{
		MSActionManager.Controls.OnAnyTap[0] -= OnGlobalTap;
		MSActionManager.Puzzle.OnDeploy -= OnDeploy;
	}

	/// <summary>
	/// Raises the deploy event.
	/// </summary>
	/// <param name="kenneth">The best monster there is.</param>
	void OnDeploy(PZMonster kenneth)
	{
		acting = false;
		tween.PlayReverse();
		swapButton.Show();
	}

	void OnGlobalTap(TCKTouchData data)
	{
		if (acting && PZCombatManager.instance.activePlayer.alive)
		{
			acting = false;
			tween.PlayReverse();
			swapButton.Show();
		}
	}

	public void Init()
	{
		Init (MSMonsterManager.instance.userTeam);
	}

	public void Init(PZMonster[] userMonsters)
	{
		tween.PlayForward();
		for (int i = 0; i < userMonsters.Length; i++) 
		{
			if (userMonsters[i] != null && userMonsters[i].monster != null && userMonsters[i].monster.monsterId > 0)
			{
				slots[i].Init(userMonsters[i]);
			}
			else
			{
				slots[i].InitEmpty();
			}
		}
		StartCoroutine(StartActingInAFrame());
	}

	IEnumerator StartActingInAFrame()
	{
		yield return null;
		acting = true;
	}
}
