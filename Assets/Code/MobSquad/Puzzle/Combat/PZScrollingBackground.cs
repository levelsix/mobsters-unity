using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZScrollingBackground : MonoBehaviour {

	public static PZScrollingBackground instance;

	[SerializeField]
	SpriteRenderer topLeft;
	
	[SerializeField]
	SpriteRenderer topRight;
	
	[SerializeField]
	SpriteRenderer bottomLeft;
	
	[SerializeField]
	SpriteRenderer bottomRight;

	[SerializeField]
	List<MSSimplePoolable> backgrounds = new List<MSSimplePoolable>();

	public SpriteRenderer[] sprites;

	Vector3 _direction = Vector3.zero;

	public Vector3 direction
	{
		get
		{
			if (_direction == Vector3.zero)
			{
				_direction = (backgrounds[0].transf.localPosition - backgrounds[1].transf.localPosition).normalized;
			}
			return _direction;
		}
		set
		{
			_direction = value;
		}
	}
	
	[SerializeField]
	MSSimplePoolable topPrefab;
	
	[SerializeField]
	MSSimplePoolable bottomPrefab;
	
	bool wasLastTop = true;
	
	const float bottomThreshold = -560f;
	
	const float topThreshold = 200f;
	
	public float scrollSpeed = 60f;

	public FullTaskProto lastTaskActivated;
	
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

	public void Scroll(MSUnit[] withUnits)
	{
		foreach (var item in withUnits) 
		{
			item.transf.localPosition += direction * scrollSpeed * Time.deltaTime;
		}
		Scroll(scrollSpeed);
	}

	public void Scroll(MSUnit withUnit)
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

	public void StopScroll()
	{
		scrolling = false;
	}

	IEnumerator KeepScroll()
	{
		while (scrolling)
		{
			Scroll(scrollSpeed);
			yield return null;
		}
	}

	public void Scroll()
	{
		Scroll(scrollSpeed);
	}

	public void Scroll(float speed)
	{
		foreach (MSSimplePoolable item in backgrounds) 
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
			MSSimplePoolable back;
			if (wasLastTop)
			{
				back = MSPoolManager.instance.Get(bottomPrefab, Vector3.zero) as MSSimplePoolable;
			}
			else
			{
				back = MSPoolManager.instance.Get(topPrefab, Vector3.zero) as MSSimplePoolable;
			}
			back.transform.parent = transform;
			back.transform.localScale = Vector3.one;
			back.transform.localPosition = backgrounds[backgrounds.Count-1].transform.localPosition + spawningOffset;

			backgrounds.Add (back);
			
			wasLastTop = !wasLastTop;
			sprites = GetComponentsInChildren<SpriteRenderer>(true);
			SetAlpha(1);
		}
		MSSimplePoolable first = backgrounds[0];
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

	public void SetBackgrounds(FullTaskProto task)
	{
		lastTaskActivated = task;
		
		string prefix = task.groundImgPrefix;

		SetSingleBackground(true, true, topLeft);
		SetSingleBackground(true, false, topRight);
		SetSingleBackground(false, true, bottomLeft);
		SetSingleBackground(false, false, bottomRight);
	}
	
	public void SetSingleBackground(bool top, bool left, SpriteRenderer sprite)
	{
		
		string prefix = lastTaskActivated.groundImgPrefix;
		string spriteName;
		if(top && left)
		{
			spriteName = "scene1left";
		}
		else if (top && !left)
		{
			spriteName = "scene1right";
		}
		else if (!top && left)
		{
			spriteName = "scene2left";
		}
		else
		{
			spriteName = "scene2right";
		}

		MSSpriteUtil.instance.SetSprite(prefix + "Scene", prefix + spriteName, sprite);
		if(sprite.sprite == null)
		{
			MSSpriteUtil.instance.SetSprite("1Scene", "1" + spriteName, sprite);
		}

	}
	
	
}
