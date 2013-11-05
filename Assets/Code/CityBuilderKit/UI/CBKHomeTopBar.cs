using UnityEngine;
using System.Collections;

public class CBKHomeTopBar : MonoBehaviour {
	
	[SerializeField]
	CBKGoonCircleIcon[] icons;
	
	void OnEnable()
	{
		CBKEventManager.Goon.OnTeamChanged += UpdateIconsFromTeam;
		UpdateIconsFromTeam();
	}
	
	void OnDisable()
	{
		CBKEventManager.Goon.OnTeamChanged -= UpdateIconsFromTeam;
	}
	
	void UpdateIconsFromTeam()
	{
		for (int i = 0; i < icons.Length; i++) 
		{
			icons[i].Init(CBKMonsterManager.instance.userTeam[i]);
		}
	}
}
