using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKTownBackground : MonoBehaviour {

	[SerializeField]
	CBKSpriteList backgroundSprites;

	[SerializeField]
	SpriteRenderer missionGround;

	[SerializeField]
	SpriteRenderer missionRoad;

	public string homeBackgroundSpriteName;

	public static float mapWidth = 0;
	public static float mapHeight = 0;

	public void InitHome()
	{
		InitMission(homeBackgroundSpriteName, "");
	}

	public void InitMission(FullCityProto city)
	{
		InitMission(city.mapImgName, city.roadImgName);
		missionRoad.transform.localPosition = new Vector3(city.center.x/25f, city.center.y/25f);
	}

	public void InitMission(string background, string road)
	{
		missionGround.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(background));
		missionRoad.sprite = backgroundSprites.GetSprite(CBKUtil.StripExtensions(road));

		mapWidth = missionGround.sprite.bounds.extents.x;
		mapHeight = missionGround.sprite.bounds.extents.y;
	}

}
