using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Control Manager, which handles mouse and touch input.
/// For touch, maintains a dictionary of active touches which
/// it checks up on every step.
/// </summary>
public class TCKControlManager : MonoBehaviour 
{
	public static TCKControlManager instance;

	[SerializeField]
	UILabel DebugLabel;
	
    /// <summary>
    /// Constant that Unity uses for left-click
    /// </summary>
    private const int LEFT_MOUSE = 0;
	
	/// <summary>
	/// The most touches we'll ever report as being part of a single
	/// multi-touch data
	/// </summary>
	public const int MAX_TOUCHES = 5;
	
	/// <summary>
	/// A dictionary of current touches, indexed by Touch.fingerId
	/// </summary>
	private Dictionary<int, TCKTouchData> touches;
	
	/// <summary>
	/// The user interface camera.
	/// Used to distinguish touches on UI from gameplay touches
	/// NEEDS to be set in editor
	/// </summary>
	[SerializeField]
	Camera uiCamera;
	
	/// <summary>
	/// The touch pile.
	/// A collection of touch datas that are no longer being used and can
	/// be recycled, to avoid allocating new data every touch.
	/// </summary>
	private List<TCKTouchData> touchPile;
	
	/// <summary>
	/// The touch data for a mouse.
	/// </summary>
	private TCKTouchData mouseData;
	
	/// <summary>
	/// The touch data for following a gesture involving multiple touches
	/// </summary>
	private TCKTouchData multiData;
	
	/// <summary>
	/// The last mouse position.
	/// For calculating DeltaMouse
	/// </summary>
	private Vector3 _lastMouse;
	
	/// <summary>
	/// Gets the delta mouse.
	/// </summary>
	/// <value>
	/// The change in mouse position since the previous frame
	/// </value>
	public Vector3 DeltaMouse{
		get
		{
			return Input.mousePosition - _lastMouse;
		}
	}
	
	/// <summary>
	/// The most recent tap.
	/// Set to null after DOUBLE_TAP_TIME
	/// </summary>
	private TCKTouchData recentTap;
	
	/// <summary>
	/// Constant for time between touches being lifted from
	/// a multitouch in order for them to be counted
	/// as a removal of a single touch
	/// </summary>
	const float MULTITOUCH_LIFT_TIME = .2f;
	
	/// <summary>
	/// Max time between taps for it to count as a double-tap
	/// </summary>
	const float DOUBLE_TAP_TIME = .8f;
	
	/// <summary>
	/// Max distance between taps for it to count as a double-tap
	/// </summary>
	const float DOULBE_TAP_DIST_SQR = 800f;
		
