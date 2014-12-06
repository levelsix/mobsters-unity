using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSEvolutionElements
/// </summary>
public class MSEvolutionElements : MonoBehaviour {

	[SerializeField]
	MSGoonCard scientistCard;

	[SerializeField]
	Transform goonCardParent;

	[SerializeField]
	UI2DSprite goonOutline;

	[SerializeField]
	UILabel finalNameLabel;

	[SerializeField]
	UILabel finalTimeLabel;

	[SerializeField]
	UILabel aboveButtonLabel;

	[SerializeField]
	MSActionButton button;

	PZMonster evolvingGoon;

	public MSGoonCard evolvingCard;

	string greenButton = "confirm";

	string gemButton = "finishbuild";

	public void Init(MSGoonCard monsterCard)
	{
		evolvingCard = monsterCard;
		evolvingGoon = monsterCard.monster;

		monsterCard.transform.parent = goonCardParent;

		//We've got to do this to force it to change what panel it's parented to
		monsterCard.gameObject.SetActive(false);
		monsterCard.gameObject.SetActive(true);

		SpringPosition.Begin(monsterCard.gameObject, Vector3.zero, 5);

		scientistCard.InitScientist(MSEvolutionManager.instance.currEvolution.catalystUserMonsterUuid);

		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(monsterCard.monster.monster.evolutionMonsterId);

		MSSpriteUtil.instance.SetSprite(evoMonster.imagePrefix, evoMonster.imagePrefix + "Character", goonOutline);

		finalNameLabel.text = evoMonster.displayName;

		finalTimeLabel.text = MSUtil.TimeStringShort(monsterCard.monster.monster.minutesToEvolve * 60000);

		if (MSEvolutionManager.instance.ready)
		{
			if (MSEvolutionManager.instance.hasEvolution)
			{
				SetGemButton();
			}
			else
			{
				SetEvolveButton();
			}
		}
		else
		{
			SetDisabledButton();
		}
	}

	void Update()
	{
		if (MSEvolutionManager.instance.hasEvolution)
		{
			finalTimeLabel.text = MSUtil.TimeStringShort(MSEvolutionManager.instance.timeLeftMillis);
			button.label.text = "(G)" + MSMath.GemsForTime(MSEvolutionManager.instance.timeLeftMillis, false);
		}
	}

	void SetDisabledButton()
	{
		button.GetComponent<UIButton>().enabled = false;
		button.icon.spriteName = greenButton;
		aboveButtonLabel.text = " ";
		button.label.text = "$" + evolvingGoon.monster.evolutionCost;
	}

	void SetEvolveButton()
	{
		button.GetComponent<UIButton>().enabled = true;
		button.icon.spriteName = greenButton;
		aboveButtonLabel.text = "Evolve";
		button.label.text = "$" + evolvingGoon.monster.evolutionCost;
	}

	void SetGemButton()
	{
		button.GetComponent<UIButton>().enabled = true;
		button.icon.spriteName = gemButton;
		aboveButtonLabel.text = "Finish Now";
		button.label.text = "(G)" + MSMath.GemsForTime(MSEvolutionManager.instance.timeLeftMillis, false);
	}

	/// <summary>
	/// ASSIGNED IN EDITOR
	/// </summary>
	public void OnButtonClick()
	{
		if (MSEvolutionManager.instance.ready)
		{
			if (MSEvolutionManager.instance.hasEvolution)
			{
				MSEvolutionManager.instance.FinishWithGems();
			}
			else
			{
				MSEvolutionManager.instance.StartEvolution();
				SetGemButton();
			}
		}
	}

}
