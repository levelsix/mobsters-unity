using UnityEngine;
using System.Collections;
using System;

public class CBKSimplePoolable : MonoBehaviour, CBKPoolable {
	
	GameObject gameObj;
	Transform trans;
	CBKSimplePoolable _prefab;
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	public Transform transf {
		get {
			return trans;
		}
	}
	public CBKPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKSimplePoolable;
		}
	}
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKSimplePoolable chunk = Instantiate(this, origin, Quaternion.identity) as CBKSimplePoolable;
		chunk.prefab = this;
		return chunk;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
}
