using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKBuildStructureButton : MonoBehaviour {

	FullStructureProto structProto;
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	UISprite costResourceIcon;
	
	[SerializeField]
	UILabel costLabel;
	
	[SerializeField]
	UILabel incomeLabel;
	
	[SerializeField]
	UILabel timeLabel;
	
	public void Init(FullStructureProto proto)
	{
		
	}
}