	/// <summary>
	/// Gets the avgerage of all current touches
	/// </summary>
	/// <value>
	/// The avgerage touch point
	/// </value>
	private Vector2 avgTouchPoint{
		get{
			Vector2 av = Vector2.zero;
			foreach (TCKTouchData item in touches.Values) 
			{
				av += item.pos;
			}
			av = av / touches.Count;
			return av;
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// Set up the touch dictionary
	/// </summary>
	void Awake()
	{
		touches = new Dictionary<int, TCKTouchData>();
		mouseData = new TCKTouchData(Vector2.zero);
		touchPile = new List<TCKTouchData>();

		instance = this;
		
		if (uiCamera == null)
		{
			Debug.LogError("UICamera NEEDS TO BE SET IN EDITOR");
		}
	}
	
	/// <summary>
    /// Update is called once per frame
	/// </summary>
	void Update () 
	{
#if UNITY_EDITOR
		ProcessMouse();
#else
		ProcessTouches();
#endif
	}
	
	/// <summary>
	/// Gets a touch data from the pool or new if necessary
	/// Inits with pos
	/// </summary>
	/// <returns>
	/// The touch data ref
	/// </returns>
	/// <param name='pos'>
	/// Position to initialize with
	/// </param>
	private TCKTouchData GetTouch(Vector2 pos)
	{
		if (touchPile.Count > 0)
		{
			Debug.Log("Recycled tap");
			TCKTouchData temp = touchPile[0];
			touchPile.RemoveAt(0);
			temp.init(pos);
			return temp;
		}
		else
		{
			Debug.Log("New tap");
			return new TCKTouchData(pos);
		}
	}
	
	/// <summary>
	/// Pools the touch data
	/// </summary>
	/// <param name='data'>
	/// Data ref
	/// </param>
	private void PoolTouch(TCKTouchData data)
	{
		touchPile.Add(data);
	}
	
	/// <summary>
	/// Processes all current touches and .
	/// </summary>  
	private void ProcessTouches()
	{
		foreach (Touch touch in Input.touches) 
		{
			//Add new touches to the dictionary
			if (touch.phase == TouchPhase.Began)
			{
				touches[touch.fingerId] = GetTouch(touch.position);
				touches[touch.fingerId].id = touch.fingerId;
				touches[touch.fingerId].ui = HitsUI(touch.position);				
			}
			
			//Remove all ended touches in the dictionary
			if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touches.ContainsKey(touch.fingerId))
			{
				//Process tap/flick
				ProcessRelease(touches[touch.fingerId]);
				if (touches[touch.fingerId] != recentTap)
				{
					PoolTouch (touches[touch.fingerId]);
				}
				touches.Remove(touch.fingerId);
			}
			//If it's not an ending touch, update it and possibly process it as a hold
			else
			{
				touches[touch.fingerId].pos = touch.position;
				touches[touch.fingerId].delta = touch.deltaPosition;
				//UpdateTouch(touches[touch.fingerId]);
			}
		}
		if (touches.Count > 1)
		{
			Debug.Log("Multi");
			UpdateMultiTouch(touches.Count);
		}
		else 
		{
			foreach (TCKTouchData item in touches.Values) 
			{
				//Debug.Log("Single");
				UpdateTouch(item);
			}
			if (multiData != null)
			{
				//Maybe we do some release stuff here for the multidata?
				PoolTouch(multiData);
				multiData = null;
			}
		}
		/*
		string str = "";
		foreach (KeyValuePair<int, TCKTouchData> item in touches) {
			str += "touch[" + item.Key + "]: " + item.Value.ToString() + "\n";
		}
		DebugLabel.text = str;
		*/
	}

    /// <summary>
    /// Process mouse click for selection and movement
    /// </summary>
    private void ProcessMouse()
    {
        //Checks left-click
        if (Input.GetMouseButtonDown(LEFT_MOUSE))
        {
			mouseData.init(Input.mousePosition);
			mouseData.ui = HitsUI(Input.mousePosition);
        }
        else if (Input.GetMouseButton(LEFT_MOUSE))
        {
			mouseData.delta = DeltaMouse;
            mouseData.pos = Input.mousePosition;
			UpdateTouch(mouseData);
        }
        else if (Input.GetMouseButtonUp(LEFT_MOUSE))
        {
            ProcessRelease(mouseData);
        }
		_lastMouse = Input.mousePosition;
    }
	
	/// <summary>
	/// Processes a single hold.
	/// </summary>
	/// <param name='touch'>
	/// The touch data of the hold
	/// </param>
	private void ProcessHold(TCKTouchData touch)
	{
		if (!touch.stationary)
		{
			if (CBKEventManager.Controls.OnKeepDrag[touch.countIndex] != null)
			{
				CBKEventManager.Controls.OnKeepDrag[touch.countIndex](touch);
			}
		}
		else
		{
			if (CBKEventManager.Controls.OnKeepHold[touch.countIndex] != null)
			{
				CBKEventManager.Controls.OnKeepHold[touch.countIndex](touch);
			}
		}
	}
	
	/// <summary>
	/// Processes the release of a touch. If it's a tap of flick, this
	/// is when the event is called
	/// </summary>
	/// <param name='touch'>
	/// Touch being released.
	/// </param>
	private void ProcessRelease(TCKTouchData touch)
	{
		if (touch.ui) //Ignore UI clicks
		{
			return;
		}
		if (touch.phase == TCKTouchData.Phase.TAP)
		{
			if (!touch.stationary)
			{
				if (CBKEventManager.Controls.OnFlick[touch.countIndex] != null)
				{
					CBKEventManager.Controls.OnFlick[touch.countIndex](touch);
				}
			}
			else
			{
				
				//Try to double-tap
				if (CheckDoubleTap(touch) && CBKEventManager.Controls.OnDoubleTap[touch.countIndex] != null)
				{
					CBKEventManager.Controls.OnDoubleTap[touch.countIndex](touch);
				}
				//Tap
				else if (CBKEventManager.Controls.OnTap[touch.countIndex] != null)
				{
					CBKEventManager.Controls.OnTap[touch.countIndex](touch);
				}
				StartCoroutine(HoldTap(touch));
			}
		}
		else
		{
			if (!touch.stationary)
			{
				if (CBKEventManager.Controls.OnReleaseDrag[touch.countIndex] != null)
				{
					CBKEventManager.Controls.OnReleaseDrag[touch.countIndex](touch);
				}
			}
			else
			{
				if (CBKEventManager.Controls.OnReleaseHold[touch.countIndex] != null)
				{
					CBKEventManager.Controls.OnReleaseHold[touch.countIndex](touch);
				}
			}
		}		
	}
	
	/// <summary>
	/// Updates a touch.
	/// </summary>
	/// <param name='touch'>
	/// Touch to be updated
	/// </param>
	private void UpdateTouch(TCKTouchData touch)
	{
		if (touch.ui) //Ignore UI clicks
		{
			return;
		}
		if (touch.phase == TCKTouchData.Phase.TAP)
		{
			touch.Update(Time.deltaTime);
			if (touch.phase == TCKTouchData.Phase.HOLD)
			{
				if (touch.stationary)
				{
					//Separate 'if' for clarity
					if (CBKEventManager.Controls.OnStartHold[touch.countIndex] != null)
					{
						CBKEventManager.Controls.OnStartHold[touch.countIndex](touch);
					}
				}
				ProcessHold(touch);
			}
		}
		else
		{
			touch.Update(Time.deltaTime);
			ProcessHold(touch);	
		}
	}
	
	/// <summary>
	/// After a tap is released, holds onto that tap
	/// for DOUBLE_TAP_TIME
	/// </summary>
	/// <param name='data'>
	/// Data for the tap
	/// </param>
	IEnumerator HoldTap(TCKTouchData data)
	{
		recentTap = data;
		yield return new WaitForSeconds(DOUBLE_TAP_TIME);
		PoolTouch(data);
		if (recentTap == data)
		{
			recentTap = null;
		}
		if (multiData != null)
		{
			multiData.count = Mathf.Min(touches.Count, MAX_TOUCHES); //Caps reported touches at MAX_TOUCHES
			if (multiData.count <= 1)
			{
				PoolTouch(multiData);
				multiData = null;
			}
		}
	}
	
	/// <summary>
	/// Checks whether a given tap is a double tap.
	/// </summary>
	/// <returns>
	/// True if it is a double tap.
	/// </returns>
	/// <param name='data'>
	/// Touch to check
	/// </param>
	private bool CheckDoubleTap(TCKTouchData data)
	{
		return recentTap != null && (data.pos - recentTap.pos).sqrMagnitude < DOULBE_TAP_DIST_SQR;
	}
	
	/// <summary>
	/// If we have multiple touches on the screen, process
	/// them differently
	/// </summary>
	private void UpdateMultiTouch(int count)
	{
		if (multiData == null)
		{
			multiData = GetTouch(avgTouchPoint);
			multiData.size = GetSize();
			multiData.count = count;
		}
		else
		{
			Vector3 sum = Vector3.zero;
			foreach (TCKTouchData item in touches.Values) 
			{
				sum += item.delta;
			}
			multiData.delta = sum;
			multiData.pos = avgTouchPoint;
			multiData.Update(Time.deltaTime);
			
			//Use delta size for pinch magnitude
			float size = GetSize();
			CBKEventManager.Controls.OnPinch(multiData.size - size);
			multiData.size = size;
			
			UpdateTouch(multiData);
		}
	}
	
	/// <summary>
	/// Gets the manhattan size of the smallest rectangle that contains
	/// all current touches
	/// </summary>
	/// <returns>
	/// The size.
	/// </returns>
	private float GetSize()
	{
		//Set all this values to +/- inf to make sure they get set
		//on first iteration
		float minX = float.PositiveInfinity;
		float minY = float.PositiveInfinity;
		float maxX = float.NegativeInfinity;
		float maxY = float.NegativeInfinity;
		
		foreach (TCKTouchData item in touches.Values) 
		{
			if (item.pos.x < minX)
			{
				minX = item.pos.x;
			}
			if (item.pos.x > maxX)
			{
				maxX = item.pos.x;
			}
			if (item.pos.y < minY)
			{
				minY = item.pos.y;
			}
			if (item.pos.y > maxY)
			{
				maxY = item.pos.y;
			}
		}
		
		return maxX - minX + maxY - minY;
	}
	
	bool HitsUI(Vector2 screenPos)
	{
		Ray ray = uiCamera.ScreenPointToRay(screenPos);
		return Physics.Raycast(ray, uiCamera.farClipPlane - uiCamera.nearClipPlane, 1 << CBKValues.Layers.UI);
	}
	
}
