using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Pool Manager, which keeps track of all unused objects
/// which we want to recycle instead of delete and reinstantiate
/// </summary>
public class CBKPoolManager : MonoBehaviour {
	
	public static CBKPoolManager instance;
	
	/// <summary>
	/// A dictionary that maps prefabs to their respective pools
	/// </summary>
	public Dictionary<CBKIPoolable, List<CBKIPoolable>> pools;
	
	/// <summary>
	/// Awake this instance.
	/// Create the pool dictionary and set the manager reference
	/// </summary>
	void Awake()
	{
		instance = this;
		pools = new Dictionary<CBKIPoolable, List<CBKIPoolable>>();
		DontDestroyOnLoad(gameObject);
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
	public CBKIPoolable Get(CBKIPoolable prefab, Vector3 pos)
	{
		CBKIPoolable pooled;
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
		pooled.transf.parent = null;
		pooled.gObj.SetActive(true);
		
		return pooled;
		
	}
	
	/// <summary>
	/// Pool the specified poolable object.
	/// </summary>
	/// <param name='pooled'>
	/// Instance of an object to pool
	/// </param>
	public void Pool(CBKIPoolable pooled)
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
			pools[pooled.prefab] = new List<CBKIPoolable>();
		}
		
		//Add it to the pool
		pools[pooled.prefab].Add(pooled);
		pooled.transf.parent = transform;
	}
	
	/// <summary>
	/// "Warm" the specified poolable by creating it
	/// and immediately pooling it, insuring that it will
	/// be around for later use.
	/// </summary>
	/// <param name='poolable'>
	/// Poolable.
	/// </param>
	public void Warm(CBKIPoolable poolable)
	{
		CBKIPoolable obj = poolable.Make(Vector3.zero);
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
	public void Warm(CBKIPoolable poolable, int count)
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
		foreach (CBKIPoolable item in pools.Keys) 
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
	public void Clean(CBKIPoolable prefab)
	{
		foreach (CBKIPoolable item in pools[prefab])
		{
			Destroy(item.gObj);
		}
	}
}
