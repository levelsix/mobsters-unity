using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSNaturalSortTable : UITable {

	protected override void Sort (List<Transform> list) { list.Sort(MSNaturalSortObject.CompareTrans); }
}
