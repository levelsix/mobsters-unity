using UnityEngine;
using System.Collections;

public class MSSetRenderQueue : MonoBehaviour {

	public int renderQueue = 3000;
	
	Material mMat;
	
	void Start ()
	{
		Renderer ren = renderer;
		
		if (ren == null)
		{
			ParticleSystem sys = GetComponent<ParticleSystem>();
			if (sys != null) ren = sys.renderer;
		}
		
		if (ren != null)
		{
			mMat = new Material(ren.sharedMaterial);
			mMat.renderQueue = renderQueue;
			ren.material = mMat;
		}
	}
	void OnDestroy () { if (mMat != null) Destroy(mMat); }
}
