using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSProfileTeammate : MonoBehaviour {

	#region UI Elements

	[SerializeField]
	GameObject hasMobsterElements;

	[SerializeField]
	GameObject noMobsterElements;

	[SerializeField]
	UISprite bg;

	[SerializeField]
	UISprite mobsterBackground;

	[SerializeField]
	UI2DSprite goonThumb;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel levelLabel;

	[SerializeField]
	Color unselectedTint;

	[SerializeField]
	MSProfileMobster centerMobster;

	[SerializeField]
	MSProfileTeammate[] otherTeammates;

	#endregion

	PZMonster monster;

	public void Init(FullUserMonsterProto userMonster)
	{
		if (userMonster != null && userMonster.monsterId > 0)
		{
			PZMonster monster = new PZMonster(userMonster);
			Init (monster);
		}
		else
		{
			InitEmpty();
			Deselect();
		}
	}

	public void Init(PZMonster monster)
	{
		this.monster = monster;

		if (monster != null)
		{
			hasMobsterElements.SetActive(true);
			noMobsterElements.SetActive(false);

			mobsterBackground.spriteName = MSGoonCard.mediumBackgrounds[monster.monster.monsterElement];
			MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Card", goonThumb);
			nameLabel.text = monster.monster.displayName;
			levelLabel.text = "LEVEL " + monster.userMonster.currentLvl;
		}
		else
		{
			InitEmpty();
		}

		Deselect();
	}

	void InitEmpty()
	{
		hasMobsterElements.SetActive(false);
		noMobsterElements.SetActive(true);
	}

	public void Select()
	{
		centerMobster.Init(monster);
		bg.color = Color.white;
		foreach (var item in otherTeammates) 
		{
			item.Deselect();
		}
	}

	public void Deselect()
	{
		bg.color = unselectedTint;
	}
	
	void OnClick()
	{
		Select();
	}
}
