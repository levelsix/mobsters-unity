using UnityEngine;
using System.Collections;

public class MSTutorialArrow : MonoBehaviour {

	public static MSTutorialArrow instance;

	public float tweenDistance = 15;

	public UISprite arrowSprite;

	void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

	public void Init(Transform parent, float baseDistance, MSValues.Direction direction)
	{
		arrowSprite.transform.localPosition = new Vector3(0, baseDistance + tweenDistance);
		TweenPosition twps = arrowSprite.GetComponent<TweenPosition>();

		twps.to = new Vector3(0, baseDistance - tweenDistance);
		twps.from = new Vector3(0, baseDistance + tweenDistance);
		twps.ResetToBeginning();
		twps.Play();

		switch (direction) 
		{
		case MSValues.Direction.EAST:
			transform.localRotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
			break;
		case MSValues.Direction.SOUTH:
			transform.localRotation = new Quaternion(0, 0, 1, 0);
			break;
		case MSValues.Direction.WEST:
			transform.localRotation = new Quaternion(0, 0, 0.7071068f, 0.7071068f);
			break;
		case MSValues.Direction.NORTH:
		default:
			transform.localRotation = Quaternion.identity;
			break;
		}

		transform.parent = parent;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;

		arrowSprite.gameObject.layer = gameObject.layer = parent.gameObject.layer;

		UIWidget parentWidget = parent.GetComponent<UIWidget>();
		if (parentWidget != null)
		{
			arrowSprite.depth = parentWidget.depth + 1;
		}

		arrowSprite.ParentHasChanged();

		gameObject.SetActive(true);
	}
}
