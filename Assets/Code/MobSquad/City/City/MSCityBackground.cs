using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSCityBackground : MonoBehaviour {

	[SerializeField]
	SpriteRenderer missionGround;

	public string homeBackgroundSpriteName;

	public static float mapWidth = 0;
	public static float mapHeight = 0;

	public void Awake()
	{
		InitMission(homeBackgroundSpriteName, "");
	}

	public void InitMission(string background, string road)
	{
		mapWidth = missionGround.sprite.bounds.extents.x;
		mapHeight = missionGround.sprite.bounds.extents.y;
	}

}
