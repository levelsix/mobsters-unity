using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PZScrollingBackground : MonoBehaviour {

	public static PZScrollingBackground instance;

	[SerializeField]
	List<CBKSimplePoolable> backgrounds = new List<CBKSimplePoolable>();

	public SpriteRenderer[] sprites;

	public Vector3 direction;
	
	[SerializeField]
	CBKSimplePoolable topPrefab;
	
	[SerializeField]
	CBKSimplePoolable bottomPrefab;
	
	bool wasLastTop = true;
	
	const float bottomThreshold = -560f;
	
	const float topThreshold = 200f;
	
	public float scrollSpeed = 60f;
	
	public static readonly Vector3 spawningOffset = new Vector3(513, 359.5f);

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		if (backgrounds.Count < 2)
		{
			Debug.LogError("Assign the first pair of backgrounds before starting!");
		}
		sprites = GetComponentsInChildren<SpriteRenderer>(true);
		
		direction = (backgrounds[0].transf.localPosition - backgrounds[1].transf.localPosition).normalized;
	}
	
	public void Scroll(CBKUnit withUnit)
	{
		withUnit.transf.localPosition += direction * scrollSpeed * Time.deltaTime;
		Scroll(scrollSpeed);
	}

	public bool scrolling = false;

	[ContextMenu("StartScroll")]
	public void StartScroll()
	{
		scrolling = true;
		StartCoroutine(KeepScroll());
	}

	IEnumerator KeepScroll()
	{
		while (scrolling)
		{
			Scroll(scrollSpeed);
			yield return null;
		}
	}
	
	public void Scroll(float speed)
	{
		foreach (CBKSimplePoolable item in backgrounds) 
		{
			item.transform.localPosition += direction * speed * Time.deltaTime;
		}
		if (PZCombatManager.instance.crate != null)
		{
			PZCombatManager.instance.crate.transform.localPosition += direction * speed * Time.deltaTime;
		}

		if (backgrounds[backgrounds.Count-1].transf.localPosition.y < topThreshold)
		{
			//Make new background
			CBKSimplePoolable back;
			if (wasLastTop)
			{
				back = CBKPoolManager.instance.Get(bottomPrefab, Vector3.zero) as CBKSimplePoolable;
			}
			else
			{
				back = CBKPoolManager.instance.Get(topPrefab, Vector3.zero) as CBKSimplePoolable;
			}
			back.transform.parent = transform;
			back.transform.localScale = Vector3.one;
			back.transform.localPosition = backgrounds[backgrounds.Count-1].transform.localPosition + spawningOffset;

			backgrounds.Add (back);
			
			wasLastTop = !wasLastTop;
			sprites = GetComponentsInChildren<SpriteRenderer>(true);
			SetAlpha(1);
		}
		CBKSimplePoolable first = backgrounds[0];
		if (first.transform.localPosition.y < bottomThreshold)
		{
			backgrounds.Remove(first);
			first.Pool();
			sprites = GetComponentsInChildren<SpriteRenderer>(true);
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach(SpriteRenderer sprite in sprites)
		{
			sprite.color = Color.Lerp(new Color(1,1,1,0), Color.white, alpha);
		}
	}
	
	
}
