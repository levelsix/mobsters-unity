using UnityEngine;
using System.Collections;

public class MSCheckBox : MonoBehaviour {

	[SerializeField]
	UISprite CheckMark;

	public bool checkMarked;

	void OnEnable(){
		checkMarked = true;
		CheckMark.gameObject.SetActive (true);
	}

	public void OnClick(){
		if (checkMarked) {
			checkMarked = false;
			CheckMark.gameObject.SetActive(false);
		}
		else
		{
			checkMarked = true;
			CheckMark.gameObject.SetActive(true);
		}
	}
}
