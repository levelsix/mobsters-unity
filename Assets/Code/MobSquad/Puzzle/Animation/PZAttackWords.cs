using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class PZAttackWords : MonoBehaviour {

	[SerializeField]
	Color hammerTimeTop;

	[SerializeField]
	Color hammerTimeBot;

	[SerializeField]
	Color cantTouchThisTop;

	[SerializeField]
	Color cantTouchThisBot;

	[SerializeField]
	Color ballinTop;

	[SerializeField]
	Color ballinBot;

	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}
	
	/// <summary>
	/// BALL SO HARD HNNNG
	/// </summary>
	public void Ballin()
	{
		label.applyGradient = true;
		label.text = "BALLIN'";
		label.gradientTop = ballinTop;
		label.gradientBottom = ballinBot;
	}

	/// <summary>
	/// PHYSICAL CONTACT IS PROHIBITED
	/// </summary>
	public void CantTouchThis()
	{
		label.applyGradient = true;
		label.text = "CAN'T TOUCH THIS";
		label.gradientTop = cantTouchThisTop;
		label.gradientBottom = cantTouchThisBot;
	}

	/// <summary>
	/// HAMMERS THE TIME
	/// </summary>
	public void HammerTime()
	{
		label.applyGradient = true;
		label.text = "HAMMER TIME";
		label.gradientTop = hammerTimeTop;
		label.gradientBottom = hammerTimeBot;
	}

	/// <summary>
	/// ALL BOW TO THE RAIN GODS
	/// </summary>
	public void MakeItRain()
	{
		label.text = "MAKE IT RAIN";
		label.applyGradient = false;
	}
}
