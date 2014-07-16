using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script for the town section.
/// No logic for locking to target.
/// Move Relative is controlled by AOC2BuildingManager, in order to delineate when
/// a drag should affect buildings rather than the camera.
/// </summary>
public class MSTownCamera : MonoBehaviour, MSIPlaceable
{
	public static MSTownCamera instance;

	/// <summary>
	/// The smallest orthographic size of the camera.
	/// Once this value is decided, we can make it a constant
	/// </summary>
	public float MIN_SIZE = 3f;
	
	/// <summary>
	/// The largest orthographic size of the camera.
	/// Once this value is decided, we can make it a constant
	/// </summary>
	public float MAX_SIZE = 10f;
	
	/// <summary>
	/// The coefficient of the drag, in order to make touch drag
	/// smooth
	/// </summary>
	public float DRAG_COEFF = 2f;
	
	/// <summary>
	/// Small coefficient applied to X movement to adjust
	/// to the fact that the screen is wider than it is tall
	/// </summary>
	public float X_DRAG_FUDGE = 1.1f;
	
	public float LEFT_LIMIT = 10f;
	public float RIGHT_LIMIT = 10f;
	public float TOP_LIMIT = 10f;
	public float BOTTOM_LIMIT = 10f;
	
	/// <summary>
	/// Constant coefficient to slow down camera scaling on zoom
	/// </summary>
	const float CAMERA_ZOOM_SCALE = .2f;
	
    /// <summary>
    /// This camera's transform, 
    /// </summary>
    public Transform trans;
	
	/// <summary>
	/// The camera that this script controls
	/// </summary>
	public Camera cam;
	
	const float ZOOM_COEFFICIENT = -1f;
	const float BASE_CAMERA_OFFSET_MAX = 17.5f;

	const float CAMERA_BASE_HOME_Y = 20.2f;
	const float CAMERA_BASE_MISSION_Y = 19.6f;

	[SerializeField] float deceleration = .7f;

	Vector3 velocity = Vector3.zero;
	
	public float maxY;
	public float maxX;

	[SerializeField] UILabel debug;

	bool controllable = true;
	
	/// <summary>
	/// Awake this instance.
	/// Get the local components that this camera will reference
	/// </summary>
	virtual public void Awake()
	{
		trans = transform;
		cam = camera;
		instance = this;
	}
	
	void Start()
	{
		Zoom (0);
		if (MSActionManager.UI.OnCameraResize != null)
		{
			MSActionManager.UI.OnCameraResize(cam);
		}
	}
	
	/// <summary>
	/// Raises the enable event.
	/// Set up delegtes
	/// </summary>
	virtual protected void OnEnable()
	{
		MSActionManager.Controls.OnPinch += Zoom;
		MSActionManager.Scene.OnCity += Reset;
		controllable = true;
	}
	
	/// <summary>
	/// Raises the diable event.
	/// Set up delegates
	/// </summary>
	virtual protected void OnDisable()
	{
		MSActionManager.Controls.OnPinch -= Zoom;
		MSActionManager.Scene.OnCity -= Reset;
	}
	
	/// <summary>
	/// Moves the camera according to touch movement
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	virtual public void MoveRelative(TCKTouchData touch)
	{
		if (!MSTutorialManager.instance.inTutorial)
		{
			velocity = touch.delta;
		}
	}

	void SetBounds ()
	{
		maxY = MSCityBackground.mapHeight - cam.orthographicSize - .5f;
		maxX = MSCityBackground.mapWidth - (cam.orthographicSize * Screen.width / Screen.height);
	}
	
	public void ClampCamera()
	{
		trans.localPosition = new Vector3(Mathf.Clamp(trans.localPosition.x, -maxX, maxX),                       
             Mathf.Clamp (trans.localPosition.y, -maxY, maxY), trans.localPosition.z);
	}
	
	/// <summary>
	/// Zoom the camera by the specified amount.
	/// </summary>
	/// <param name='amount'>
	/// Amount to zoome the camera in or out by.
	/// A positive number zooms in, while a negative number
	/// zooms out.
	/// </param>
	public void Zoom(float amount)
	{
		cam.orthographicSize += amount * Time.deltaTime * CAMERA_ZOOM_SCALE;
		if (cam.orthographicSize > MAX_SIZE)
		{
			cam.orthographicSize = MAX_SIZE;
		}
		else if (cam.orthographicSize < MIN_SIZE)
		{
			cam.orthographicSize = MIN_SIZE;
		}
		
		if (MSActionManager.UI.OnCameraResize != null)
		{
			MSActionManager.UI.OnCameraResize(cam);
		}

		SetBounds ();
		
		ClampCamera ();
	}

	void Reset()
	{
		trans.localPosition = Vector3.zero;
		cam.orthographicSize = 8;
		cam.aspect = ((float)Screen.width)/Screen.height;
		SetBounds();
	}

	/// <summary>
	/// Update this instance.
	/// Cheats to zoom the camera in/out when on computer
	/// </summary>
	void Update()
	{
		if (velocity.sqrMagnitude > 0)
		{
			MoveVelocity ();
			velocity *= deceleration;
		}
		
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.K))
		{
			Zoom(12);
		}
		if (Input.GetKey(KeyCode.I))
		{
			Zoom (-12);
		}
		#endif
	}

	void MoveVelocity()
	{
		Vector3 movement = velocity;
		
		//Turn the mouse difference in screen coordinates to world coordinates
		movement.y *= DRAG_COEFF * (cam.orthographicSize / Screen.height);
		movement.x *= DRAG_COEFF * (cam.orthographicSize / Screen.width) * X_DRAG_FUDGE;
		
		//Flip the directions, since we want to move the camera in the opposite
		//directions to the touch movement
		movement *= -1;
		
		//Add the difference to the original position, since we only hold original mouse pos
		trans.localPosition += movement;
		ClampCamera();
	}

	public void SlideToPos(Vector3 localPos, float size, float time)
	{
		StartCoroutine(SlideToCameraPosition(localPos, size, time));
	}

	public IEnumerator SlideToCameraPosition(Vector3 localPos, float size, float time = 0)
	{
		controllable = false;
		if (time == 0)
		{
			trans.localPosition = localPos;
		}
		else
		{
			float currTime = 0;
			Vector3 startPos = trans.localPosition;
			float startSize = cam.orthographicSize;
			while (currTime < time)
			{
				currTime += Time.deltaTime;
				trans.localPosition = Vector3.Lerp(startPos, localPos, currTime/time);
				cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, currTime/time);
				yield return null;
			}
		}
		controllable = true;

	}
}
