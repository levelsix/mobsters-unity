using UnityEngine;
using System.Collections;

public class MSSetRenderQueue : MonoBehaviour {

	public int renderQueue = 3000;
	
	Material mMat;
	
	void Start ()
	{

		Material mMat;
		foreach (var ren in GetComponentsInChildren<Renderer>()) 
		{
			mMat = new Material(ren.sharedMaterial);
			mMat.renderQueue = renderQueue;
			ren.material = mMat;
		}

		Renderer rend;
		foreach (var psys in GetComponentsInChildren<ParticleSystem>()) 
		{
			rend = psys.renderer;
			mMat = new Material(rend.sharedMaterial);
			mMat.renderQueue = renderQueue;
			rend.material = mMat;
		}

		rend = renderer;
		if (rend == null)
		{
			ParticleSystem sys = GetComponent<ParticleSystem>();
			if (sys != null) rend = sys.renderer;
		}
		
		if (rend != null)
		{
			mMat = new Material(rend.sharedMaterial);
			mMat.renderQueue = renderQueue;
			rend.material = mMat;
		}
	}
	//void OnDestroy () { if (mMat != null) Destroy(mMat); }
}
