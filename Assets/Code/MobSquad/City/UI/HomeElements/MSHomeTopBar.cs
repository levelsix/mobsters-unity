using UnityEngine;
using System.Collections;

public class MSHomeTopBar : MonoBehaviour {
	
	[SerializeField]
	MSGoonCircleIcon[] icons;
	
	void Start()
	{
		UpdateIconsFromTeam();
	}
	
	void OnEnable()
	{
		MSActionManager.Goon.OnTeamChanged += UpdateIconsFromTeam;
		MSActionManager.Scene.OnCity += UpdateIconsFromTeam;
	}
	
	void OnDisable()
	{
		MSActionManager.Goon.OnTeamChanged -= UpdateIconsFromTeam;
		MSActionManager.Scene.OnCity -= UpdateIconsFromTeam;
	}
	
	void UpdateIconsFromTeam()
	{
		for (int i = 0; i < icons.Length; i++) 
		{
			icons[i].Init(MSMonsterManager.instance.userTeam[i]);
		}
	}
}
