using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Serialization;
using UnityEngine;

//Do not add this script to your own classes! This is created internally
[AddComponentMenu("Storage/Internal/Level Loader (Internal use only, do not add this to your objects!)")]
/// <summary>
/// Level loader - used to load back in data
/// </summary>
public class LevelLoader : MonoBehaviour
{
    #region Delegates

	/// <summary>
	/// Delegate used when creating an object as part of deserialization
	/// </summary>
    public delegate void CreateObjectDelegate(GameObject prefab, ref bool cancel);
	/// <summary>
	/// Delegate used when deserializing a component
	/// </summary>
    public delegate void SerializedComponentDelegate(GameObject gameObject, string componentName, ref bool cancel);
	/// <summary>
	/// Delegate used when deserializing an object
	/// </summary>
    public delegate void SerializedObjectDelegate(GameObject gameObject, ref bool cancel);

    #endregion

	/// <summary>
	/// The current LevelLoader
	/// </summary>
    public static LevelLoader Current;
    private static Texture2D _pixel;
    private readonly Dictionary<string, int> _indexDictionary = new Dictionary<string, int>();
	/// <summary>
	/// The Level data that will be loaded
	/// </summary>
    public LevelSerializer.LevelData Data;
	/// <summary>
	/// Whether items that aren't in the loaded data should be deleted
	/// </summary>
	public bool DontDelete = false;
		/// <summary>
	/// The last game object to be loaded (the root of the deserialization).  This is
	/// useful in onComplete<LevelLoader> events
	/// </summary>
    public GameObject Last;
    private float _alpha = 1;
    private bool _loading = true;
    public bool showGUI = true;
	/// <summary>
	/// The time scale after loading.
	/// </summary>
    public float timeScaleAfterLoading = 1;
	public Action<List<GameObject>> whenCompleted = delegate { };

	/// <summary>
	/// Occurs when creating game object to allow cancellation
	/// </summary>
    public static event CreateObjectDelegate CreateGameObject = delegate { };
	/// <summary>
	/// Occurs when on destroying an object to allow cancellation
	/// </summary>
    public static event SerializedObjectDelegate OnDestroyObject = delegate { };
	/// <summary>
	/// Occurs when loading data for an object
	/// </summary>
    public static event SerializedObjectDelegate LoadData = delegate { };
	/// <summary>
	/// Occurs before loading a component for an object to allow cancellation
	/// </summary>
    public static event SerializedComponentDelegate LoadComponent = delegate { };
	/// <summary>
	/// Occurs after loading a component for an object
	/// </summary>
    public static event Action<Component> LoadedComponent = delegate { };


    private void Awake()
    {
        timeScaleAfterLoading = Time.timeScale;
        Current = this;
        if (_pixel == null)
        {
            _pixel = new Texture2D(1, 1);
        }
    }

