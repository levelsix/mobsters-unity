using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Pool Manager, which keeps track of all unused objects
/// which we want to recycle instead of delete and reinstantiate
/// </summary>
public class MSPoolManager : MonoBehaviour {
	
	public static MSPoolManager instance;
	
	/// <summary>
	/// A dictionary that maps prefabs to their respective pools
	/// </summary>
	private Dictionary<MSPoolable, List<MSPoolable>> pools;
	
	/// <summary>
	/// Awake this instance.
	/// Create the pool dictionary and set the manager reference
	/// </summary>
	void Awake()
	{
		instance = this;
		pools = new Dictionary<MSPoolable, List<MSPoolable>>();
		DontDestroyOnLoad(gameObject);
	}

	public MSSimplePoolable Get(MonoBehaviour prefab, Transform parent = null)
	{
		return Get(prefab.GetComponent<MSSimplePoolable>(), Vector3.zero, parent) as MSSimplePoolable;
	}

	public MSSimplePoolable Get(MSSimplePoolable prefab, Transform parent = null)
	{
		return Get (prefab, Vector3.zero, parent) as MSSimplePoolable;
	}

	public T Get<T>(MonoBehaviour prefab, Transform parent = null) where T : MonoBehaviour
	{
		return (Get (prefab.GetComponent<MSSimplePoolable>(), Vector3.zero, parent) as MSSimplePoolable).GetComponent<T>();
	}

	public MSPoolable Get(MSPoolable prefab, Transform parent = null)
	{
		return Get(prefab, Vector3.zero, parent);
	}
	
	/// <summary>
	/// Get the specified prefab at pos.
	/// If there is a pooled instance, re-enables it and returns it
	/// Otherwise, creates a new instance
	/// </summary>
	/// <param name='prefab'>
	/// Prefab of which to get a newly enabled copy
	/// </param>
	/// <param name='pos'>
	/// Position at which to move the object to
	/// </param>
	public MSPoolable Get(MSPoolable prefab, Vector3 pos, Transform parent = null)
	{
		MSPoolable pooled;
		if(pools.ContainsKey(prefab) && pools[prefab].Count > 0)
		{
			//Get from existing pool
			pooled = pools[prefab][0];
			pools[prefab].RemoveAt(0);
			
			pooled.transf.position = pos;
		}
		else
		{
			//Create new object
			pooled = prefab.Make(pos);
		}
		
		//Set it back up.
		pooled.transf.parent = parent;
		if (parent != null)
		{
			pooled.gObj.layer = parent.gameObject.layer;
		}
		pooled.gObj.SetActive(true);
		
		return pooled;
		
	}
	
	/// <summary>
	/// Pool the specified poolable object.
	/// </summary>
	/// <param name='pooled'>
	/// Instance of an object to pool
	/// </param>
	public void Pool(MSPoolable pooled)
	{
		//Disable the poolable
		pooled.gObj.SetActive(false);
		
		//Short the function if this object isn't set up to pool; we'll just
		//leave it disabled until the scene change
		if (pooled.prefab == null)
		{
			return;
		}
		
		//If no pool, make a new pool
		if (!pools.ContainsKey(pooled.prefab))
		{
			pools[pooled.prefab] = new List<MSPoolable>();
		}
		
		//Add it to the pool
		pools[pooled.prefab].Add(pooled);
		pooled.transf.parent = transform;
	}

	public void Warm(MonoBehaviour poolable)
	{
		MSSimplePoolable simPo = poolable.GetComponent<MSSimplePoolable>();
		if (simPo == null)
		{
			Debug.LogError("Trying to warm " + poolable.name + " but it isn't poolable.");
			return;
		}
		Pool(simPo.Make(Vector3.zero));
	}

	public void Warm(MonoBehaviour poolable, int count)
	{
		for (int i = 0; i < count; i++) 
		{
			Warm (poolable);
		}
	}
	
	/// <summary>
	/// "Warm" the specified poolable by creating it
	/// and immediately pooling it, insuring that it will
	/// be around for later use.
	/// </summary>
	/// <param name='poolable'>
	/// Poolable.
	/// </param>
	public void Warm(MSPoolable poolable)
	{
		MSPoolable obj = poolable.Make(Vector3.zero);
		Pool(obj);
	}
	
	/// <summary>
	/// Warm the specified poolable the specified number of times.
	/// </summary>
	/// <param name='poolable'>
	/// Poolable prefab to be warmed.
	/// </param>
	/// <param name='count'>
	/// Count.
	/// </param>
	public void Warm(MSPoolable poolable, int count)
	{
		for (int i = 0; i < count; i++) 
		{
			Warm (poolable);
		}
	}
	
	/// <summary>
	/// Clean all of the pools
	/// </summary>
	public void Clean()
	{
		foreach (MSPoolable item in pools.Keys) 
		{
			Clean(item);
		}
	}
	
	/// <summary>
	/// Clean the specified prefab from the pool by deleting all
	/// references to pooled instances of it.
	/// </summary>
	/// <param name='prefab'>
	/// Prefab to clean.
	/// </param>
	public void Clean(MSPoolable prefab)
	{
		foreach (MSPoolable item in pools[prefab])
		{
			Destroy(item.gObj);
		}
	}
}
