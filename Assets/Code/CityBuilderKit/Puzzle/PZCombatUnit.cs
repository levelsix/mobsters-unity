using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[RequireComponent (typeof(CBKUnit))]
public class PZCombatUnit : MonoBehaviour {
	
	public CBKUnit unit;
	
	public MonsterProto proto;
	
	void Awake()
	{
		unit = GetComponent<CBKUnit>();
	}
	
	void Init(MonsterProto proto)
	{
		this.proto = proto;
	}
	
	//TODO: Init for FullUserMonsterProto
}
