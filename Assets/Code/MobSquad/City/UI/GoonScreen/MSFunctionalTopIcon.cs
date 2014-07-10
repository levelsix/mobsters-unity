using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (MSUIHelper))]
public class MSFunctionalTopIcon : MonoBehaviour 
{
	[SerializeField]
	UISprite icon;

	public UILabel label;

	[HideInInspector]
	public MSUIHelper helper;

	GoonScreenMode mode;

	static readonly Dictionary<GoonScreenMode, string> iconNames = new Dictionary<GoonScreenMode, string>()
	{
		{GoonScreenMode.TEAM, "manageteammenuheader"},
		{GoonScreenMode.SELL, "residencemenuheader"},
		{GoonScreenMode.HEAL, "hospitalmenuheader"},
		{GoonScreenMode.PICK_ENHANCE, "enhancelabmenuheader"},
		{GoonScreenMode.DO_ENHANCE, "enhancelabmenuheader"},
		{GoonScreenMode.PICK_EVOLVE, "evolutionlabmenuheader"},
		{GoonScreenMode.DO_EVOLVE, "evolutionlabmenuheader"}
	};

	static readonly Dictionary<GoonScreenMode, string> baseWords = new Dictionary<GoonScreenMode, string>()
	{
		{GoonScreenMode.TEAM, "My Team"},
		{GoonScreenMode.SELL, "Sell Mobsters"},
		{GoonScreenMode.HEAL, "Heal Mobsters"},
		{GoonScreenMode.PICK_ENHANCE, "Enhance Mobsters"},
		{GoonScreenMode.DO_ENHANCE, "Enhance Mobsters"},
		{GoonScreenMode.PICK_EVOLVE, "Evolve Mobsters"},
		{GoonScreenMode.DO_EVOLVE, "Evolve Mobsters"}
	};

	void Awake()
	{
		helper = GetComponent<MSUIHelper>();
	}

	public void Init(GoonScreenMode mode)
	{
		icon.spriteName = iconNames[mode];
		icon.MakePixelPerfect();

		label.text = baseWords[mode];

		this.mode = mode;
	}
}
