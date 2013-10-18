using UnityEngine;
using System.Collections;

public class CBKPrefabList : MonoBehaviour {

	public static CBKPrefabList instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public CBKMoneyPickup moneyPrefab;
	
}
