using UnityEngine;
using System.Collections;

public class CBKTownBackground : MonoBehaviour {

	[SerializeField]
	CBKSpriteList backgroundSprites;

	[SerializeField]
	GameObject homeParent;

	[SerializeField]
	GameObject missionParent;

	[SerializeField]
	SpriteRenderer missionGround;

	[SerializeField]
	SpriteRenderer missionRoad;

	public void InitHome()
	{
		homeParent.SetActive(true);
		missionParent.SetActive(false);
	}

	public void InitMission(string background, string road)
	{
		homeParent.SetActive (false);

		missionParent.SetActive(true);
		missionGround.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(background));
		missionRoad.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(road));
	}

}
