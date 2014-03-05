using UnityEngine;
using System.Collections;

public class PZPlane : MonoBehaviour {

	Transform trans;

	CBKSimplePoolable pool;

	const float Y_ABOVE_SCREEN_THRESH = 100;

	[SerializeField]
	float speed;

	float maxY
	{
		get
		{
			return Screen.height/2 + Y_ABOVE_SCREEN_THRESH;
		}
	}

	void Awake()
	{
		pool = GetComponent<CBKSimplePoolable>();
		trans = transform;
	}

	void OnEnable()
	{
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.plane);
	}

	void Update () 
	{
		trans.localPosition += speed * Time.deltaTime * -PZScrollingBackground.instance.direction;
		if (trans.localPosition.y > maxY)
		{
			pool.Pool();
		}
	}
}
