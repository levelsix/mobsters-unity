using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof (MSUIHelper))]
public class PZSkillIndicator : MonoBehaviour {

	[SerializeField] UI2DSprite fillIcon;
	[SerializeField] UI2DSprite bwIcon;
	[SerializeField] UISprite label;
	[SerializeField] UISprite counterSprite;
	[SerializeField] UILabel counterLabel;
	[SerializeField] TweenAlpha counterTween;

	MSUIHelper helper;

	[SerializeField] TweenPosition mover;

	[SerializeField] bool enemy;

	Element monsterElement = Element.NO_ELEMENT;

	float currFill;

	int maxPoints;

	bool ready = false;

	static readonly Dictionary<Element, string> spriteElementPrefixes = new Dictionary<Element, string>()
	{
		{Element.FIRE, "fire"},
		{Element.DARK, "night"},
		{Element.EARTH, "earth"},
		{Element.LIGHT, "light"},
		{Element.WATER, "water"},
		{Element.NO_ELEMENT, "inactive"},
		{Element.ROCK, "inactive"}
	};

	const string enemySpritePrefix = "enemy";
	const string yourSpritePrefix = "your";

	[SerializeField] float fillSpeed = 1;

	public void Init(SkillProto skill, Element userElement)
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();

		if (userElement == Element.NO_ELEMENT) Debug.LogWarning("Dafuq");

		if (skill != null && skill.skillId > 0)
		{
			collider.enabled = true;
			maxPoints = skill.orbCost;
			monsterElement = userElement;

			string skillBundle = skill.iconImgName;
			string skillIconName = skill.iconImgName + "icon";
//			Debug.Log("Skill!: " + skillBundle + ", icon: " + skillIconName);
			MSSpriteUtil.instance.SetSprite(skillBundle, skillIconName, bwIcon);
			MSSpriteUtil.instance.SetSprite(skillBundle, skillIconName, fillIcon);

			counterSprite.spriteName = spriteElementPrefixes[userElement] + "counter";

			SetPoints(0);
			helper.FadeIn();
			mover.PlayForward();

			counterTween.Sample(0, true);
		}
		else
		{
			collider.enabled = false;
//			Debug.Log("No skill");
			maxPoints = 0;
			helper.FadeOut();
		}
	}

	public void SetPoints(int currPoints)
	{
		currPoints = Mathf.Clamp(currPoints, 0, maxPoints);
		ready = false;
		if (maxPoints == 0)
		{
			currFill = fillIcon.fillAmount = 1;
			counterLabel.text = "Passive";
		}
		else
		{

			currFill = Mathf.Clamp01(((float)currPoints)/maxPoints);
			counterLabel.text = "(" + monsterElement.ToString()[0] + ") " + currPoints + "/" + maxPoints;
			if (currFill >= 1) ready = true;
		}

		string spriteName;
		if (currFill >= 1)
		{
			spriteName = spriteElementPrefixes[monsterElement];
		}
		else
		{
			spriteName = "inactive";
		}
		spriteName += (enemy ? enemySpritePrefix : yourSpritePrefix) + "skill";

		label.spriteName = spriteName;
	}

	/// <summary>
	/// Tweens and fades the indicator out.
	/// Used when a mobster with a skill dies or is swapped out.
	/// </summary>
	public void FadeOut()
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();
		mover.PlayReverse();
		helper.FadeOut();
	}

	/// <summary>
	/// Turns the indicator completely off.
	/// Used at the beginning of battles.
	/// </summary>
	public void ShutOff()
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();
		mover.Sample(0, true);
		helper.ResetAlpha(false);
	}

	void OnClick()
	{
		if (ready && PZPuzzleManager.instance.swapLock == 0)
		{
			ready = false;
			PZCombatManager.instance.RunPlayerSkill();
		}
		else
		{
			StartCoroutine(FadeInCounter());
		}
	}

	IEnumerator FadeInCounter()
	{
		counterTween.PlayForward();
		yield return new WaitForSeconds(5);
		counterTween.PlayReverse();
	}

	void Update()
	{
		if (fillIcon.fillAmount < currFill)
		{
			fillIcon.fillAmount = Mathf.Min(fillIcon.fillAmount + fillSpeed * Time.deltaTime, currFill);
			fillIcon.MarkAsChanged();
		}
		else if (fillIcon.fillAmount > currFill)
		{
			fillIcon.fillAmount = Mathf.Max(fillIcon.fillAmount - fillSpeed * Time.deltaTime, currFill);
			fillIcon.MarkAsChanged();
		}
	}
}
