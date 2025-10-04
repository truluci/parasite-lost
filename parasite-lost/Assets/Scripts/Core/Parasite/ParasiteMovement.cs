using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class ParasiteMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Movement speed in units per second.")]
    public float moveSpeed = 5f;

    [Tooltip("If true, the sprite will rotate to face the movement direction.")]
    public bool faceMovementDirection = true;

    [Tooltip("Degrees per second for smoothing rotation. Use 0 for instant rotation.")]
    public float rotationSpeed = 720f;

    Rigidbody2D rb;

    Vector2 inputVector;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!IsPlayable())
        {
            inputVector = Vector2.zero;
            return;
        }
        inputVector = ReadInput();

        if (faceMovementDirection && inputVector.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;
            if (rotationSpeed <= 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle - 90f);
            }
            else
            {
                float desired = targetAngle - 90f; // subtract 90 if sprite faces up by default
                float smoothed = Mathf.MoveTowardsAngle(currentAngle, desired, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, smoothed);
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsPlayable())
        {
            if (rb.linearVelocity != Vector2.zero)
                rb.linearVelocity = Vector2.zero;
            return;
        }
        Vector2 velocity = inputVector * moveSpeed;
        rb.linearVelocity = velocity;
    }

    Vector2 ReadInput()
    {
        Vector2 v = Vector2.zero;

#if ENABLE_LEGACY_INPUT_MANAGER
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        v = new Vector2(x, y);
#else
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) v.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) v.x += 1f;
        }

        if (Gamepad.current != null)
        {
            v += Gamepad.current.leftStick.ReadValue();
        }
#else
        try
        {
            v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        catch (System.Exception)
        {
            v = Vector2.zero;
        }
#endif
#endif

        if (v.sqrMagnitude > 1f) v.Normalize();
        return v;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        rb.linearVelocity = newVelocity;
    }

    bool IsPlayable()
    {
        var gm = ParasiteLost.Managers.GameManager.Instance;
        if (gm == null) return true; // fallback: allow movement if no manager yet
        return gm.currentState == ParasiteLost.Managers.GameManager.GameState.Playing;
    }
}
