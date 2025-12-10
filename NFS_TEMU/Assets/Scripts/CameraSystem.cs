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

       
       if (cameraTransform == null && Camera.main != null)
       {
           cameraTransform = Camera.main.transform;
           Debug.Log($"CameraSystem: cameraTransform auto-asignado a Camera.main ('{cameraTransform.name}')");
       }
    }
    
    void LateUpdate()
    {
        
        var t = target;

        if (t == null)
        {
            
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                target = go.transform;
                t = target;
                Debug.Log($"CameraSystem: Target reasignado automáticamente a '{t.name}'");
            }
            else
            {
                
                return;
            }
        }

        if (cameraTransform == null)
        {
            Debug.LogWarning("CameraSystem: 'cameraTransform' no está asignado.");
            return;
        }

        
        Vector3 targetPosition;
        try
        {
            
            if (t == null)
            {
                var go2 = GameObject.FindGameObjectWithTag("Player");
                if (go2 != null)
                {
                    target = go2.transform;
                    t = target;
                }
                else
                {
                    return;
                }
            }

            targetPosition = t.position;
        }
        catch (UnityEngine.MissingReferenceException)
        {
            
            var go3 = GameObject.FindGameObjectWithTag("Player");
            if (go3 != null)
            {
                target = go3.transform;
                Debug.Log("CameraSystem: Target destruido; reasignado a nuevo Player.");
            }
            return;
        }

        Vector3 direction = (targetPosition - cameraTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cameraTransform.rotation = targetRotation;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
        
        cameraTransform.localPosition = offset;
        
        var rotationDelta = Input.mousePositionDelta.x *Time.deltaTime * cameraRotationSpeed;
        var sign = isInverted ? -1 : 1;
        transform.Rotate(0f, rotationDelta, 0f);
    }
}
