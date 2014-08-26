using UnityEngine;
using System.Collections;

public class MSMiniJobHurtGoon : MonoBehaviour {

	[SerializeField]
	MSChatAvatar avatar;

	[SerializeField]
	UILabel name;

	[SerializeField]
	UILabel healthLost;

	public void Init(PZMonster monster, int damage)
	{
		name.text = monster.monster.shorterName;
		healthLost.text = "-"+damage+" HP";
		avatar.Init(monster.monster.monsterId);
		gameObject.SetActive(true);
	}
}
