using UnityEngine;
using System.Collections;

//
// An Example class to show code manipulation of the IcoParticles object
//
public class IcoParticlesManagerExample : MonoBehaviour
{
	// Script instance variables
	private IcoParticles particleScript;
	private Material particleMaterial;
	
	// Default parameters for showing the particle system
	private string defaultEffect 	= "ParticleSystems/ConvertedPEXtoTXTfiles/RealPopcorn";
	private string defaultTexture	= "DifferentTexture/icoicon";
	private bool defaultLoop 		= true;
	private int defaultDelay 		= 1000;
	
	//
	// A function to change all the values of the existing particle system
	// to the values we choose
	//
	void Awake()
	{
		// Get references to the workings of the particle system
		particleScript = this.GetComponent<IcoParticles>();
		particleMaterial = this.renderer.material;
		
		// Set the properties to whatever we want
		SetEffect(defaultEffect);
		SetTexture(defaultTexture);
		SetLoop(defaultLoop);
		SetLoopDelay(defaultDelay);
	}
	
	//
	// Set effect to default example
	//
	private void SetEffect(string effect)
	{
		TextAsset effectObj = (TextAsset)Resources.Load(effect, typeof(TextAsset));
		particleScript.source = effectObj;
	}
	
	//
	// Set material texture in IcoMaterials script
	//
	private void SetTexture(string texture)
	{
		Texture tex = Resources.Load(texture) as Texture;
		particleMaterial.mainTexture = tex;
	}
	
	//
	// Set looping to true or false in IcoParticles script
	//
	private void SetLoop(bool looping)
	{
		particleScript.loop = looping;
	}
	
	//
	// Set looping dealy in IcoParticles script
	//
	private void SetLoopDelay(int delay)
	{
		particleScript.loopDelay = delay;
	}
}
