#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public enum CombatTurn {PLAYER, ENEMY};

/// <summary>
/// PZCombatScheduler
/// @author Rob Giusti
/// </summary>
public class PZCombatScheduler : MonoBehaviour 
{

	public static PZCombatScheduler instance;

	List<CombatTurn> turns = new List<CombatTurn>();

	void Awake()
	{
		if (instance != null)
		{
			Destroy(instance.gameObject);
		}
		instance = this;
	}

	public void Schedule(PZMonster player, PZMonster enemy, bool justSwapped)
	{
		Schedule(player.speed, enemy.speed, justSwapped);
	}

	void Schedule(int speedA, int speedB, bool justSwapped)
	{
		turns.Clear();

		//Insure that we have valid values for speed
		speedA = Mathf.Max(speedA, 1);
		speedB = Mathf.Max(speedB, 1);

		//The number of sets of interleavings 
		int numInterleavings = Mathf.Min(speedA, speedB);

		bool firstAttackerIsA = justSwapped ? false : ChooseFirst(speedA, speedB);

#if DEBUG
		Debug.Log("Scheduler:\nSpeed A: " + speedA + "\nSpeed B: " + speedB
		          + "\nInterleavings: " + numInterleavings + "\nA first? " + firstAttackerIsA);
#endif

		int numBpA = 1, numBpB = 1;
		//If one speed is more than 2x the other, it gets 2x the moves on its turn
		if (speedA < speedB)
		{
			numBpB = speedB/speedA;
		}
		else
		{
			numBpA = speedA/speedB;
		}

		for (int i = 0; i < numInterleavings; i++) 
		{
			if (firstAttackerIsA)
			{
				AddTurns(numBpA, CombatTurn.PLAYER);
				AddTurns(numBpB, CombatTurn.ENEMY);
			}
			else
			{
				AddTurns(numBpB, CombatTurn.ENEMY);
				AddTurns(numBpA, CombatTurn.PLAYER);
			}
		}
	
		speedA -= numInterleavings * numBpA;
		speedB -= numInterleavings * numBpB;

		#if DEBUG
		string turnsD = "Turns beofre extra turns:";
		foreach (var item in turns) 
		{
			turnsD += "\n" + item.ToString();
		}
		Debug.Log(turnsD);
		#endif

		int numLeft = Mathf.Max(speedA, speedB);

		//Figure out how many extra turns to disperse
		if (numLeft > 0)
		{
			CombatTurn extraTurn = speedA > 0 ? CombatTurn.PLAYER : CombatTurn.ENEMY;
			bool firstAttkIsVal = speedA > 0 ? firstAttackerIsA : !firstAttackerIsA;
			int numToSkip = speedA > 0 ? numBpB : numBpA;

			//Create a list of all the turns
			List<int> slots = new List<int>();
			for (int i = 0; i < numInterleavings; i++) 
			{
				slots.Add(i);
			}

			//Pick random turns to insert the extra turns into
			MSUtil.ShuffleList<int>(slots);
			slots = slots.GetRange(0, Mathf.Min(numLeft, numInterleavings));
			slots.Sort((x, y) => y.CompareTo(x)); //Reverse sort
			foreach (int num in slots) 
			{
				int index = num * (numBpA + numBpB) + (firstAttkIsVal ? 0 : numToSkip);
				turns.Insert(index, extraTurn);
			}
		}


		#if DEBUG
		turnsD = "Turns with extra turns:";
		foreach (var item in turns) 
		{
			turnsD += "\n" + item.ToString();
		}
		Debug.Log(turnsD);
		#endif

	}

	bool ChooseFirst(int speedA, int speedB)
	{
		return (UnityEngine.Random.Range(0, speedA + speedB) < speedA);
	}

	void AddTurns(int numTurns, CombatTurn turn)
	{
		for (int i = 0; i < numTurns; i++) 
		{
			turns.Add(turn);
		}
	}

	[ContextMenu ("Test")]
	public void Test()
	{
		Schedule (1, 1, false);
		Schedule (4, 8, false);
		Schedule (5, 5, true);
		Schedule (-5, 8, false);
	}
}
