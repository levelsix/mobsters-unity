using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Called a "chat table", but really just a hacky UITable
/// that aligns everything in one column without doing NGUI's stupid offsetting stuff
/// </summary>
public class MSChatTable : UITable {

	[ContextMenu ("Test")]
	public override void Reposition ()
	{
		base.Reposition ();

		SpringPosition spring;
		foreach (var item in GetChildList()) 
		{
			item.localPosition = new Vector3(0, item.localPosition.y, item.localPosition.z);
			spring = item.GetComponent<SpringPosition>();
			if (spring != null)
			{
				spring.target.x = 0;
			}
		}
	}
}
