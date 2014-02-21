using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class PZTurnsLeftCounter : MonoBehaviour {

	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	void OnEnable()
	{
		CBKEventManager.Puzzle.OnTurnChange += OnTurnChange;
	}

	void OnDisable()
	{
		CBKEventManager.Puzzle.OnTurnChange -= OnTurnChange;
	}

	void OnTurnChange(int turn)
	{
		label.text = turn.ToString();
	}
}
