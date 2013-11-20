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

	[SerializeField]
	SpriteRenderer homeGroundLeft;

	[SerializeField]
	SpriteRenderer homeGroundRight;

	public static float mapWidth = 0;
	public static float mapHeight = 0;

	public void InitHome()
	{
		homeParent.SetActive(true);
		missionParent.SetActive(false);
		mapWidth = homeGroundLeft.sprite.bounds.extents.x + homeGroundRight.sprite.bounds.extents.x;
		mapHeight = homeGroundLeft.sprite.bounds.extents.y;
	}

	public void InitMission(string background, string road)
	{
		homeParent.SetActive (false);

		missionParent.SetActive(true);
		missionGround.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(background));
		missionRoad.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(road));

		mapWidth = missionGround.sprite.bounds.extents.x;
		mapHeight = missionGround.sprite.bounds.extents.y;
	}

}
