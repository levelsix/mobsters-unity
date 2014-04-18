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
		evolvingGoon = monsterCard.goon;

		monsterCard.transform.parent = goonCardParent;

		//We've got to do this to force it to change what panel it's parented to
		monsterCard.gameObject.SetActive(false);
		monsterCard.gameObject.SetActive(true);

		SpringPosition.Begin(monsterCard.gameObject, Vector3.zero, 5);

		scientistCard.InitScientist(MSEvolutionManager.instance.currEvolution.catalystUserMonsterId);

		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(monsterCard.goon.monster.evolutionMonsterId);

		goonOutline.sprite2D = MSAtlasUtil.instance.GetMobsterSprite(evoMonster.imagePrefix);

		finalNameLabel.text = evoMonster.displayName;

		finalTimeLabel.text = MSUtil.TimeStringShort(monsterCard.goon.monster.minutesToEvolve * 60000);

		if (MSEvolutionManager.instance.ready)
		{
			if (MSEvolutionManager.instance.active)
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
		if (MSEvolutionManager.instance.active)
		{
			finalTimeLabel.text = MSUtil.TimeStringShort(MSEvolutionManager.instance.timeLeftMillis);
			button.label.text = "(G)" + Mathf.CeilToInt((MSEvolutionManager.instance.timeLeftMillis/6000f) / MSWhiteboard.constants.minutesPerGem);
		}
	}

	void SetDisabledButton()
	{
		button.button.enabled = false;
		button.icon.spriteName = greenButton;
		aboveButtonLabel.text = " ";
		button.label.text = "$" + evolvingGoon.monster.evolutionCost;
	}

	void SetEvolveButton()
	{
		button.button.enabled = true;
		button.icon.spriteName = greenButton;
		aboveButtonLabel.text = "Evolve";
		button.label.text = "$" + evolvingGoon.monster.evolutionCost;
	}

	void SetGemButton()
	{
		button.button.enabled = true;
		button.icon.spriteName = gemButton;
		aboveButtonLabel.text = "Finish Now";
		button.label.text = "(G)" + Mathf.CeilToInt((MSEvolutionManager.instance.timeLeftMillis/6000f) / MSWhiteboard.constants.minutesPerGem);
	}

	public void OnButtonClick()
	{
		if (MSEvolutionManager.instance.ready)
		{
			if (MSEvolutionManager.instance.active)
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
