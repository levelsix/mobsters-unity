using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PZMatch {
	
	/// <summary>
	/// Whether this match was created from the result
	/// of a special gem
	/// </summary>
	public bool special;
	
	/// <summary>
	/// Whether this match contains multiple matches
	/// </summary>
	public int multi = 0;
	
	/// <summary>
	/// The list of gems in this match
	/// </summary>
	public List<PZGem> gems;
	
	public PZMatch ()
	{
		gems = new List<PZGem>();
		special = false;
		multi = 0;
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name='gems'>
	/// Gems.
	/// </param>
	public PZMatch (List<PZGem> gems, bool special = false)
	{
		this.gems = MSUtil.CopyList<PZGem>(gems);
		this.special = special;
		this.multi = 0;
	}
	
	/// <summary>
	/// Checks to see if this is a 
	/// </summary>
	/// <param name='otherMatch'>
	/// Other list.
	/// </param>
	public void CheckAgainst(PZMatch otherMatch)
	{
		bool stop = false;
		foreach (PZGem item in gems) 
		{
			foreach (PZGem otherItem in otherMatch.gems) 
			{
				if (item == otherItem)
				{
					CombineWith(otherMatch);
					stop = true;
					break;
				}
			}
			if (stop)
			{
				break;
			}
		}
	}
	
	/// <summary>
	/// Combines this match with another match, creating
	/// a multi match.
	/// </summary>
	/// <param name='otherMatch'>
	/// Other match to combine with.
	/// </param>
	void CombineWith(PZMatch otherMatch)
	{
		foreach (PZGem item in otherMatch.gems)
		{
			if (!gems.Contains(item))
			{
				gems.Add(item);
			}
			else
			{
				item.dragged = true; //We do this so that a gem that falls into an L will become the bomb
			}
		}
		otherMatch.gems.Clear();
		multi += otherMatch.multi + 1;
	}
	
	public void Destroy()
	{	
		gems.Sort((gem1, gem2) => -gem1.dragged.CompareTo(gem2.dragged));
		
		int i = 0;
		if (!special) //Don't make special gems if this is the result of a special detonation
		{
			if (multi > 0)
			{
				//Make special bomb gem, and save gem
				gems[i++].gemType = PZGem.GemType.BOMB;
				//PZPuzzleManager.instance.gemsOnBoardByType[gems[i].colorIndex]++;
			}
			else if (gems.Count > 3)
			{
				if (gems.Count == 4)
				{
					//Make special rocket gem, and save gem
					gems[i++].gemType = PZGem.GemType.ROCKET;
					//PZPuzzleManager.instance.gemsOnBoardByType[gems[i].colorIndex]++;
				}
				else
				{
					//Make special molly gem, and save gem
					PZGem molly = gems[i++];
					molly.gemType = PZGem.GemType.MOLOTOV;
					molly.colorIndex = -1;
					molly.sprite.color = Color.white;
				}
			}
		}
		
		for (; i < gems.Count; i++) 
		{
			gems[i].Destroy();
		}
	}
	
	public override string ToString ()
	{
		string str = "Match: ";
		foreach (PZGem item in gems) {
			str += item + " ";
		}
		return str;
	}
}
