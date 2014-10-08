using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSHideObjectsWhileActive : MonoBehaviour {
	[SerializeField]
	bool revealInstead = false;
	public bool clearOnDisable = false;
	public bool fadeAlpha = true;
	public float fadeTime = 0.3f;

	[SerializeField]
	List<GameObject> thingsToHide = new List<GameObject>();

	float[] originalAlpha;
	bool[] originalActiveState;
	
	void OnEnable()
	{
		originalAlpha = new float[thingsToHide.Count];
		originalActiveState = new bool[thingsToHide.Count];

		for(int i = 0; i < thingsToHide.Count; i++)
		{
			if(thingsToHide[i].GetComponent<UIWidget>() != null)
			{
				originalAlpha[i] = thingsToHide[i].GetComponent<UIWidget>().alpha;
			}
			else
			{
				originalActiveState[i] = thingsToHide[i].activeSelf;
			}
		}

		if(revealInstead)
		{
			ChangeState(false, false);
		}
		else
		{
			ChangeState(true, false);
		}
	}
	
	void OnDisable()
	{
		if(revealInstead)
		{
			ChangeState(true, true);
		}
		else
		{
			ChangeState(false, true);
		}


		if(clearOnDisable)
		{
			thingsToHide.Clear();
		}
	}

	void ChangeState(bool hide, bool originalState)
	{
		for(int i = 0; i < thingsToHide.Count; i++)
		{
			if(thingsToHide[i] != null)
			{
				if(thingsToHide[i].GetComponent<UIWidget>() != null)
				{
					if(fadeAlpha)
					{
						if(originalState)
						{
							TweenAlpha.Begin(thingsToHide[i], fadeTime, originalAlpha[i]);
						}
						else
						{
							TweenAlpha.Begin(thingsToHide[i], fadeTime, hide?0f:1f);
						}
					}
					else
					{
						if(originalState)
						{
							thingsToHide[i].GetComponent<UIWidget>().alpha = originalAlpha[i];
						}
						else
						{
							thingsToHide[i].GetComponent<UIWidget>().alpha = hide?0f:1f;
						}
					}
				}
				else
				{
					if(originalState)
					{
						thingsToHide[i].SetActive(originalActiveState[i]);
					}
					else
					{
						thingsToHide[i].SetActive(!hide);
					}
				}
			}
		}
	}

	/// <summary>
	/// Adds the item to be hidden.  Must add items while the gameObject is inactive
	/// </summary>
	/// <param name="toBeHidden">To be hidden.</param>
	public void AddItemToBeHidden(GameObject toBeHidden)
	{
		if(this.gameObject.activeSelf)
		{
			//you probably won't need to add stuff while it's already active
			//I doubt any one else will even use this.
			Debug.LogWarning("Objects added while active are ignored.");
		}
		else
		{
			if(!thingsToHide.Contains(toBeHidden))
			{
				thingsToHide.Add(toBeHidden);
			}
		}
	}
}
