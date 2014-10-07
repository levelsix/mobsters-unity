using UnityEngine;
using System.Collections;

public class QuickTest : MonoBehaviour {


	void Awake()
	{
		Debug.Log("Awake");
	}

	void OnEnable()
	{
		Debug.Log("On Enable");
	}

	void OnDisable()
	{
		Debug.Log("On Disable");
	}

	void Start () 
	{
		Debug.Log("Start");
	}

	void Update () 
	{
		Debug.Log("Update");
	}
}
