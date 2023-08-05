using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class LevelLocation : MonoBehaviour
{

    [Header("Saved Data")] // Class needs Level ID to be set for save function to work.
    public string uniqueLevelID;
    public int numberOfLevelCompletes;
    public int numberOfAttempts;
    public bool isTeleporter;
    
    [Header("Level Options")] 
    public LevelData levelData;
    public int fameRequirementsToMove;
    public int moneyRequirementsToMove;
    public List<string> itemRequirementsToMove;
    public bool moveRequirementsDisabled;
    [Header("Checked = No Reqs to Enter")]
    
    public LevelLocation teleportLocation;

    [Header("Autogenerated")] // Class will set these variables at run time if you forget to set them
    public List<LevelLocation> connectedLevels;
    public List<Route> routes;
    public Vector2 mapLocation;
    public Collider2D locationCollider;

    private float hoverStartTime;
    private OverworldControls playerInput;
    private Camera mainCamera;
    private float lastClickTime;
    private float clickThreshold = 0.3f;
    private bool isMouseOver = false;

    private void Awake()
    {
        if (locationCollider == null)
        {
            locationCollider = gameObject.AddComponent<CircleCollider2D>();
            locationCollider.isTrigger = true;
        }
        connectedLevels.RemoveAll(level => level == this);
        mapLocation = new Vector2(transform.position.x, transform.position.y);
        mainCamera = Camera.main;

        if (isTeleporter && teleportLocation == this)
        {
            teleportLocation = null;
            isTeleporter = false;
        }
    }

    private void OnEnable()
    {
        playerInput = new OverworldControls();
        playerInput.Enable();
        playerInput.UI.Click.performed += ClickPerformed;
        StartCoroutine(CheckMouseHoverCoroutine());
    }

    private void OnDisable()
    {
        playerInput.UI.Click.performed -= ClickPerformed;
        playerInput.Disable();
        StopCoroutine(CheckMouseHoverCoroutine());
    }

    private void ClickPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("Clicked");
        Vector2 mousePosition = playerInput.UI.Point.ReadValue<Vector2>();
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        if (locationCollider.OverlapPoint(worldPosition))
        {
            if (isTeleporter && Time.time - lastClickTime < clickThreshold)
            {
                OverworldEvent.TeleporterDoubleClick(this);
            }
            else
            {
                OverworldEvent.LevelSelected(this);
            }

            lastClickTime = Time.time;
        }
    }
    
    private void OnPointerEnter()
    {
        //Debug.Log("Pointer Entered");
        hoverStartTime = Time.time;
        StartCoroutine(HoverCoroutine());
    }

    private void OnPointerExit()
    {
        //Debug.Log("Pointer Exit");
        OverworldEvent.LevelUnhovered(this);
        StopCoroutine(HoverCoroutine());
    }

    private void CheckMouseHover()
    {
        if (playerInput != null)
        {
            Vector2 mousePosition = playerInput.UI.Point.ReadValue<Vector2>();
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null && hit.collider == locationCollider)
            {
                if (!isMouseOver)
                {
                    isMouseOver = true;
                    OnPointerEnter();
                }
            }
            else
            {
                if (isMouseOver)
                {
                    isMouseOver = false;
                    OnPointerExit();
                }
            }
        }
    }

    private IEnumerator CheckMouseHoverCoroutine()
    {
        while (enabled)
        {
            CheckMouseHover();
            yield return new WaitForSeconds(0.05f); // Check every 0.05 seconds (customize as needed)
        }
    }

    private IEnumerator HoverCoroutine()
    {
        while (true)
        {
            if (Time.time - hoverStartTime >= 1.5f)
            {
                OverworldEvent.LevelHovered(this);
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collided");
        if (collision.CompareTag("Player"))
        {
            OverworldEvent.LevelEntered(this);
        }
    }


}