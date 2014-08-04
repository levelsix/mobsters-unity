using UnityEngine;
using System.Collections;

public class PZForfeitWords : MonoBehaviour {

	UISprite forfeitSprite;

	TweenAlpha alpha;

	TweenScale scale;

	TweenPosition pos;

	Transform parent;

	Transform trans;

	float tweenDistance;

	void Awake()
	{
		forfeitSprite = GetComponent<UISprite>();
		trans = transform;
		parent = trans.parent;

		pos = GetComponent<TweenPosition>();
		scale = GetComponent<TweenScale>();
		alpha = GetComponent<TweenAlpha>();

		tweenDistance = pos.to.y;
	}

	/// <summary>
	/// Sets the origin of the forfeit sprite tween by moving its parent
	/// </summary>
	/// <param name="position">World space coordinates. NOT local</param>
	public void SetParentPosition(Vector3 position)
	{
		parent.position = position;
	}

	public IEnumerator Animate(bool successfulForfeit)
	{
		StopAllCoroutines();
		forfeitSprite.alpha = 0f;
		trans.localPosition = Vector3.zero;

		if (successfulForfeit) {
			forfeitSprite.spriteName = "runawaysuccess";
			forfeitSprite.MakePixelPerfect();
		} else {
			forfeitSprite.spriteName = "runawayfailed";
			forfeitSprite.MakePixelPerfect();
		}

		pos.ResetToBeginning();
		pos.PlayForward();

		scale.ResetToBeginning ();
		scale.PlayForward ();

		alpha.ResetToBeginning ();
		alpha.PlayForward ();

		if(!successfulForfeit)
		{
			yield return new WaitForSeconds(alpha.duration);
		}
	}

	/// <summary>
	/// depreciated
	/// </summary>
	/// <param name="successfulForfeit">If set to <c>true</c> successful forfeit.</param>
//	public IEnumerator Forfeit(bool successfulForfeit)
//	{
//		forfeitSprite.transform.localPosition = Vector3.zero;
//		
//		Transform oldParent = forfeitSprite.transform.parent;
//		forfeitSprite.transform.parent = transform.parent;
//		if (successfulForfeit) {
//			forfeitSprite.spriteName = "runawaysuccess";
//			forfeitSprite.MakePixelPerfect();
//		} else {
//			forfeitSprite.spriteName = "runawayfailed";
//			forfeitSprite.MakePixelPerfect();
//		}
//		
//		TweenAlpha alpha = forfeitSprite.GetComponent<TweenAlpha> ();
//		TweenScale scale = forfeitSprite.GetComponent<TweenScale> ();
//		
//		alpha.ResetToBeginning ();
//		alpha.PlayForward ();
//		scale.ResetToBeginning ();
//		scale.PlayForward ();
//		
//		Vector3 start = forfeitSprite.transform.localPosition;
//		
//		float currTime = 0f;
//		while (currTime < alpha.duration) {
//			forfeitSprite.transform.localPosition = new Vector3(start.x, start.y + (200f * (currTime/alpha.duration)), start.z);
//			currTime += Time.deltaTime;
//			yield return new WaitForEndOfFrame();
//		}
//		
//		forfeitSprite.transform.parent = oldParent;
//	}
}
