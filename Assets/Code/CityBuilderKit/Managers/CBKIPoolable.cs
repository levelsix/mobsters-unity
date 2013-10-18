using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface CBKIPoolable {
	
	CBKIPoolable prefab {get;set;}
	GameObject gObj {get;}
	Transform transf {get;}
	
	CBKIPoolable Make(Vector3 origin);
	
	void Pool();
	
}
