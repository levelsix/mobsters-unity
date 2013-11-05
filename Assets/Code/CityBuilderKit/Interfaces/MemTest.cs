using UnityEngine;
using System.Collections;

public class MemTest : MonoBehaviour {

	[SerializeField]
	string[] xmls;
	
	int index;
	
	void Start()
	{
		foreach (string item in xmls) {
			CBKAtlasUtil.instance.WarmAtlasDictionaryFromXML(item);
		}
	}
	
}
