using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMoneyBar : MonoBehaviour {

	[SerializeField]
	MSFillBar bar;
	
	[SerializeField]
	UILabel capacityLabel;
	
	void UpdateBar()
	{
		//capacityLabel.text = "Max: " + (CBKDataManager.instance.Get(typeof(staticlevelinfo), CBKWhiteboard.localUser.level) as static 
	}
}
