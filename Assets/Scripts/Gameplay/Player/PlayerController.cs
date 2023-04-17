using RoboRyanTron.Unite2017.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlatformServices;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    // private PlayerInput playerInput;

    [SerializeField] private FloatVariable HealthVariable;
    [SerializeField] private FloatVariable ManaVariable;
    [SerializeField] private FloatVariable StaminaVariable;

    private CharacterController characterController;

    [Header("Movmeent Settings")]
    [SerializeField] private float velocity = 5;
    [SerializeField] private float sprintModificator = 3;
    [SerializeField] private float staminaUse = 0.5f;
    [SerializeField] private LayerMask layerMask;

    [Header("Skill Settings")]
    [SerializeField] SkillSO JumpSkill;
    [SerializeField] SkillSO SprintSkill;

    [Header("Input Actions References")]
    [SerializeField] InputActionAsset playerInput;

    private float yMovement = -9.81f;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There is more than one instance of this!", gameObject);

        Instance = this;
        characterController = GetComponent<CharacterController>();
        PlatformUserStats.SetAchievement("playerControlled");
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {

        // var movementValue = new Vector2(Input.GetAxis("Horizontal"),  Input.GetAxis("Vertical"));
        var movementValue = playerInput.FindAction("Move").ReadValue<Vector2>();

        if (SprintSkill.IsActive && playerInput.FindAction("Sprint").ReadValue<float>() > 0f && StaminaVariable.Value > 0)
        {
            PlatformUserStats.SetAchievement("staminaUsing");
            movementValue *= sprintModificator;
            StaminaVariable.Value -= staminaUse * Time.deltaTime;
        }
        else
        {
            StaminaVariable.Value += Time.fixedDeltaTime;
            StaminaVariable.Value = Mathf.Clamp01(StaminaVariable.Value);
        }

        movementValue *= velocity;
        movementValue *= Time.deltaTime;

        characterController.Move(new Vector3(movementValue.x, yMovement * Time.deltaTime, movementValue.y));
        if (characterController.velocity.sqrMagnitude > 0.1)
            transform.forward = new Vector3(movementValue.x, 0f, movementValue.y);

        if (JumpSkill.IsActive && playerInput.FindAction("Jump").ReadValue<float>() > 0f && characterController.isGrounded)
            yMovement = 10f;

        yMovement = Mathf.Max(-9.81f, yMovement - Time.deltaTime * 30f);
    }
}
