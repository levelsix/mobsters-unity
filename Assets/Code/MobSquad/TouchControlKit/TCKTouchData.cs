using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Touch data class, so that we can keep track of certain aspects of touch
/// </summary>
[System.Serializable]
public class TCKTouchData 
{
	#region Members
	
	#region Enumerations
	
	/// <summary>
	/// Phases for touches, TAP for short term, HOLD for long term
	/// </summary>
	public enum Phase{TAP, HOLD};
	
	#endregion
	
	#region Public
	
	/// <summary>
	/// Whether or not this touch started on UI
	/// </summary>
	public bool ui = false;
	
	/// <summary>
	/// Constant for the amount of time it takes for a tap to become a hold
	/// </summary>
	public float HOLD_TIME = .2f;
	
	/// <summary>
	/// Whether or not this touch is stationary
	/// </summary>
	public bool stationary;
	
	/// <summary>
	/// The phase of this touch instance.
	/// </summary>
	public Phase phase;
	
	/// <summary>
	/// The initial position.
	/// </summary>
	public Vector2 _initialPos;
	
	/// <summary>
	/// The current position.
	/// </summary>
	public Vector2 pos;
	
	/// <summary>
	/// The change in position since the last frame
	/// </summary>
	public Vector3 delta;
	
	/// <summary>
	/// The size between all points in this touch.
	/// Value ignored unless multitouch.
	/// </summary>
	public float size = 0;
	
	/// <summary>
	/// The count of touches.
	/// Value == 1 unless this is a multitouch data
	/// </summary>
	public int count = 1;
	
	/// <summary>
	/// The identifier.
	/// </summary>
	public int id;
	
	/// <summary>
	/// Gets the index of the count.
	/// Used for touch events, which are indexed with
	/// 1-finger touches being index[0]
	/// </summary>
	/// <value>
	/// The index of the count.
	/// </value>
	public int countIndex{
		get
		{
			return count-1;
		}
	}
	
	#endregion
	
	#region Private
	
	/// <summary>
	/// The lifetime.
	/// </summary>
	private float _lifetime;
	
	#endregion
	
	#region Constant
	
	/// <summary>
	/// Constant for the square of the distance which a tap must move to become a drag
	/// Kept as a square so that we don't have to sqrt the distance
	/// </summary>
	private const float SQR_DRAG_DIST = 225f;
	
	#endregion
	
	#region Properties
	
	/// <summary>
	/// Gets the raw movement from original to current positions.
	/// </summary>
	/// <value>
	/// The movement.
	/// </value>
	public Vector2 Movement{
		get
		{
			return pos - _initialPos;
		}
	}
	
	/// <summary>
	/// Gets the square distance between the initial
	/// touch and the current position.
	/// </summary>
	public float SqrDist{
		get
		{
			float dy = _initialPos.y - pos.y;
			float dx = _initialPos.x - pos.x;
			return dx * dx + dy * dy;
		}
	}
	
	#endregion
	
	#endregion
	
	#region Functions
	
	#region Initialization
	
	/// <summary>
	/// Initializes a new instance of the <see cref="TCKTouchData"/> class.
	/// </summary>
	/// <param name='_pos'>
	/// Position.
	/// </param>
	public TCKTouchData(Vector2 _pos)
	{
		init(_pos);
	}
	
	/// <summary>
	/// Init this touch data at the specified position
	/// </summary>
	/// <param name='_pos'>
	/// Starting position for this touch
	/// </param>
	public void init(Vector2 _pos)
	{
		count = 1; //Assumed one count unless set otherwise after init
		_initialPos = _pos;
		pos = _pos;
		phase = Phase.TAP;
		_lifetime = 0;
		stationary = true;
	}
	
	#endregion
	
	/// <summary>
	/// Update this touch data with time and state
	/// </summary>
	/// <param name='time'>
	/// Time since last frame.
	/// </param>
	public void Update(float time)
	{
		_lifetime += time;
		
		//Hold check
		if (phase == Phase.TAP && _lifetime > HOLD_TIME)
		{
			phase = Phase.HOLD;
		}
		
		//Stationary check
		if (stationary && SqrDist > SQR_DRAG_DIST)
		{
			stationary = !stationary;
			//If nothing is detecting flicks, turn this into a hold
			//so that a drag will be detected immediately
			
			if (CBKEventManager.Controls.OnFlick[countIndex] == null)
			{
				phase = Phase.HOLD;
				if (CBKEventManager.Controls.OnStartDrag[countIndex] != null)
				{
					CBKEventManager.Controls.OnStartDrag[countIndex](this);
				}
			}
		}
	}
	
	public override string ToString()
	{
		return phase + ", Count: " + count + ", Curr: " + pos + ", Initial: " + _initialPos + ", Life: " + _lifetime;
	}
	
	#endregion
}
