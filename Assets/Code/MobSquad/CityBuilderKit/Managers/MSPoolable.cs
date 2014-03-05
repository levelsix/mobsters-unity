using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface MSPoolable {
	
	MSPoolable prefab {get;set;}
	GameObject gObj {get;}
	Transform transf {get;}
	
	MSPoolable Make(Vector3 origin);
	
	void Pool();
	
}
