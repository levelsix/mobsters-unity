using UnityEngine;
using System.Collections;

public class PZShieldLabel : MonoBehaviour {

	[SerializeField] UILabel label;
	[SerializeField] TweenPosition tPos;
	[SerializeField] TweenAlpha tAlph;

	public void Set(int amt)
	{
		label.text = amt.ToString();
		tPos.Sample(0, false);
		tAlph.Sample(0, false);
		tPos.PlayForward();
		tAlph.PlayForward();
	}
}
