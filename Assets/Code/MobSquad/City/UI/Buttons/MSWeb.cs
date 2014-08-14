using UnityEngine;
using System.Collections;

public class MSWeb : MonoBehaviour {

	public string URL;

	public void OnClick()
	{
		Application.OpenURL(URL);
	}
}
