using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {

	[SerializeField]
	IcoParticles ico;

	[SerializeField]
	ParticleSystem sys;

	[SerializeField]
	bool sysOn;

	// Use this for initialization
	void Awake () {
		ico = GetComponent<IcoParticles>();
		sys = particleSystem;
	}
	
	// Update is called once per frame
	void Update () {
		sysOn = sys.isPlaying;
	}

	[ContextMenu ("Play")]
	void Play()
	{
		sys.Stop();
		sys.Play();
	}

	[ContextMenu ("Stop")]
	void Stop()
	{
		sys.Stop();
	}
}
