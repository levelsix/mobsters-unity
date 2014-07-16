using UnityEngine;
using System.Collections;

public abstract class MSFunctionalScreen : MonoBehaviour
{
	public abstract bool IsAvailable();
	public abstract void Init();
}
