using UnityEngine;
using System.Collections;

public class CBKHomeTopBar : MonoBehaviour {
	
	[SerializeField]
	CBKGoonCircleIcon[] icons;
	
	void Start()
	{
		UpdateIconsFromTeam();
	}
	
	void OnEnable()
	{
		CBKEventManager.Goon.OnTeamChanged += UpdateIconsFromTeam;
	}
	
	void OnDisable()
	{
		CBKEventManager.Goon.OnTeamChanged -= UpdateIconsFromTeam;
	}
	
	void UpdateIconsFromTeam()
	{
		for (int i = 0; i < icons.Length; i++) 
		{
			icons[i].Init(CBKMonsterManager.userTeam[i]);
		}
	}
}
