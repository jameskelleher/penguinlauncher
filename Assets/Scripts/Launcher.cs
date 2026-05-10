using UnityEngine;
using UnityEngine.InputSystem;


public class Launcher : MonoBehaviour
{
    public InputAction input;

    public float minHeight = -3.0f;
    public float maxHeight = 3.0f;

    public float inputMin = -1.0f;
    public float inputMax = -0.8f;

    public InputMode inputMode;

    private Rigidbody2D rb;
    private float dragStartY;
    private float dragRange;

    void Start()
    {
        if (Application.isMobilePlatform)
            inputMode = InputMode.Touch;

        input.Enable();

        rb = GetComponent<Rigidbody2D>();

        float worldUnitsInPixels = Screen.height / (Camera.main.orthographicSize * 2);
        float heightRange = maxHeight - minHeight;
        dragRange = heightRange * worldUnitsInPixels;

        dragStartY = Screen.height;
    }

    void Update()
    {
        bool pressedThisFrame =
            (inputMode == InputMode.Mouse && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (inputMode == InputMode.Touch && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

        if (pressedThisFrame)
        {
            float deadZone = Screen.height * 0.05f;
            float pressY = inputMode == InputMode.Mouse
                ? Mouse.current.position.ReadValue().y
                : Touchscreen.current.primaryTouch.position.ReadValue().y;
            dragStartY = pressY + deadZone;
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
                normalizedInput = Mathf.InverseLerp(inputMin, inputMax, input);
                result = Mathf.Lerp(minHeight, maxHeight, normalizedInput);
                break;
            case InputMode.Mouse:
            case InputMode.Touch:
            default:
                bool isPressed =
                    (inputMode == InputMode.Mouse && Mouse.current != null && Mouse.current.leftButton.isPressed) ||
                    (inputMode == InputMode.Touch && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed);

                if (isPressed)
                {
                    float screenY = inputMode == InputMode.Mouse
                        ? Mouse.current.position.ReadValue().y
                        : Touchscreen.current.primaryTouch.position.ReadValue().y;
                    float normalizedY = Mathf.InverseLerp(dragStartY, dragStartY + dragRange, screenY);
                    result = Mathf.Lerp(minHeight, maxHeight, normalizedY);
                }
                else
                {
                    dragStartY = Screen.height;
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