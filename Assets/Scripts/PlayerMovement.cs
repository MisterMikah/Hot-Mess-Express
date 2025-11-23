using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float forwardSpeed = 10f;
    public float laneChangeSpeed = 15f;

    [Header("Lanes")]
    public float laneWidth = 3f;        // distance between lanes
    private int lane = 1;                 // 0 = left, 1 = middle, 2 = right
    private float targetX;

    [Header("Jump & Gravity")]
    public float jumpForce = 15f;
    public float gravity = -27f;
    public float fastFallMultiplier = 1.5f; // Press down in-air = drop faster
    private float yVelocity;

    [Header("Slide")]
    public float slideDuration = 0.6f;
    public float slideHeight = 1.0f;

    // ---- internals ----
    private CharacterController cc;
    private PlayerControls input;
    private bool sliding;
    private Coroutine slideRoutine;

    // Exposed read-only for camera, etc.
    public bool IsSliding => sliding;

    // store originals once so we can go back on cancel/jump
    private float originalHeight;
    private Vector3 originalCenter;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = new PlayerControls();

        originalHeight = cc.height;
        originalCenter = cc.center;
    }

    void OnEnable()
    {
        input.Player.Left.performed += _ => ChangeLane(-1);
        input.Player.Right.performed += _ => ChangeLane(+1);
        input.Player.Jump.performed += _ => TryJump();
        input.Player.Slide.performed += _ => TrySlide();
        input.Player.Enable();

        targetX = transform.position.x;
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        // Constant forward motion
        move.z = forwardSpeed * Time.deltaTime;

        // Smooth lane movement toward targetX
        float deltaX = targetX - transform.position.x;
        float stepX = Mathf.Clamp(deltaX, -laneChangeSpeed * Time.deltaTime, laneChangeSpeed * Time.deltaTime);
        move.x = stepX;

        // Gravity & vertical motion
        if (cc.isGrounded)
        {
            if (yVelocity < 0f) yVelocity = -2f; // small stick-to-ground pull
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        move.y = yVelocity * Time.deltaTime;

        cc.Move(move);
    }

    // ---------------- helpers ----------------

    void ChangeLane(int dir)
    {
        int newLane = Mathf.Clamp(lane + dir, 0, 2); // Can only move within 3 lanes
        if (newLane == lane) return;
        lane = newLane;
        targetX = (lane - 1) * laneWidth; // lanes at -w, 0, +w
    }

    void TryJump()
    {
        // Only jump if grounded
        if (!cc.isGrounded) return;

        // If we’re sliding, cancel slide and go into jump
        if (sliding) CancelSlide();

        yVelocity = jumpForce;
    }

    void TrySlide()
    {
        if (cc.isGrounded)
        {
            // Begin ground slide
            if (!sliding) slideRoutine = StartCoroutine(SlideRoutine());
        }
        else
        {
            // In air -> fast-fall
            yVelocity = gravity * fastFallMultiplier;
        }
    }

    IEnumerator SlideRoutine()
    {
        sliding = true;

        // shrink controller so we can go under obstacles
        cc.height = slideHeight;
        cc.center = new Vector3(originalCenter.x, slideHeight * 0.5f, originalCenter.z);

        // Time the slide
        float t = 0f;
        while (t < slideDuration && sliding)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // restore if not already restored by CancelSlide()
        cc.height = originalHeight;
        cc.center = originalCenter;
        sliding = false;
        slideRoutine = null;
    }

    void CancelSlide()
    {
        if (!sliding) return;

        // stop coroutine if running
        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
            slideRoutine = null;
        }

        // restore controller now
        cc.height = originalHeight;
        cc.center = originalCenter;
        sliding = false;
    }
}
