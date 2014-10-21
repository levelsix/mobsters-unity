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

	[SerializeField] bool enemy;

	Element monsterElement;

	float currFill;

	int maxPoints;

	static readonly Dictionary<Element, string> spriteElementPrefixes = new Dictionary<Element, string>()
	{
		{Element.FIRE, "fire"},
		{Element.DARK, "night"},
		{Element.EARTH, "earth"},
		{Element.LIGHT, "light"},
		{Element.WATER, "water"}
	};

	const string enemySpritePrefix = "enemy";
	const string yourSpritePrefix = "your";

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

			string skillImgBase = MSUtil.StripExtensions(skill.iconImgName);
			MSSpriteUtil.instance.SetSprite(skillImgBase, skillImgBase + "icon", bwIcon);
			MSSpriteUtil.instance.SetSprite(skillImgBase, skillImgBase + "icon", fillIcon);

			SetPoints(0);
			helper.FadeIn();
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
		if (maxPoints == 0) return;

		//currFill = fillIcon.fillAmount = ((float)currPoints)/maxPoints;
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

	public void FadeOut()
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();

		helper.FadeOut();
	}

	public void Off()
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();

		helper.ResetAlpha(false);
	}

	void OnClick()
	{
		PZCombatManager.instance.RunPlayerSkill();
	}
}
