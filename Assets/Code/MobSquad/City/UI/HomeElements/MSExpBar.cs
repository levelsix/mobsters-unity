using UnityEngine;
using System.Collections;

public class MSExpBar : MonoBehaviour {
	
	[SerializeField]
	UILabel levelLabel;
	
	[SerializeField]
	UILabel expLabel;
	
	[SerializeField]
	MSFillBar expBar;

	void OnEnable()
	{
		MSActionManager.Scene.OnCity += UpdateBar;

	}

	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= UpdateBar;
	}
	
	void UpdateBar()
	{
		if (!MSTutorialManager.instance.inTutorial)
		{   
		    levelLabel.text = MSWhiteboard.localUser.level.ToString();
		    expLabel.text = MSWhiteboard.localUser.experience + "/" + MSWhiteboard.nextLevelInfo.requiredExperience;
		    expBar.fill = ((float)MSWhiteboard.localUser.experience) / MSWhiteboard.nextLevelInfo.requiredExperience;

		}
	}
}
