using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Transform cameraTransform;
    public float cameraRotationSpeed = 50f;
    [SerializeField] private float cameraFollowSpeed = 5f;
    public bool isInverted;
    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
    }
    
    void LateUpdate()
    {
        Vector3 direction = (target.position - cameraTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cameraTransform.rotation = targetRotation;
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * cameraFollowSpeed);
        
        cameraTransform.localPosition = offset;
        
        var rotationDelta = Input.mousePositionDelta.x *Time.deltaTime * cameraRotationSpeed;
        var sign = isInverted ? -1 : 1;
        transform.Rotate(0f, rotationDelta, 0f);
    }
}

