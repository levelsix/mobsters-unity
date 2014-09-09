using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSSkillInfo : MonoBehaviour {

	[SerializeField]
	UISprite skillIcon;

	[SerializeField]
	UISprite iconBg;

	[SerializeField]
	UILabel skillName;

	[SerializeField]
	UILabel description;

	[SerializeField]
	Color activeTextColor;

	[SerializeField]
	Color inactiveTextColor;

	SkillProto skill;

	const string ACTIVE_BG = "activeskill";
	const string INACTIVE_BG = "insactiveskill";

	public void Init(SkillProto skill, bool active = true)
	{
		this.skill = skill;

		skillName.text = skill.name;
		//skillIcon.spriteName = 

		if (active)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	public void Activate()
	{
		skillName.color = activeTextColor;
		iconBg.spriteName = ACTIVE_BG;
		//description.text = skill.description;
	}

	public void Deactivate()
	{
		skillName.color = inactiveTextColor;
		iconBg.spriteName = INACTIVE_BG;
	}
}
