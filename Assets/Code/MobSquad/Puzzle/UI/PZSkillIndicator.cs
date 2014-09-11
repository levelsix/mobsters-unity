using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof (MSUIHelper))]
public class PZSkillIndicator : MonoBehaviour {

	[SerializeField] UISprite fillIcon;
	[SerializeField] UISprite bwIcon;
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

	static readonly Dictionary<SkillType, string> skillSprites = new Dictionary<SkillType, string>()
	{
		{SkillType.CAKE_DROP, "cakedropicon"},
		{SkillType.JELLY, "goosplashicon"},
		{SkillType.QUICK_ATTACK, "cheapshoticon"}
	};

	const string enemySpritePrefix = "enemy";
	const string yourSpritePrefix = "your";

	public void Init(SkillProto skill, Element userElement, int pointsNeeded)
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();

		if (skill != null && skill.skillId > 0)
		{
			maxPoints = pointsNeeded;
			monsterElement = userElement;
			bwIcon.spriteName = fillIcon.spriteName = skillSprites[skill.type];
			SetPoints(0);
			helper.FadeIn();
		}
		else
		{
			maxPoints = 0;
			helper.FadeOut();
		}
	}

	public void SetPoints(int currPoints)
	{
		if (maxPoints == 0) return;

		currFill = fillIcon.fillAmount = ((float)currPoints)/maxPoints;
		string spriteName;
		if (currFill >= 1)
		{
			spriteName = spriteElementPrefixes[monsterElement];
		}
		else
		{
			spriteName = "inactive";
		}
	}

	public void FadeOut()
	{
		if (helper == null) helper = GetComponent<MSUIHelper>();

		helper.FadeOut();
	}
}
