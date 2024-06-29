using UnityEngine;
using UnityEngine.InputSystem;
using EmotivUnityPlugin;
using UnityEngine.XR;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;

public class LocomotionManager : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float runSpeed;
    public int lookThreshold;
    public float rotationCooldown;
    public float rotationAngle;

    private int mentalAction;
    private string eyeAction;
    private bool isInputEnabled = true;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float lastRotationTime;
    private int lookRightCount;
    private int lookLeftCount;

    private Vector2 leftStick;
    private Vector2 rightStick;

    private UnityEngine.XR.InputDevice leftHand;
    private UnityEngine.XR.InputDevice rightHand;

    private void Start()
    {
        InitializePositionAndRotation();
        InitializeXRDevices();
    }

    private void Update()
    {
        if (HandleResetInput()) return;
        if (HandleToggleInput()) return;
        if (!isInputEnabled) return;

        UpdateInputs();
        HandleRotation();
        HandleLocomotion();
    }

    private void InitializePositionAndRotation()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void InitializeXRDevices()
    {
        leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private bool HandleResetInput()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetObject();
            return true;
        }
        return false;
    }

    private bool HandleToggleInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ToggleInput();
            return true;
        }
        return false;
    }

    private void ToggleInput()
    {
        isInputEnabled = !isInputEnabled;
        Debug.Log($"Input is {(isInputEnabled ? "enabled" : "disabled")}");
    }

    private void UpdateInputs()
    {
        mentalAction = UDPReceiver.receivedInt;
        eyeAction = EmotivUnityItf.EyeAction;

        UpdateControllerInputs();
    }

    private void UpdateControllerInputs()
    {
        leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);
        rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);
    }

    private void HandleRotation()
    {
        if (ShouldRotateRight())
        {
            LookRightDetected();
        }
        else if (ShouldRotateLeft())
        {
            LookLeftDetected();
        }
    }

    private bool ShouldRotateRight()
    {
        return eyeAction == "lookR" || Keyboard.current.dKey.wasPressedThisFrame || rightStick.x > 0.1f;
    }

    private bool ShouldRotateLeft()
    {
        return eyeAction == "lookL" || Keyboard.current.aKey.wasPressedThisFrame || rightStick.x < -0.1f;
    }

    private void HandleLocomotion()
    {
        Vector3 movement = CalculateMovement();
        ApplyMovement(movement);
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = Vector3.zero;

        if (ShouldMoveForward())
        {
            movement += transform.forward;
        }
        else if (ShouldMoveBackward())
        {
            movement -= transform.forward;
        }

        return movement;
    }

    private bool ShouldMoveForward()
    {
        return mentalAction == 2 || Keyboard.current.wKey.isPressed || leftStick.y > 0.1f;
    }

    private bool ShouldMoveBackward()
    {
        return Keyboard.current.sKey.isPressed || leftStick.y < -0.1f;
    }

    private void ApplyMovement(Vector3 movement)
    {
        transform.position += movement.normalized * runSpeed * Time.deltaTime;
    }

    private void LookRightDetected()
    {
        lookRightCount++;
        if (CanRotate())
        {
            RotateR();
            ResetLookCounts();
        }
    }

    private void LookLeftDetected()
    {
        lookLeftCount++;
        if (CanRotate())
        {
            RotateL();
            ResetLookCounts();
        }
    }

    private bool CanRotate()
    {
        return (lookRightCount >= lookThreshold || lookLeftCount >= lookThreshold) && 
               (Time.time - lastRotationTime >= rotationCooldown);
    }

    private void RotateR()
    {
        Rotate(rotationAngle);
    }

    private void RotateL()
    {
        Rotate(-rotationAngle);
    }

    private void Rotate(float angle)
    {
        transform.Rotate(0f, angle, 0f);
        lastRotationTime = Time.time;
    }

    private void ResetLookCounts()
    {
        lookRightCount = 0;
        lookLeftCount = 0;
    }

    private void ResetObject()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        isInputEnabled = false;
        Debug.Log("Object reset and input disabled");
    }
}