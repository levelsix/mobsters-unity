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
		levelLabel.text = CBKWhiteboard.localUser.level.ToString();
		expLabel.text = CBKWhiteboard.localUser.experience + "/" + CBKWhiteboard.nextLevelInfo.requiredExperience;
		expBar.fill = ((float)CBKWhiteboard.localUser.experience) / CBKWhiteboard.nextLevelInfo.requiredExperience;
	}
}
