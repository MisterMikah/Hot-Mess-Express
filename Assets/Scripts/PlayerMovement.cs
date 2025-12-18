using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float forwardSpeed = 10f;
    public float laneChangeSpeed = 15f;

    [Header("Lanes")]
    public float laneWidth = 3f;          // distance between lanes
    private int lane = 1;                 // 0 = left, 1 = middle, 2 = right
    private int previousLane = 1;
    private float targetX;

    [Header("Jump & Gravity")]
    public float jumpForce = 15f;
    public float gravity = -27f;
    public float fastFallMultiplier = 1.5f; // Press down in-air = drop faster
    private float yVelocity;

    [Header("Slide")]
    public float slideDuration = 0.6f;
    public float slideHeight = 1.0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot; // model root to rotate at finish

    // internals
    private CharacterController cc;
    private PlayerControls input;
    private bool sliding;
    private Coroutine slideRoutine;

    public bool IsSliding => sliding;

    private float originalHeight;
    private Vector3 originalCenter;

    private bool hasFinished = false;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = new PlayerControls();

        originalHeight = cc.height;
        originalCenter = cc.center;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (visualRoot == null && animator != null)
            visualRoot = animator.transform;
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
        if (hasFinished)
        {
            // Only gravity so we stay grounded, no lane/forward movement
            if (cc.isGrounded)
            {
                if (yVelocity < 0f) yVelocity = -2f;
            }
            else
            {
                yVelocity += gravity * Time.deltaTime;
            }

            Vector3 moveFinished = new Vector3(0f, yVelocity * Time.deltaTime, 0f);
            cc.Move(moveFinished);
            return;
        }

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

        UpdateAnimator();
    }

    // ---------------- helpers ----------------

    void ChangeLane(int dir)
    {
        int newLane = Mathf.Clamp(lane + dir, 0, 2); // 3 lanes: 0,1,2
        if (newLane == lane) return;

        previousLane = lane;
        lane = newLane;
        targetX = (lane - 1) * laneWidth; // lanes at -w, 0, +w
    }

    public void RevertLane()
    {
        lane = previousLane;
        targetX = (lane - 1) * laneWidth;
    }

    void TryJump()
    {
        if (!cc.isGrounded) return;

        if (sliding) CancelSlide();

        yVelocity = jumpForce;
    }

    void TrySlide()
    {
        if (cc.isGrounded)
        {
            if (!sliding) slideRoutine = StartCoroutine(SlideRoutine());
        }
        else
        {
            // in air -> fast-fall
            yVelocity = gravity * fastFallMultiplier;
        }
    }

    IEnumerator SlideRoutine()
    {
        sliding = true;

        // shrink controller for slide
        cc.height = slideHeight;
        cc.center = new Vector3(originalCenter.x, slideHeight * 0.5f, originalCenter.z);

        float t = 0f;
        while (t < slideDuration && sliding)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // restore
        cc.height = originalHeight;
        cc.center = originalCenter;
        sliding = false;
        slideRoutine = null;
    }

    void CancelSlide()
    {
        if (!sliding) return;

        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
            slideRoutine = null;
        }

        cc.height = originalHeight;
        cc.center = originalCenter;
        sliding = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Don't stomp dance state
        if (animator.GetBool("IsDancing")) return;

        bool grounded = cc.isGrounded;
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsSliding", sliding);
    }

    public void EndRun()
    {
        if (hasFinished) return;
        hasFinished = true;

        forwardSpeed = 0f;
        sliding = false;

        if (input != null)
            input.Player.Disable();

        if (animator != null)
        {
            animator.SetBool("IsSliding", false);
            animator.SetBool("IsDancing", true);
        }

        if (visualRoot != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 toCam = cam.transform.position - visualRoot.position;
                toCam.y = 0f;

                if (toCam.sqrMagnitude > 0.01f)
                    visualRoot.forward = toCam.normalized;
            }
            else
            {
                visualRoot.Rotate(0f, 180f, 0f);
            }
        }
    }


}