    private void OnGUI()
    {
        if (!showGUI)
        {
            return;
        }
        if (!_loading && Event.current.type == EventType.repaint)
        {
            _alpha = Mathf.Clamp01(_alpha - 0.02f);
        }
        else if (Math.Abs(_alpha - 0) < float.Epsilon)
        {
            Destroy(gameObject);
        }
        if (!(Math.Abs(_alpha - 0) > float.Epsilon))
        {
            return;
        }
        _pixel.SetPixel(0, 0, new Color(1, 1, 1, _alpha));
        _pixel.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _pixel, ScaleMode.StretchToFill);
    }

    bool wasLoaded;

    private void OnLevelWasLoaded(int level)
    {
		if(wasLoaded)
			return;
		wasLoaded = true;
        timeScaleAfterLoading = Time.timeScale;
        StartCoroutine(Load());
    }


    private static void SetActive(GameObject go, bool activate)
    {
        // go.active = activate;
        go.SetActive(activate);
        foreach (var c in go.transform.Cast<Transform>())
        {
            if (c.GetComponent<StoreInformation>() == null)
            {
                SetActive(c.gameObject, activate);
            }
        }
    }

	/// <summary>
	/// Starts the loading process
	/// </summary>
    public IEnumerator Load()
    {
        yield return StartCoroutine(Load(2));
    }

	/// <summary>
	/// Starts the loading process
	/// </summary>
	/// <param name='numberOfFrames'>
	/// Waits for this many frames before commencing
	/// </param>
	/// <param name='timeScale'>
	/// The timescale to use while loading
	/// </param>
    public IEnumerator Load(int numberOfFrames, float timeScale = 0)
    {
		var oldFixedTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime = 9;
        //Need to wait while the base level is prepared, it takes 2 frames
        while (numberOfFrames-- > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        if (LevelSerializer.ShouldCollect && timeScale ==0)
        {
            GC.Collect();
        }

        LevelSerializer.RaiseProgress("Initializing", 0);

        //Check if we should be deleting missing items
        if (!DontDelete)
        {
            //First step is to remove any items that should not exist according to the saved scene
            foreach (var go in
                UniqueIdentifier.AllIdentifiers.Where(n => Data.StoredObjectNames.All(sn => sn.Name != n.Id)).ToList())
            {
                try
                {
                    var cancel = false;
                    OnDestroyObject(go.gameObject, ref cancel);
                    if (!cancel)
                    {
                        Destroy(go.gameObject);
                    }
                }
                catch (Exception e)
                {
                    Radical.LogWarning("Problem destroying object " + go.name + " " + e.ToString());
                }
            }
        }

		var flaggedObjects = new List<UniqueIdentifier>();

        LevelSerializer.RaiseProgress("Initializing", 0.25f);

        var position = new Vector3(0, 2000, 2000);
        //Next we need to instantiate any items that are needed by the stored scene
        foreach (var sto in
            Data.StoredObjectNames.Where(c => UniqueIdentifier.GetByName(c.Name) == null))// == null && !string.IsNullOrEmpty(c.ClassId) || c.createEmptyObject))
        {
            try
            {
				GameObject pf = null;
				LevelSerializer.AllPrefabs.TryGetValue(sto.ClassId, out pf);

                if (sto.createEmptyObject || pf == null)
                {
                    sto.GameObject = new GameObject("CreatedObject");
                    sto.GameObject.transform.position = position;
					var emptyObjectMarker = sto.GameObject.AddComponent<EmptyObjectIdentifier>();
					emptyObjectMarker.Id = sto.Name;
                }
                else
                {
                 	var cancel = false;
                    CreateGameObject(pf, ref cancel);
                    if (cancel)
                    {
                        continue;
                    }

					var uis = pf.GetAllComponentsInChildren<UniqueIdentifier>();
					foreach(var ui in uis)
						ui.IsDeserializing = true;
					sto.GameObject = Instantiate(pf, position, Quaternion.identity) as GameObject;
					foreach(var ui in uis)
						ui.IsDeserializing = false;
					flaggedObjects.AddRange(sto.GameObject.GetAllComponentsInChildren<UniqueIdentifier>());
                }

                position += Vector3.right*50;
                sto.GameObject.GetComponent<UniqueIdentifier>().Id = sto.Name;
                sto.GameObject.name = sto.GameObjectName;
                if (sto.ChildIds.Count > 0)
                {
                    var list = sto.GameObject.GetComponentsInChildren<UniqueIdentifier>().ToList();
                    for (var i = 0; i < list.Count && i < sto.ChildIds.Count; i++)
                    {
                        list[i].Id = sto.ChildIds[i];
                    }
                }
                if (sto.Children.Count > 0)
                {
                    var list = LevelSerializer.GetComponentsInChildrenWithClause(sto.GameObject);
                    _indexDictionary.Clear();
                    foreach (var c in list)
                    {
                        if (!sto.Children.ContainsKey(c.ClassId))
                        {
                            continue;
                        }
                        if (!_indexDictionary.ContainsKey(c.ClassId))
                        {
                            _indexDictionary[c.ClassId] = 0;
                        }
                        c.Id = sto.Children[c.ClassId][_indexDictionary[c.ClassId]];
                        _indexDictionary[c.ClassId] = _indexDictionary[c.ClassId] + 1;
                    }
                }
            }
            catch (Exception e)
            {
                Radical.LogWarning("Problem creating an object " + sto.GameObjectName + " with classID " + sto.ClassId + " " + e);
            }
        }
        var loadedGameObjects = new HashSet<GameObject>();

        LevelSerializer.RaiseProgress("Initializing", 0.75f);


        foreach (var so in Data.StoredObjectNames)
        {
            var go = UniqueIdentifier.GetByName(so.Name);
            if (go == null)
            {
                Radical.LogNow("Could not find " + so.GameObjectName + " " + so.Name);
            }
            else
            {
				var uis = go.GetAllComponentsInChildren<UniqueIdentifier>();
				foreach(var ui in uis)
					ui.IsDeserializing = true;
				flaggedObjects.AddRange(uis);

                loadedGameObjects.Add(go);
                if (so.Components != null && so.Components.Count > 0)
                {
                    var all = go.GetComponents<Component>().ToList();
					var store = go.GetComponent<StoreInformation>();
                    foreach (var comp in all)
                    {
                        if (!so.Components.ContainsKey(comp.GetType().AssemblyQualifiedName) && !so.Components.ContainsKey(comp.GetType().FullName) && (store == null || store.StoreAllComponents))
                        {
                            Destroy(comp);
                        }
                    }
                }
                SetActive(go, so.Active);
            }
        }

        LevelSerializer.RaiseProgress("Initializing", 0.85f);


        foreach (var go in Data.StoredObjectNames.Where(c => !string.IsNullOrEmpty(c.ParentName)))
        {
            var parent = UniqueIdentifier.GetByName(go.ParentName);
            var item = UniqueIdentifier.GetByName(go.Name);
            if (item != null && parent != null)
            {
                item.transform.parent = parent.transform;
            }
        }


        //Newly created objects should have the time to start
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Time.timeScale = timeScale;


        LevelSerializer.RaiseProgress("Initializing", 1f);


        using (new Radical.Logging())
        {
            var currentProgress = 0;

            using (new UnitySerializer.SerializationScope())
            {
                //Now we restore the data for the items
                foreach (var item in
                    Data.StoredItems.GroupBy(i => i.Name,
                                             (name, cps) => new
                                                                {
                                                                    Name = name,
                                                                    Components = cps.Where(cp => cp.Name == name).GroupBy(cp => cp.Type,
                                                                                                                          (type, components) => new
                                                                                                                                                    {
                                                                                                                                                        Type = type,
                                                                                                                                                        List = components.ToList()
                                                                                                                                                    }).ToList()
                                                                }))
                {
#if US_LOGGING
					Radical.Log ("\n*****************\n{0}\n********START**********\n", item.Name);
					Radical.IndentLog ();
#endif
                    var go = UniqueIdentifier.GetByName(item.Name);
                    if (go == null)
                    {
                        Radical.LogWarning(item.Name + " was null");
                        continue;
                    }
					var sendStartup = false;

                    foreach (var cp in item.Components)
                    {
                        try
                        {
                            LevelSerializer.RaiseProgress("Loading", (float) ++currentProgress/(float) Data.StoredItems.Count);
                            var type = UnitySerializer.GetTypeEx(cp.Type);
                            if (type == null)
                            {
                                continue;
                            }
                            Last = go;
                            var cancel = false;
                            LoadData(go, ref cancel);
                            LoadComponent(go, type.Name, ref cancel);
                            if (cancel)
                            {
                                continue;
                            }

#if US_LOGGING
								Radical.Log ("<{0}>\n", type.FullName);
								Radical.IndentLog ();
#endif

                            var list = go.GetComponents(type).Where(c => c.GetType() == type).ToList();
                            //Make sure the lists are the same length
                            while (list.Count > cp.List.Count)
                            {
                                DestroyImmediate(list.Last());
                                list.Remove(list.Last());
                            }
                            if (type == typeof (NavMeshAgent))
                            {
                                var cp1 = cp;
                                var item1 = item;
                                Action perform = () =>
                                                     {
                                                         var comp = cp1;
                                                         var tp = type;
                                                         var tname = item1.Name;
                                                         UnitySerializer.AddFinalAction(() =>
                                                                                            {
                                                                                                var g = UniqueIdentifier.GetByName(tname);
                                                                                                var nlist = g.GetComponents(tp).Where(c => c.GetType() == tp).ToList();
                                                                                                while (nlist.Count < comp.List.Count)
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        nlist.Add(g.AddComponent(tp));
                                                                                                    }
                                                                                                    catch
                                                                                                    {
                                                                                                    }
                                                                                                }
                                                                                                list = list.Where(l => l != null).ToList();
                                                                                                //Now deserialize the items back in
                                                                                                for (var i = 0; i < nlist.Count; i++)
                                                                                                {
                                                                                                    if (LevelSerializer.CustomSerializers.ContainsKey(tp))
                                                                                                    {
                                                                                                        LevelSerializer.CustomSerializers[tp].Deserialize(comp.List[i].Data, nlist[i]);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        UnitySerializer.DeserializeInto(comp.List[i].Data, nlist[i]);
                                                                                                    }
                                                                                                    LoadedComponent(nlist[i]);
                                                                                                }
                                                                                            });
                                                     };
                                perform();
                            }
                            else
                            {
                                while (list.Count < cp.List.Count)
                                {
                                    try
                                    {
#if US_LOGGING
										    Radical.Log("Adding component of type " + type.ToString());
#endif
                                        list.Add(go.AddComponent(type));
                                    }
                                    catch
                                    {
										sendStartup = true;
                                    }
                                }
                                list = list.Where(l => l != null).ToList();
                                //Now deserialize the items back in
                                for (var i = 0; i < list.Count; i++)
                                {
                                    Radical.Log(string.Format("Deserializing {0} for {1}", type.Name, go.GetFullName()));
                                    if (LevelSerializer.CustomSerializers.ContainsKey(type))
                                    {
                                        LevelSerializer.CustomSerializers[type].Deserialize(cp.List[i].Data, list[i]);
                                    }
                                    else
                                    {
                                        UnitySerializer.DeserializeInto(cp.List[i].Data, list[i]);
                                    }
                                    LoadedComponent(list[i]);
                                }
                            }
#if US_LOGGING
							    Radical.OutdentLog ();
							    Radical.Log ("</{0}>", type.FullName);
#endif
                        }
                        catch (Exception e)
                        {
                            Radical.LogWarning("Problem deserializing " + cp.Type + " for " + go.name + " " + e.ToString());
                        }
                    }

#if US_LOGGING
					Radical.OutdentLog ();
					Radical.Log ("\n*****************\n{0}\n********END**********\n\n", item.Name);
#endif
					if(sendStartup)
					{
						go.SendMessage("Awake");
						go.SendMessage("OnEnable");
					}
				}

				UnitySerializer.RunDeferredActions(1, false);

                Time.fixedDeltaTime = oldFixedTime;
				yield return new WaitForFixedUpdate();

				UnitySerializer.RunDeferredActions();

				//Finally we need to fixup any references to other game objects,
                //these have been stored in a list inside the serializer
                //waiting for us to call this.  Vector3s are also deferred until this point
				//UnitySerializer.RunDeferredActions(2);
                if (LevelSerializer.ShouldCollect && timeScale == 0)
                {
                	Resources.UnloadUnusedAssets();
                    GC.Collect();
                }

            	UnitySerializer.InformDeserializedObjects();


                //Tell the world that the level has been loaded
                LevelSerializer.InvokeDeserialized();
                whenCompleted(loadedGameObjects.ToList());
				//Flag that we aren't deserializing
				foreach(var obj in flaggedObjects)
				{
					obj.IsDeserializing = false;
				}

                LevelSerializer.IsDeserializing = false;
				_loading = false;
                RoomManager.loadingRoom = false;
                //Restore the time scale
                Loom.QueueOnMainThread(() => Time.timeScale = timeScaleAfterLoading);
                //Get rid of the current object that is holding this level loader, it was
                //created solely for the purpose of running this script
                Destroy(gameObject, 1.1f);
            }
        }
    }
}
