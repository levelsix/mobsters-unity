using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PZScrollingBackground : MonoBehaviour {
	
	[SerializeField]
	List<CBKSimplePoolable> backgrounds = new List<CBKSimplePoolable>();
	
	Vector3 direction;
	
	[SerializeField]
	CBKSimplePoolable topPrefab;
	
	[SerializeField]
	CBKSimplePoolable bottomPrefab;
	
	bool wasLastTop = true;
	
	const float bottomThreshold = -560f;
	
	const float topThreshold = 200f;
	
	public float scrollSpeed = 60f;
	
	public static readonly Vector3 spawningOffset = new Vector3(513, 359.5f);
	
	// Use this for initialization
	void Start () 
	{
		if (backgrounds.Count < 2)
		{
			Debug.LogError("Assign the first pair of backgrounds before starting!");
		}
		
		direction = (backgrounds[0].transf.localPosition - backgrounds[1].transf.localPosition).normalized;
	}
	
	public void Scroll(CBKUnit withUnit)
	{
		withUnit.transf.localPosition += direction * scrollSpeed * Time.deltaTime;
		Scroll(scrollSpeed);
	}
	
	public void Scroll(float speed)
	{
		foreach (CBKSimplePoolable item in backgrounds) 
		{
			item.transf.localPosition += direction * speed * Time.deltaTime;
		}
		if (backgrounds[backgrounds.Count-1].transf.localPosition.y < topThreshold)
		{
			//Make new background
			Debug.Log("Add new background");
			CBKSimplePoolable back;
			if (wasLastTop)
			{
				back = CBKPoolManager.instance.Get(bottomPrefab, Vector3.zero) as CBKSimplePoolable;
			}
			else
			{
				back = CBKPoolManager.instance.Get(topPrefab, Vector3.zero) as CBKSimplePoolable;
			}
			back.transf.parent = transform;
			back.transf.localScale = Vector3.one;
			back.transf.localPosition = backgrounds[backgrounds.Count-1].transf.localPosition + spawningOffset;
			
			backgrounds.Add (back);
			
			wasLastTop = !wasLastTop;
		}
		CBKSimplePoolable first = backgrounds[0];
		if (first.transf.localPosition.y < bottomThreshold)
		{
			backgrounds.Remove(first);
			first.Pool();
		}
	}
	
	
	
}
