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
		MSActionManager.Goon.OnTeamChanged += UpdateIconsFromTeam;
	}
	
	void OnDisable()
	{
		MSActionManager.Goon.OnTeamChanged -= UpdateIconsFromTeam;
	}
	
	void UpdateIconsFromTeam()
	{
		for (int i = 0; i < icons.Length; i++) 
		{
			icons[i].Init(MSMonsterManager.instance.userTeam[i]);
		}
	}
}
