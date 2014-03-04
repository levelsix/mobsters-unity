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
	CBKGoonCard scientistCard;

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
	CBKActionButton button;

	PZMonster evolvingGoon;

	public CBKGoonCard evolvingCard;

	string greenButton = "confirm";

	string gemButton = "finishbuild";

	public void Init(CBKGoonCard monsterCard)
	{
		evolvingCard = monsterCard;
		evolvingGoon = monsterCard.goon;

		monsterCard.transform.parent = goonCardParent;

		//We've got to do this to force it to change what panel it's parented to
		monsterCard.gameObject.SetActive(false);
		monsterCard.gameObject.SetActive(true);

		SpringPosition.Begin(monsterCard.gameObject, Vector3.zero, 5);

		scientistCard.InitScientist(CBKEvolutionManager.instance.currEvolution.catalystUserMonsterId);

		MonsterProto evoMonster = CBKDataManager.instance.Get<MonsterProto>(monsterCard.goon.monster.evolutionMonsterId);

		goonOutline.sprite2D = CBKAtlasUtil.instance.GetMobsterSprite(evoMonster.imagePrefix);

		finalNameLabel.text = evoMonster.displayName;

		finalTimeLabel.text = CBKUtil.TimeStringShort(monsterCard.goon.monster.minutesToEvolve * 60000);

		if (CBKEvolutionManager.instance.ready)
		{
			if (CBKEvolutionManager.instance.active)
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
		if (CBKEvolutionManager.instance.active)
		{
			finalTimeLabel.text = CBKUtil.TimeStringShort(CBKEvolutionManager.instance.timeLeftMillis);
			button.label.text = "(G)" + Mathf.CeilToInt((CBKEvolutionManager.instance.timeLeftMillis/6000f) / MSWhiteboard.constants.minutesPerGem);
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
		button.label.text = "(G)" + Mathf.CeilToInt((CBKEvolutionManager.instance.timeLeftMillis/6000f) / MSWhiteboard.constants.minutesPerGem);
	}

	public void OnButtonClick()
	{
		if (CBKEvolutionManager.instance.ready)
		{
			if (CBKEvolutionManager.instance.active)
			{
				CBKEvolutionManager.instance.FinishWithGems();
			}
			else
			{
				CBKEvolutionManager.instance.StartEvolution();
				SetGemButton();
			}
		}
	}

}
