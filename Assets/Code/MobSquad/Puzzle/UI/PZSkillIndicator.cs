using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof (MSUIHelper))]
public class PZSkillIndicator : MonoBehaviour {

	[SerializeField] UI2DSprite fillIcon;
	[SerializeField] UI2DSprite bwIcon;
	[SerializeField] UISprite label;

	MSUIHelper helper;

	[SerializeField] TweenPosition mover;

	[SerializeField] bool enemy;

	Element monsterElement = Element.NO_ELEMENT;

	float currFill;

	int maxPoints;

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

		if (skill != null && skill.skillId > 0)
		{
			collider.enabled = skill.activationType == SkillActivationType.USER_ACTIVATED;
			Debug.Log("Skill!: " + skill.name);
			if (skill.orbCost > 0)
			{
				maxPoints = skill.orbCost;
			}
			else
			{
				if (skill.type == SkillType.JELLY)
				{
					maxPoints = (int)skill.properties.Find(x=>x.name == "SPAWN_TURNS").skillValue;
				}
			}
			monsterElement = userElement;

			string skillBundle = MSUtil.SkillBundleName(skill);
			string skillIconName = MSUtil.StripExtensions(skill.iconImgName);
			MSSpriteUtil.instance.SetSprite(skillBundle, skillIconName, bwIcon);
			MSSpriteUtil.instance.SetSprite(skillBundle, skillIconName, fillIcon);

			SetPoints(0);
			helper.FadeIn();
			mover.PlayForward();
		}
		else
		{
			collider.enabled = false;
			Debug.Log("No skill");
			maxPoints = 0;
			helper.FadeOut();
		}
	}

	public void SetPoints(int currPoints)
	{
		if (maxPoints == 0)
		{
			currFill = fillIcon.fillAmount = 1;
		}
		else
		{
			currFill = ((float)currPoints)/maxPoints;
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
		PZCombatManager.instance.RunPlayerSkill();
	}

	void Update()
	{
		if (fillIcon.fillAmount < currFill)
		{
			fillIcon.fillAmount = Mathf.Min(fillIcon.fillAmount + fillSpeed * Time.deltaTime, currFill);
		}
		else if (fillIcon.fillAmount > currFill)
		{
			fillIcon.fillAmount = Mathf.Max(fillIcon.fillAmount - fillSpeed * Time.deltaTime, currFill);
		}
	}
}
