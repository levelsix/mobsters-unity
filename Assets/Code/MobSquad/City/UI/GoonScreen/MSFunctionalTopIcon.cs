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
		{GoonScreenMode.DO_EVOLVE, "evolutionlabmenuheader"},
		{GoonScreenMode.MINIJOB, "minijobsbuilding"}
	};

	static readonly Dictionary<GoonScreenMode, string> baseWords = new Dictionary<GoonScreenMode, string>()
	{
		{GoonScreenMode.TEAM, "MY TEAM"},
		{GoonScreenMode.SELL, "SELL TOONS"},
		{GoonScreenMode.HEAL, "HEAL TOONS"},
		{GoonScreenMode.PICK_ENHANCE, "ENHANCE TOONS"},
		{GoonScreenMode.DO_ENHANCE, "ENHANCE TOONS"},
		{GoonScreenMode.PICK_EVOLVE, "EVOLVE TOONS"},
		{GoonScreenMode.DO_EVOLVE, "EVOLVE TOONS"},
		{GoonScreenMode.MINIJOB, "MINI JOBS"}
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
