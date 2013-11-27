using UnityEngine;
using System.Collections;

public class CBKGridItem : MonoBehaviour {

	public int height;
	public int width;

	Transform _trans;
	public Transform trans
	{
		get
		{
			if (_trans == null)
			{
				_trans = transform;
			}
			return _trans;
		}
	}

	GameObject _gObj;
	public GameObject gObj
	{
		get
		{
			if (_gObj == null)
			{
				_gObj = gameObject;
			}
			return _gObj;
		}
	}
}
