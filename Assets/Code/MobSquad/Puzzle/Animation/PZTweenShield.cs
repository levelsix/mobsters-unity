using UnityEngine;
using System.Collections;

public class PZTweenShield : MonoBehaviour {

	[SerializeField] TweenScale shieldIn;
	[SerializeField] TweenScale shieldIdle;
	[SerializeField] TweenScale shieldBounce;
	[SerializeField] TweenScale shieldDestroy;

	[SerializeField] SpriteRenderer shieldBack;
	[SerializeField] SpriteRenderer shieldFront;
	[SerializeField] SpriteRenderer shieldGlow;

	[SerializeField] AnimationCurve alphaFadeCurve;

	float alpha
	{
		set
		{
			shieldBack.color = new Color(shieldBack.color.r, shieldBack.color.g, shieldBack.color.b, value);
			shieldFront.color = new Color(shieldFront.color.r, shieldFront.color.g, shieldFront.color.b, value);
			shieldGlow.color = new Color(shieldGlow.color.r, shieldGlow.color.g, shieldGlow.color.b, value);
		}
	}

	public Coroutine BringIn()
	{
		alpha = 1;
		gameObject.SetActive(true);
		shieldIn.enabled = shieldIdle.enabled = shieldBounce.enabled = shieldDestroy.enabled = false;
		return StartCoroutine(In());
	}

	IEnumerator In()
	{
		shieldIn.ResetToBeginning();
		shieldIn.PlayForward();
		while (shieldIn.tweenFactor < 1) yield return null;
		shieldIdle.Sample(.5f, false);
		shieldIdle.PlayForward();
	}

	public Coroutine BounceShield()
	{
		return StartCoroutine(Bounce());
	}

	IEnumerator Bounce()
	{
		shieldBounce.from = shieldIdle.value;
		shieldIdle.enabled = false;
		shieldBounce.ResetToBeginning();
		shieldBounce.PlayForward();
		while (shieldBounce.tweenFactor < 1) yield return null;
		shieldBounce.PlayReverse();
		while (shieldBounce.tweenFactor > 0) yield return null;
		shieldIdle.enabled = true;
	}

	public Coroutine DestroyShield()
	{
		return StartCoroutine(Destroy ());
	}

	IEnumerator Destroy()
	{
		shieldDestroy.from = shieldIdle.value;
		shieldIdle.enabled = false;
		shieldDestroy.ResetToBeginning();
		shieldDestroy.PlayForward();
		while (shieldDestroy.tweenFactor < 1)
		{
			alpha = 1 - shieldDestroy.tweenFactor;
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
