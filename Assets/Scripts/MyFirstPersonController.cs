using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstPersonController : MonoBehaviour
{
    // public MouseData mouseItem = new MouseData();

    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKey(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("UI Effects")]
    [SerializeField] private ParticleSystem speedUpParticleFX;

#region fields 
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;


    [Header("Movement Parametrs")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float slideSpeed = 10.0f;


    [Header("Look Parametrs")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;


    [Header("Jumping Parametrs")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Health Paranetrs")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float timeBeforeRegenStarts = 20f;
    [SerializeField] private float healthValueIncrement = 1f;
    [SerializeField] private float healthTimeIncrement = 1f;
    private float currentHealth;
    private Coroutine regeneratingHealthCoroutine;
    public static Action<float> OnDamageTaken;
    public static Action<float> OnDamage;  // UI for displaying when we get damage
    public static Action<float> OnHeal;   // UI for displaying when we start healing

    [Header("Stamina Parametrs")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaUseMultiplier = 10f; // how many stamina will be reduced when shift is pressed
    [SerializeField] private float timeBeforeStaminaRegens = 5f;
    [SerializeField] private float staminaValueIncrement = 1f;
    [SerializeField] private float  staminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStaminaCoroutine;
    public static Action<float> OnReduceStamina;
    public static Action<float> OnRenewStamina;
    

    [Header("Crouch Parametrs")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool duringCrouchAnimation;


    [Header("Headbob Parametrs")]
    [SerializeField] private float walkBobSpeed = 14.0f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18.0f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crouchBobSpeed = 8.0f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0; // default y position of our camera
    private float timer; 

    [Header("Zoom Parametrs")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f; // Field of View
    private float defaultFOV;
    private Coroutine zoomRoutine; // to stop coroutine on demand

    [Header("Footstep Parametrs")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiPlayer = 1.5f;
    [SerializeField] private float sprintStepMultiplayer = 0.6f;
    [SerializeField] private AudioSource footStepsAudioSource = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] dirtClips = default;
    [SerializeField] private AudioClip[] waterClips = default;
    private float footStepTimer = 0f;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiPlayer : IsSprinting ? baseStepSpeed * sprintStepMultiplayer : baseStepSpeed;

    #endregion

    //Sliding Parametrs
    private Vector3 hitPointNormal;
    private bool IsSliding
    {
        get
        {
            if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit; 
            }
            {
                return false;
            }
        }
    }

    [Header("Interactable")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask currentInteractionLayer = default;
    private Interactable currentInteractable;

    public InventoryObject inventory;
    public InventoryObject equipment;

    Camera playerCamera;
    CharacterController characterController;
    Animator animator;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    [SerializeField] GameObject inventoryCanvas;

    private void OnEnable()
    {
        OnDamageTaken += ApplyDamage;
    }

    private void OnDisable()
    {
        OnDamageTaken -= ApplyDamage;
    }

    void Awake()
    {
        inventoryCanvas.SetActive(false);
        animator = GetComponentInChildren<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    private IEnumerator Hit()
    {
        animator.SetBool("punch", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("punch", false);
    }
    bool isActiveCanv = false;
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Q))
        // {
        //     isActiveCanv = !isActiveCanv;
        // }

        // if(isActiveCanv)
        // {
        //     inventoryCanvas.SetActive(true);
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        //     characterController.enabled = false;
        // }
        // else
        // {
        //     inventoryCanvas.SetActive(false);
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        //     characterController.enabled = true;
        // }

        HandleSavingAndLoading();
        HandleHitAnimation();

        RunWalkAnim();

        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
            {
                HandleJump();
            }

            if (ShouldCrouch)
            {
                HandleCrouch();
            }

            if (canUseHeadbob)
            {
                HandleHeadBob();
            }

            if (canZoom)
            {
                HandleZoom();
            }

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if (useFootsteps)
            {
                HandleFootSteps();
            }

            if (useStamina)
            {
                HandleStamina();
            }

            ApplyFinalMovements();
        }
    }

    private bool canHit = true;
    private void HandleHitAnimation()
    {
        if (Input.GetMouseButton(0) && canHit)
        {
            canHit = false;
            if (!InteractableObjects.isArmedMelee)
            {
                animator.SetTrigger("hit");

            }
            else if (InteractableObjects.isArmedMelee)
            {
                animator.SetTrigger("hitMelee");
            }
            StartCoroutine(DelayHitCoroutine());
        }
    }

    private IEnumerator DelayHitCoroutine() // Corounite for delayng hit animation
    {
        yield return new WaitForSeconds(0.8f);
        canHit = true;
    }

    private void HandleSavingAndLoading()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventory.Save();
            equipment.Save();
            print("Saved");
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            inventory.Load();
            equipment.Load();
            print("Loaded");
        }
    }

    private void RunWalkAnim()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(characterController.velocity);
        float speedZ = Mathf.Abs(localVelocity.z);
        animator.SetFloat("moveSpeed", speedZ);
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : isCrouching ? crouchSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : isCrouching ? crouchSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        if(IsSprinting && currentInput != Vector2.zero && characterController.isGrounded)
        {
            speedUpParticleFX.Play();
        }
        else
        {
            speedUpParticleFX.Stop();
        }
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -lowerLookLimit, upperLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if(ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleCrouch()
    {
        StartCoroutine(StandCrouch());
    }

    private void HandleHeadBob()
    {
        if(!characterController.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);

            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z
            );
        }
    }

    private void HandleZoom()
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(TogleZoom(true));
        }

        if(Input.GetKeyUp(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(TogleZoom(false));
        }
    }

    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 7 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);
                if(currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if(currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }
    private void HandleInteractionInput()
    {
        if(Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),out RaycastHit hit, interactionDistance, currentInteractionLayer))
        {
            currentInteractable.OnInteract();
            animator.SetTrigger("pickUp");
            var item = hit.transform.GetComponent<GroundItem>();
            if(item)
            {
                if(inventory.AddItem(new Item(item.item), 1))
                {
                    Destroy(hit.transform.gameObject);
                }

                
            }
        }
    }

    private void HandleFootSteps()
    {
        if(!characterController.isGrounded) return;
        if(currentInput == Vector2.zero) return;

        footStepTimer -= Time.deltaTime;

        if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            switch (hit.collider.tag)
            {
                case "GrassWalk":
                footStepsAudioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
                    break;
                case "DirtWalk":
                footStepsAudioSource.PlayOneShot(dirtClips[UnityEngine.Random.Range(0, dirtClips.Length - 1)]);
                    break;
                case "WoodWalk":
                footStepsAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
                    break;
                case "WaterWalk":
                footStepsAudioSource.PlayOneShot(waterClips[UnityEngine.Random.Range(0, waterClips.Length - 1)]);
                    break; 
            }
        }

        footStepTimer = GetCurrentOffset;
    }

    private void HandleStamina()
    {
        if(IsSprinting && currentInput != Vector2.zero)
        {
            if(regeneratingStaminaCoroutine != null)
            {
                StopCoroutine(regeneratingStaminaCoroutine);
                regeneratingStaminaCoroutine = null;
            }
            currentStamina -= staminaUseMultiplier * Time.deltaTime;
            OnReduceStamina?.Invoke(currentStamina);

            if(currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
            }

            if(currentStamina <= 0)
            {
                canSprint = false;
            }
        }

        if(!IsSprinting && regeneratingStaminaCoroutine == null && currentStamina < maxStamina)
        {
            regeneratingStaminaCoroutine = StartCoroutine(RegenerateStamina());
        }
    }

    private void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth);
        if(currentHealth <= 0)
        {
            KillPlayer();
        }
        else if(regeneratingHealthCoroutine != null)
        {
            StopCoroutine(regeneratingHealthCoroutine);
        }

        regeneratingHealthCoroutine = StartCoroutine(RegenerateHealth());
    }

    private void KillPlayer()
    {
        currentHealth = 0;
        if(regeneratingHealthCoroutine != null)
        {
            StopCoroutine(RegenerateHealth());
        }

        print("YOU HAVE PASSED AWAY,DUMMY,YOU CANT MOVE!!!");
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        equipment.Container.Clear();
    }

    private void ApplyFinalMovements()
    {
        if(!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if(characterController.isGrounded && IsSliding)
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slideSpeed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);

        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement; 

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            
            OnHeal?.Invoke(currentHealth); // усіх підписників сповіщаємо про те,що подія викликана і як параметр передаємо currentHealth
            yield return timeToWait;
        }

        regeneratingHealthCoroutine = null;
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegens);
        WaitForSeconds incrementTime = new WaitForSeconds(staminaTimeIncrement);

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaValueIncrement;
            canSprint = true;
            OnRenewStamina?.Invoke(currentStamina);

            if(currentStamina >= maxStamina)
            {
                currentStamina = currentHealth;
            }

            yield return incrementTime;
        }

        regeneratingStaminaCoroutine = null;
        
    }

    private IEnumerator StandCrouch()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnimation = true;
        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null; // waits till the of the frame
        }

        characterController.center = targetCenter;
        characterController.height = targetHeight;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    private IEnumerator TogleZoom(bool isEntered)
    {
        float targetFOV = isEntered ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
