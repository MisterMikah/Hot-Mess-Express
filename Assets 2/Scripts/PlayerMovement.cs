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
    private int lane = 1;               // 0 = left, 1 = middle, 2 = right
    private float targetX;

    [Header("Jump & Gravity")]
    public float jumpForce = 15f;
    public float gravity = -27f;
    public float fastFallMultiplier = 1.5f; // Press down in-air = drop faster
    private float yVelocity;

    [Header("Slide")]
    public float slideDuration = 0.6f;
    public float slideHeight = 1.0f;

    [Header("Visuals")]
    public Transform visualModel;        // assign the Model child here
    public float slideVisualOffset = 0.5f; // how far down the model moves when sliding

    private int previousLane;
    public int CurrentLane => lane;
    public int PreviousLane => previousLane;

    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioClip slideClip;
    public AudioClip laneSwitchClip;
    public AudioClip runningClip;

    [SerializeField] private Animator animator;


    // ---- internals ----
    private CharacterController cc;
    private PlayerControls input;
    private bool sliding;
    private Coroutine slideRoutine;

    public bool IsSliding => sliding;

    private float originalHeight;
    private Vector3 originalCenter;

    private Vector3 originalModelLocalPos;

    private void Start()
    {
        Time.timeScale = 1f; //ensure time is normal on start
    }
    void Awake()
    {
        previousLane = lane; // start with middle

        cc = GetComponent<CharacterController>();
        input = new PlayerControls();

        originalHeight = cc.height;
        originalCenter = cc.center;

        if (visualModel != null)
            originalModelLocalPos = visualModel.localPosition;

        // play running sound
        if (SoundFXManager.instance != null && runningClip != null)
        {
            SoundFXManager.instance.PlayLoopingFXClip(runningClip, transform, 1f); 
        }
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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

        UpdateAnimator();

    }

    // ---------------- helpers ----------------

    void ChangeLane(int dir)
    {
        int newLane = Mathf.Clamp(lane + dir, 0, 2); // 0 = left, 1 = middle, 2 = right
        if (newLane == lane) return;

        previousLane = lane;            // remember where we came from
        lane = newLane;
        targetX = (lane - 1) * laneWidth; // -w, 0, +w


        // play lane switch sound
        if (SoundFXManager.instance != null && laneSwitchClip != null) //add condition for running being played
        {
            SoundFXManager.instance.DestroyLoopingFXClip(); //destroys running audio
            SoundFXManager.instance.PlaySoundFXClip(laneSwitchClip, transform, 1f); //plays lane switch sound
            SoundFXManager.instance.PlayLoopingFXClip(runningClip, transform, 1f); //plays running sound after lane switch
        }
    }

    // called when you side-bump something
    public void RevertLane()
    {
        lane = previousLane;
        targetX = (lane - 1) * laneWidth;
    }


    void TryJump()
    {
        // Only jump if grounded
        if (!cc.isGrounded) return; 

        // If we�re sliding, cancel slide and go into jump
        if (sliding) CancelSlide();

        yVelocity = jumpForce;

        // play jump sound if manager exists
        if (SoundFXManager.instance != null && jumpClip != null)
        {
            SoundFXManager.instance.DestroyLoopingFXClip(); //destroys running audio
            SoundFXManager.instance.PlaySoundFXClip(jumpClip, transform, 1f); //plays jump sound
            SoundFXManager.instance.PlayLoopingFXClip(runningClip, transform, 1f); //plays running sound after jump
        }
    }


    void TrySlide()
    {
        if (cc.isGrounded)
        {
            if (!sliding) slideRoutine = StartCoroutine(SlideRoutine());

            // play slide sound
            if (SoundFXManager.instance != null && slideClip != null)
            {
                SoundFXManager.instance.DestroyLoopingFXClip(); //destroys running audio
                SoundFXManager.instance.PlaySoundFXClip(slideClip, transform, 1f); //plays slide whoosh sound
                SoundFXManager.instance.PlayLoopingFXClip(runningClip, transform, 1f); //plays running sound after sliding
            }
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

        // move visual model down
        if (visualModel != null)
        {
            visualModel.localPosition = originalModelLocalPos + new Vector3(0f, -slideVisualOffset, 0f);
        }

        // Time the slide
        float t = 0f;
        while (t < slideDuration && sliding)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // restore collider
        cc.height = originalHeight;
        cc.center = originalCenter;

        // restore visual
        if (visualModel != null)
        {
            visualModel.localPosition = originalModelLocalPos;
        }

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

        if (visualModel != null)
            visualModel.localPosition = originalModelLocalPos;

        sliding = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        bool grounded = cc.isGrounded;

        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsSliding", sliding);
    }


}
