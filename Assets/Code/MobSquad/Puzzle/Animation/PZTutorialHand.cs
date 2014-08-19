using UnityEngine;
using System.Collections;

/// <summary>
/// PZ tutorial hand.
/// @author Rob Giusti
/// </summary>
[RequireComponent (typeof(TweenPosition))]
[RequireComponent (typeof(TweenAlpha))]
public class PZTutorialHand : MonoBehaviour 
{
	[HideInInspector] TweenPosition tweenPos;
	[HideInInspector] TweenAlpha tweenAlph;

	[SerializeField] Vector3 offset;

	void OnEnable()
	{
		MSActionManager.Puzzle.OnGemSwapSuccess += OnGemSwapSuccess;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnGemSwapSuccess -= OnGemSwapSuccess;
	}

	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
		tweenAlph = GetComponent<TweenAlpha>();
	}

	/// <summary>
	/// Init the specified startPos and endPos.
	/// </summary>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	public void Init(Vector3 startPos, Vector3 endPos)
	{
		gameObject.SetActive(true);

		tweenPos.from = startPos * PZPuzzleManager.SPACE_SIZE + offset;
		tweenPos.to = endPos * PZPuzzleManager.SPACE_SIZE + offset;

		tweenPos.ResetToBeginning();
		tweenAlph.ResetToBeginning();

		tweenPos.PlayForward();
		tweenAlph.PlayForward();
	}

	void OnGemSwapSuccess()
	{
		gameObject.SetActive(false);
	}

}
