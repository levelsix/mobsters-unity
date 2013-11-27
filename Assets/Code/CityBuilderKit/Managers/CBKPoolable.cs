using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface CBKPoolable {
	
	CBKPoolable prefab {get;set;}
	GameObject gObj {get;}
	Transform transf {get;}
	
	CBKPoolable Make(Vector3 origin);
	
	void Pool();
	
}
