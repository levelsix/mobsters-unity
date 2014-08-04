using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UIInput))]
public class MSSelectInputOnEnable : MonoBehaviour {

	void OnEnable()
	{
		GetComponent<UIInput>().OnSelectEvent();
	}
}
