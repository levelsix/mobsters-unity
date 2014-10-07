using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSHideObjectsWhileActive : MonoBehaviour {

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
				UIWidget widget = thingsToHide[i].GetComponent<UIWidget>();
				originalAlpha[i] = widget.alpha;
				if(fadeAlpha)
				{
					TweenAlpha.Begin(thingsToHide[i], fadeTime, 0f);
				}
				else
				{
					widget.alpha = 0f;
				}
			}
			else
			{
				originalActiveState[i] = thingsToHide[i].activeSelf;
				thingsToHide[i].SetActive(false);
			}
		}
	}
	
	void OnDisable()
	{
		for(int i = 0; i < thingsToHide.Count; i++)
		{
			if(thingsToHide[i].GetComponent<UIWidget>() != null)
			{
				if(fadeAlpha)
				{
					TweenAlpha.Begin(thingsToHide[i], fadeTime, originalAlpha[i]);
				}
				else
				{
					thingsToHide[i].GetComponent<UIWidget>().alpha = originalAlpha[i];
				}
			}
			else
			{
				originalActiveState[i] = thingsToHide[i].activeSelf;
				thingsToHide[i].SetActive(true);
			}
		}

		if(clearOnDisable)
		{
			thingsToHide.Clear();
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
			Debug.LogError("Don't add objects to be hidden while already active");
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
