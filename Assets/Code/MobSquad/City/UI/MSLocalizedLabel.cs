using UnityEngine;
using System.Collections;
using GoogleFu;

[RequireComponent (typeof (UILabel))]
public class MSLocalizedLabel : MonoBehaviour 
{
	public Sheet1.rowIds stringId;

	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		label.text = MSLocalization.GetString(stringId);
	}
}
