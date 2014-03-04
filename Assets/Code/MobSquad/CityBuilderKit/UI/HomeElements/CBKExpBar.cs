using UnityEngine;
using System.Collections;

public class CBKExpBar : MonoBehaviour {
	
	[SerializeField]
	UILabel levelLabel;
	
	[SerializeField]
	UILabel expLabel;
	
	[SerializeField]
	CBKFillBar expBar;

	void OnEnable()
	{
		CBKEventManager.Scene.OnCity += UpdateBar;
	}

	void OnDisable()
	{
		CBKEventManager.Scene.OnCity -= UpdateBar;
	}
	
	void UpdateBar()
	{
		levelLabel.text = MSWhiteboard.localUser.level.ToString();
		expLabel.text = MSWhiteboard.localUser.experience + "/" + MSWhiteboard.nextLevelInfo.requiredExperience;
		expBar.fill = ((float)MSWhiteboard.localUser.experience) / MSWhiteboard.nextLevelInfo.requiredExperience;
	}
}
