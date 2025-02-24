/*  Created By:     Kamil Woloszyn
 *  Function:       Camera Movement & Zoom for game
 *  Last Edit Date: 14/03/2024
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Creating Instance of the CameraController script
    private static CameraController instance = null;
    public static CameraController Singleton
    {
        get
        {
            return instance;
        }
    }

    //Camera Serialised Object
    [SerializeField] private Camera cam = null;
    [SerializeField] private GameObject player = null;

    //Player Variables
    [SerializeField, Range(0, 10)] private int health;
    [SerializeField, Range(0, 5)] private int damage;

    //Movement Serialised Variables
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveSmoothing = 25;

    //Zoom Serialised Variables
    [SerializeField] private float zoomSpeed = 20;
    [SerializeField] private float zoomSmoothing = 25;

    //Inputs Variable
    private InputControls inputs = null;

    //Boolean to check if the player is moving the camera around
    private bool isMoving = false;
    public bool IsMoving // Expose isMoving as a public property
    {
        get { return isMoving; }
    }

    private bool isZooming = false;
    public bool IsZooming
    {
        get { return isZooming; }
    }

    private bool isPlayerClicked = false;
    public bool IsPlayerClicked
    {
        get { return isPlayerClicked; }
    }


    //Camera Transforms to setup camera
    private Transform rootTransform = null;
    private Transform pivotTransform = null;
    private Transform targetTransform = null;

    //Vector3 value to set camera to the centre of the map
    private Vector3 _center = Vector3.zero;

    //Default Variables
    private float rightMax = 0f;
    private float leftMax = 4f;
    private float upMax = 2f;
    private float downMax = 2f;
    private float pivotAngle = 0;
    private float rootAngle = 90;
    private float zoom = 3;
    private float zoomMax = 3f;
    private float zoomMin = 2f;
    private float zoomSpeedDifference = 0;
    private float startMoveSpeed = 0;

    //Zoom Pos On Screen Variable
    private Vector2 zoomPositionOnScreen = Vector2.zero;
    private Vector3 zoomPositionInWorld = Vector3.zero;
    private float zoomBaseValue = 0;
    private float zoomBaseDistance = 0;

    //Time Keeping & Cooldown Variables
    private float time = 0f;
    private float bulletCooldown = 0.8f;

    //Function happens on start of game
    private void Awake()
    {
        //Assigning Instance to this script
        instance = this;

        //Setting inputs to new CameraControls object
        inputs = new InputControls();

        //Creating new camera objects
        rootTransform = new GameObject("CameraRoot").transform;
        pivotTransform = new GameObject("CameraPivot").transform;
        targetTransform = new GameObject("CameraTarget").transform;

        //Setting camera to orthographic view
        cam.orthographic = true;
        cam.nearClipPlane = 0;


    }

    private void Start()
    {
        Initialise();
    }
    private void Initialise()
    {
        //Parenting Camera objects to eachother
        pivotTransform.SetParent(rootTransform);
        targetTransform.SetParent(pivotTransform);

        //Setting Default Zoom on Camera
        cam.orthographicSize = zoom;

        //Setting position and angle of the root camera object
        rootTransform.position = _center;
        rootTransform.localEulerAngles = new Vector3(0, 0, 0);

        //Setting position and angle of the pivot camera object
        pivotTransform.localPosition = Vector3.zero;
        pivotTransform.localEulerAngles = new Vector3(pivotAngle, 0, 0);

        //Setting position and angle of the target camera object
        targetTransform.localPosition = new Vector3(0, 0, 0);
        targetTransform.localEulerAngles = Vector3.zero;

        //starting move speed saved
        startMoveSpeed = moveSpeed;

        //Checking if player is null if so assign the gameobject automatically
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnEnable()
    {
        //Enabling Input
        inputs.Enable();

        //Adding Listeners 
        inputs.Main.Move.started += _ => MoveStart();
        inputs.Main.Move.canceled += _ => MoveCancelled();

        inputs.Main.TouchZoom.started += _ => ZoomStart();
        inputs.Main.TouchZoom.canceled += _ => ZoomCancelled();
    }

    private void OnDisable()
    {
        //Disabling input
        inputs.Disable();

        //Removing Listeners
        inputs.Main.Move.started -= _ => MoveStart();
        inputs.Main.Move.canceled -= _ => MoveCancelled();


        inputs.Main.TouchZoom.started -= _ => ZoomStart();
        inputs.Main.TouchZoom.canceled -= _ => ZoomCancelled();
    }

    private void ZoomStart()
    {
        Vector2 touch0 = inputs.Main.TouchPos0.ReadValue<Vector2>();
        Vector2 touch1 = inputs.Main.TouchPos1.ReadValue<Vector2>();
        zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
        zoomPositionInWorld = CameraScreenPositionToPlanePosition(zoomPositionOnScreen);
        zoomBaseValue = zoom;

        touch0.x /= Screen.width;
        touch1.x /= Screen.width;
        touch0.y /= Screen.height;
        touch1.y /= Screen.height;

        zoomBaseDistance = Vector2.Distance(touch0, touch1);

        isZooming = true;
    }

    private void ZoomCancelled()
    {
        isZooming = false;
    }

    private void MoveStart()
    {
        isMoving = true;
        var rayHit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(inputs.Main.PointerPosition.ReadValue<Vector2>()));
        if(rayHit.collider)
        {
            isPlayerClicked = true;
        }
        
    }

    private void MoveCancelled()
    {
        isMoving = false;
        isPlayerClicked = false;
    
    }

    private void Update()
    {
        //Time Keeping
        time += Time.deltaTime;
        if (Input.touchSupported == false)
        {
            //Updating camera zoom
            cam.orthographicSize = zoom;

            //Setting mouse scroll value from inputs
            float mouseScroll = inputs.Main.MouseScroll.ReadValue<float>();


            //Checking if there was any input from scroll wheel
            if (mouseScroll > 0)
            {
                zoom -= zoomSpeed * Time.deltaTime;
                //Lowering Speed when zoomed in
                zoomSpeedDifference = (zoom - zoomMax) * 2;
                moveSpeed = startMoveSpeed - zoomSpeedDifference;
            }
            else if (mouseScroll < 0)
            {
                zoom += zoomSpeed * Time.deltaTime;
                //Lowering Speed when zoomed in
                zoomSpeedDifference = (zoom - zoomMax) * 2;
                moveSpeed = startMoveSpeed - zoomSpeedDifference;
            }
        }

        if (isZooming)
        {
            Vector2 touch0 = inputs.Main.TouchPos0.ReadValue<Vector2>();
            Vector2 touch1 = inputs.Main.TouchPos1.ReadValue<Vector2>();

            touch0.x /= Screen.width;
            touch1.x /= Screen.width;
            touch0.y /= Screen.height;
            touch1.y /= Screen.height;

            float currentDistance = Vector2.Distance(touch0, touch1);
            float deltaDistance = currentDistance - zoomBaseDistance;
            zoom = zoomBaseValue - (deltaDistance * zoomSpeed);

            Vector3 zoomCentre = CameraScreenPositionToPlanePosition(zoomPositionOnScreen);
            rootTransform.position += (zoomPositionInWorld - zoomCentre);

        }
        //If the player is moving the camera
        else if (isMoving)
        {
            if(isPlayerClicked)
            {
                Vector2 move = inputs.Main.MoveDelta.ReadValue<Vector2>();
                if (move != Vector2.zero)
                {
                    
                    //Divinging the movement by resolution giving us a percentage which allows for smooth movement regardless of resolution/screen size
                    move.x /= Screen.width;
                    move.y /= Screen.height;

                    //Moving left & Right
                    rootTransform.transform.position += rootTransform.transform.right.normalized * move.x * moveSpeed;

                    //Moving UP & DOWN
                    rootTransform.transform.position += rootTransform.transform.up.normalized * move.y * moveSpeed * 0.5f;
                }
            }
            
        }
        //Function to check if the camera is still inside of the map bounds
        AdjustBounds();

        //If camera position is not equal to target position we lerp the camera to the target position smoothly
        if (player.transform.position != rootTransform.position)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, rootTransform.position, moveSmoothing * Time.deltaTime);
            
        }

        //If the camera zoom is not equal to target zoom we lerp the camera zoom to the target zoom smoothly
        if (cam.orthographicSize != zoom)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, zoomSmoothing * Time.deltaTime);
        }

        player.transform.rotation = new Quaternion(0, 0, 0, 0);

        if(time >= bulletCooldown)
        {
            ContinousFireBullet();
            time = 0f;
            
        }
    }

    private void AdjustBounds()
    {
        //Checking if the zoom is less than minimum zoom value
        if (zoom < zoomMin)
        {
            zoom = zoomMin;
        }

        //Checking if the zoom is more than max zoom value
        if (zoom > zoomMax)
        {
            zoom = zoomMax;
        }

        //Getting camera position from the root positon
        Vector3 camPosition = rootTransform.position;

        //Checking the bounds of the camera in a square and making sure it is within this square.
        if (camPosition.x > rightMax)
        {
            camPosition.x = rightMax;
        }
        if (camPosition.x < -leftMax)
        {
            camPosition.x = -leftMax;
        }
        if (camPosition.y > upMax)
        {
            camPosition.y = upMax;
        }
        if (camPosition.y < -downMax)
        {
            camPosition.y = -downMax;
        }

        //Setting the position of the root camera to a position inside the bounding square
        rootTransform.position = camPosition;
    }

    //Calculating Screen position to world position
    public Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
    {
        float h = cam.orthographicSize * 2f;
        float w = cam.aspect * h;

        Vector3 anchor = cam.transform.position - (cam.transform.right.normalized * w / 2f) - (cam.transform.up.normalized * h / 2f);

        return anchor + (cam.transform.right.normalized * position.x / Screen.width * w) + (cam.transform.up.normalized * position.y / Screen.height * h);
    }

    //Calculating Screen Positon to Plane Position
    public Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
    {
        Vector3 point = CameraScreenPositionToWorldPosition(position);
        float h = point.y - rootTransform.position.y;
        float x = h / Mathf.Sin(0 * Mathf.Deg2Rad);
        return point + cam.transform.forward.normalized * x;
    }

    public void ContinousFireBullet()
    {
        if (!isMoving) return;
        if (!isPlayerClicked) return;
        SpawningManager.Singleton.InstantiateBullet();
        
    }

    public void PlayerTookDamage(int damage)
    {
        health -= damage;
    }

    public int GetPlayerDamage()
    {
        return damage;
    }

    public int GetPlayerHealth()
    {
        return health;
    }

}
