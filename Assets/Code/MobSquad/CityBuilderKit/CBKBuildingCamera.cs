using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script for the town section.
/// No logic for locking to target.
/// Move Relative is controlled by AOC2BuildingManager, in order to delineate when
/// a drag should affect buildings rather than the camera.
/// </summary>
public class CBKBuildingCamera : MonoBehaviour, CBKIPlaceable
{
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
	
	public Vector3 worldPosition;
	
    /// <summary>
    /// This camera's transform, 
    /// </summary>
    public Transform trans;
	
	/// <summary>
	/// The camera that this script controls
	/// </summary>
	public Camera cam;
	
	const float ZOOM_COEFFICIENT = -1f;
	const float BASE_CAMERA_OFFSET_MAX = 18f;

	const float CAMERA_BASE_HOME_Y = 26.8f;
	const float CAMERA_BASE_MISSION_Y = 19.6f;
	
	public float maxY;
	public float maxX;
	
	/// <summary>
	/// Awake this instance.
	/// Get the local components that this camera will reference
	/// </summary>
	virtual public void Awake()
	{
		trans = transform;
		cam = camera;
	}
	
	void Start()
	{
		Zoom (0);
		if (CBKEventManager.UI.OnCameraResize != null)
		{
			CBKEventManager.UI.OnCameraResize(cam);
		}
	}
	
	/// <summary>
	/// Raises the enable event.
	/// Set up delegtes
	/// </summary>
	virtual protected void OnEnable()
	{
		CBKEventManager.Controls.OnPinch += Zoom;
		CBKEventManager.Scene.OnCity += Reset;
	}
	
	/// <summary>
	/// Raises the diable event.
	/// Set up delegates
	/// </summary>
	virtual protected void OnDisable()
	{
		CBKEventManager.Controls.OnPinch -= Zoom;
		CBKEventManager.Scene.OnCity -= Reset;
	}
	
	/// <summary>
	/// Moves the camera according to touch movement
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	virtual public void MoveRelative(TCKTouchData touch)
	{
		Vector3 movement = touch.delta;
		
        //Turn the mouse difference in screen coordinates to world coordinates
        movement.y *= DRAG_COEFF * (Camera.main.orthographicSize / Screen.height);
        movement.x *= DRAG_COEFF * (Camera.main.orthographicSize / Screen.width) * X_DRAG_FUDGE;

        //Turn the 2D coordinates into our tilted isometric coordinates
		/*
        movement.z = movement.y - movement.x;
        movement.x = movement.x + movement.y;
        movement.y = 0;
		*/
		movement *= -1;

        //Add the difference to the original position, since we only hold original mouse pos
        trans.localPosition += movement;
		ClampCamera();
		
	}

	void SetBounds ()
	{
		maxY = CBKTownBackground.mapHeight - cam.orthographicSize;
		maxX = CBKTownBackground.mapWidth - (cam.orthographicSize * Screen.width / Screen.height);
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
		
		if (CBKEventManager.UI.OnCameraResize != null)
		{
			CBKEventManager.UI.OnCameraResize(cam);
		}

		SetBounds ();
		
		ClampCamera ();
	}

	void Reset()
	{
		Debug.Log("Camera reset: " + CBKWhiteboard.currCityType.ToString());
		if (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.NEUTRAL)
		{
			trans.parent.localPosition = new Vector3(0, CAMERA_BASE_MISSION_Y);
		}
		else
		{
			trans.parent.localPosition = new Vector3(0, CAMERA_BASE_HOME_Y);
		}
		cam.orthographicSize = 8;
		cam.aspect = ((float)Screen.width)/Screen.height;
		trans.localPosition = Vector3.zero;
		SetBounds();
	}

#if UNITY_EDITOR
	/// <summary>
	/// Update this instance.
	/// Cheats to zoom the camera in/out when on computer
	/// </summary>
	void Update()
	{
		worldPosition = trans.localPosition;
		
		if (Input.GetKey(KeyCode.K))
		{
			Zoom(12);
		}
		if (Input.GetKey(KeyCode.I))
		{
			Zoom (-12);
		}
	}
#endif
	
}
