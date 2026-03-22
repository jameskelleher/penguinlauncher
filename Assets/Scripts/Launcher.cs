using UnityEngine;
using UnityEngine.InputSystem;


public class Launcher : MonoBehaviour
{
    public InputAction input;

    public float minHeight = -3.0f;
    public float maxHeight = 3.0f;

    public float inputHigh = -0.8f;

    public InputMode inputMode;

    private Rigidbody2D rb;
    private float touchStartY;
    private float maxTouchInputY;

    void Start()
    {
        if (Application.isMobilePlatform)
            inputMode = InputMode.Touch;

        input.Enable();

        rb = GetComponent<Rigidbody2D>();

        maxTouchInputY = Screen.height * 0.95f;
        touchStartY = maxTouchInputY;
    }

    void Update()
    {
        if (inputMode == InputMode.Touch &&
            Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            float deadZone = Screen.height * 0.05f;
            touchStartY = Touchscreen.current.primaryTouch.position.ReadValue().y + deadZone;
            touchStartY = Mathf.Min(touchStartY, maxTouchInputY);
        }
    }

    void FixedUpdate()
    {
        float result;
        float normalizedInput;

        switch (inputMode)
        {
            case InputMode.Controller:
                float input = this.input.ReadValue<float>();
                normalizedInput = Mathf.InverseLerp(-1.0f, inputHigh, input);
                result = Mathf.Lerp(minHeight, maxHeight, normalizedInput);
                break;
            case InputMode.Mouse:
                result = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;
                break;
            case InputMode.Touch:
            default:
                if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                {

                    float screenY = Touchscreen.current.primaryTouch.position.ReadValue().y;
                    float normalizedY = Mathf.InverseLerp(touchStartY, maxTouchInputY, screenY);
                    Debug.Log(normalizedY);
                    result = Mathf.Lerp(minHeight, maxHeight, normalizedY);
                }
                else
                {
                    touchStartY = maxTouchInputY;
                    result = minHeight;
                }
                break;
        }

        result = Mathf.Clamp(result, minHeight, maxHeight);
        Vector2 newPos = new Vector2(transform.position.x, result);
        rb.MovePosition(newPos);
    }

    public enum InputMode { Controller, Mouse, Touch }
}
