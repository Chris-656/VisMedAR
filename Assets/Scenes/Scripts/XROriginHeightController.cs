using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class XROriginHeightController : MonoBehaviour
{
    public float heightSpeed = 1.0f; // Geschwindigkeit der Höhenänderung
    public float rotationSpeed = 60f; // Rotationsgeschwindigkeit in Grad pro Sekunde
    public float panSpeed = 1.0f; // Geschwindigkeit für Panning
    private XROrigin xrOrigin;
    private float targetHeight;
    private float targetYRotation;
    private Vector3 targetPan;

    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
        if (xrOrigin != null)
        {
            var pos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
            targetHeight = pos.y;
            targetYRotation = xrOrigin.CameraFloorOffsetObject.transform.localEulerAngles.y;
            targetPan = new Vector3(pos.x, 0f, pos.z);
        }
    }

    void Update()
    {
        // Rechter Thumbstick: Höhe und Rotation
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightThumbstick))
        {
            if (xrOrigin != null)
            {
                if (Mathf.Abs(rightThumbstick.y) > 0.1f)
                {
                    targetHeight += rightThumbstick.y * heightSpeed * Time.deltaTime;
                }
                if (Mathf.Abs(rightThumbstick.x) > 0.1f)
                {
                    targetYRotation += rightThumbstick.x * rotationSpeed * Time.deltaTime;
                }
                targetYRotation = Mathf.Repeat(targetYRotation, 360f);
            }
        }

        // Linker Thumbstick: Panning
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftThumbstick))
        {
            if (xrOrigin != null)
            {
                if (leftThumbstick.magnitude > 0.1f)
                {
                    // Panning in lokale X/Z Richtung
                    targetPan.x += leftThumbstick.x * panSpeed * Time.deltaTime;
                    targetPan.z += leftThumbstick.y * panSpeed * Time.deltaTime;
                }
            }
        }

        // Position und Höhe direkt setzen
        var pos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
        pos.x = targetPan.x;
        pos.z = targetPan.z;
        pos.y = targetHeight;
        xrOrigin.CameraFloorOffsetObject.transform.localPosition = pos;

        // Rotation direkt setzen
        xrOrigin.CameraFloorOffsetObject.transform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
    }
}
