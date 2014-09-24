using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSSkillInfo : MonoBehaviour {

	[SerializeField]
	UI2DSprite skillIcon;

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

	[SerializeField]
	MSSkillInfo otherSkill;

	SkillProto skill;

	const string ACTIVE_BG = "activeskill";
	const string INACTIVE_BG = "insactiveskill";
	const string NO_SKILL = "noskillcircle";

	public void Init(int skillId, bool active = true)
	{
		skill = MSDataManager.instance.Get<SkillProto>(skillId);;

		if (skill != null)
		{
			skillName.text = skill.name;
			string skillBundleName = skill.iconImgName.Substring(0, skill.iconImgName.Length-8);
			MSSpriteUtil.instance.SetSprite(skillBundleName, skillBundleName+"icon", skillIcon);
			if (active)
			{
				Activate();
			}
			else
			{
				Deactivate();
			}
		}
		else
		{
			Deactivate();
			skillIcon.sprite2D = null;
			iconBg.spriteName = NO_SKILL;
			skillName.text = "No Skill";
		}
	}

	public void Activate()
	{
		skillName.color = activeTextColor;
		iconBg.spriteName = ACTIVE_BG;

		skillIcon.color = Color.white;
		description.text = skill.desc;
	}

	public void Deactivate()
	{
		skillName.color = inactiveTextColor;
		iconBg.spriteName = INACTIVE_BG;
		skillIcon.color = Color.black;
	}

	void OnClick()
	{
		if (skill != null)
		{
			Activate();
			otherSkill.Deactivate();
		}
	}
}
