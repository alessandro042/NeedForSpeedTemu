using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] frontWheels;
    
    [Header("Asignación explícita de ruedas (opcional)")]
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform rearLeftWheel;
    [SerializeField] private Transform rearRightWheel;
    public enum WheelRotationAxis { LocalX, LocalY, LocalZ }
    public enum WheelSteerAxis { LocalX, LocalY, LocalZ }
     [Header("Rotación de las ruedas")]
     [Tooltip("Eje local que representa el eje de giro de la rueda (normalmente LocalX).")]
     [SerializeField] private WheelRotationAxis wheelRotationAxis = WheelRotationAxis.LocalX;
     [Tooltip("Multiplicador de signo para corregir giro invertido (-1 o 1)")]
     [SerializeField] private int wheelRotationSign = 1;
     [Tooltip("Eje local usado para el giro de dirección de las ruedas delanteras (steer).")]
     [SerializeField] private WheelSteerAxis frontWheelSteerAxis = WheelSteerAxis.LocalY;
     [Tooltip("Signo para invertir el giro de dirección si está al revés (1/-1)")]
     [SerializeField] private int frontWheelSteerSign = 1;
    [Header("Detección automática de ejes")]
    [Tooltip("Si está activo, el script intentará detectar automáticamente el eje de rotación (rolling) y el eje de steering según la orientación de las ruedas.")]
    [SerializeField] private bool autoDetectWheelAxes = true;
     [SerializeField] private float maxSpeed = 30f; 
     [SerializeField] private float steeringSpeed = 45f;
     [SerializeField] private float wheelRotationSpeed = 360f;
     [SerializeField] private float acceleration = 120f; 
    [SerializeField]
    [Tooltip("Ángulo máximo de giro en grados para las ruedas delanteras")]
    private float maxSteerAngle = 45f;
    [SerializeField]
    [Tooltip("Si está activado, se preservará la posición local inicial de cada rueda después de aplicar rotaciones (útil si el pivote de la malla no está centrado).")]
    private bool preserveWheelLocalPosition = true;
     private float _currentSpeed;

     private Rigidbody _rb;

    
    private readonly Dictionary<Transform, Quaternion> _initialLocalRotations = new Dictionary<Transform, Quaternion>();
     
     private readonly Dictionary<Transform, float> _wheelRollAngles = new Dictionary<Transform, float>();
     
     private readonly Dictionary<Transform, Vector3> _initialLocalPositions = new Dictionary<Transform, Vector3>();
     
     private readonly Dictionary<Transform, Transform> _wheelVisuals = new Dictionary<Transform, Transform>();
     
     private readonly Dictionary<Transform, Quaternion> _initialVisualLocalRotations = new Dictionary<Transform, Quaternion>();

    
    private float _lastDiagTime;
    private float _diagInterval = 1f; 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            

            Debug.Log($"CarController('{gameObject.name}'): Rigidbody encontrado. isKinematic={_rb.isKinematic}, mass={_rb.mass}, constraints={_rb.constraints}, useGravity={_rb.useGravity}, velocity={_rb.linearVelocity}");
        }
        else
        {
            Debug.LogWarning($"CarController('{gameObject.name}'): NO se encontró Rigidbody en el prefab.");
        }

        
        if ((wheels == null || wheels.Length == 0) || (frontWheels == null || frontWheels.Length == 0))
        {
            var childTransforms = GetComponentsInChildren<Transform>(true)
                .Where(t => t != this.transform)
                .ToArray();

            
            var candidates = childTransforms
                .Where(t => t.name.ToLower().Contains("wheel") || t.name.ToLower().Contains("llanta") || t.name.ToLower().Contains("rueda"))
                .ToList();

            if (candidates.Count == 0)
            {
                
                candidates = childTransforms.Where(t => t.GetComponent<MeshRenderer>() != null || t.GetComponentInChildren<MeshRenderer>() != null).ToList();
            }

            if (candidates.Count > 0)
            {
                var root = this.transform;

                
                Transform fl = null, fr = null, rl = null, rr = null;
                string[] flPatterns = new[] {"front-left","front_left","frontleft","fl","frontl","left-front","front left","delantera-izquierda","delantero-izquierda","delan-left","delanl"};
                string[] frPatterns = new[] {"front-right","front_right","fr","frontr","right-front","front right","delantera-derecha","delantero-derecha","delan-right","delanr"};
                string[] rlPatterns = new[] {"rear-left","rear_left","rearleft","rl","rear l","back-left","back_left","backleft","rear left","trasera-izquierda","trasero-izquierda"};
                string[] rrPatterns = new[] {"rear-right","rear_right","rearright","rr","back-right","back_right","backright","rear right","trasera-derecha","trasero-derecha"};

                Func<string,string[],bool> matchAny = (s, pats) => {
                    if (string.IsNullOrEmpty(s)) return false;
                    var ln = s.ToLower();
                    foreach (var p in pats) if (ln.Contains(p)) return true;
                    return false;
                };

                foreach (var c in candidates)
                {
                    var n = c.name.ToLower();
                    if (fl==null && matchAny(n, flPatterns)) fl = c;
                    if (fr==null && matchAny(n, frPatterns)) fr = c;
                    if (rl==null && matchAny(n, rlPatterns)) rl = c;
                    if (rr==null && matchAny(n, rrPatterns)) rr = c;
                }

                
                if (fl!=null && fr!=null && rl!=null && rr!=null)
                {
                    frontWheels = new[] { fl, fr };
                    wheels = new[] { fl, fr, rl, rr };
                }
                else
                {

                
                if (frontLeftWheel != null && frontRightWheel != null && rearLeftWheel != null && rearRightWheel != null)
                {
                    frontWheels = new[] { frontLeftWheel, frontRightWheel };
                    wheels = new[] { frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel };
                }
                else
                {
                    
                    if (candidates.Count >= 4)
                    {
                        var byZDesc = candidates.OrderByDescending(t => root.InverseTransformPoint(t.position).z).ToList();
                        var frontCandidates = byZDesc.Take(2).OrderBy(t => root.InverseTransformPoint(t.position).x).ToArray();
                        var byZAsc = candidates.OrderBy(t => root.InverseTransformPoint(t.position).z).ToList();
                        var rearCandidates = byZAsc.Take(2).OrderBy(t => root.InverseTransformPoint(t.position).x).ToArray();

                        frontWheels = frontCandidates;

                        
                        wheels = new[] { frontCandidates[0], frontCandidates[1], rearCandidates[0], rearCandidates[1] };
                    }
                    else
                    {
                        
                        if (wheels == null || wheels.Length == 0)
                            wheels = candidates.OrderBy(t => t.name).ToArray();

                        var frontByName = candidates.Where(t => t.name.ToLower().Contains("front") || t.name.ToLower().Contains("frente") || t.name.ToLower().Contains("delan")).ToArray();
                        if (frontByName.Length >= 2)
                            frontWheels = frontByName.Take(2).ToArray();
                        else
                        {
                            var orderedByZ = candidates.OrderByDescending(t => root.InverseTransformPoint(t.position).z).ToArray();
                            frontWheels = orderedByZ.Take(Math.Min(2, orderedByZ.Length)).ToArray();
                        }
                    }
                }
                }

                Debug.Log($"CarController('{gameObject.name}'): Detectados {wheels.Length} ruedas totales: {string.Join(", ", wheels.Select(w=>w.name))}");
                Debug.Log($"CarController('{gameObject.name}'): Detectados {frontWheels.Length} ruedas delanteras: {string.Join(", ", frontWheels.Select(w=>w.name))}");

                
                if (autoDetectWheelAxes && wheels.Length > 0)
                {
                    DetectWheelAxes();
                }
            }
            else
            {
                Debug.LogWarning($"CarController('{gameObject.name}'): No se detectaron ruedas automáticamente entre los hijos.");
            }
        }

        
        CacheInitialRotations();
    }

    private void CacheInitialRotations()
    {
        _initialLocalRotations.Clear();
        _wheelRollAngles.Clear();
        _initialLocalPositions.Clear();
        if (wheels != null)
        {
            foreach (var w in wheels)
            {
                if (w == null) continue;
                if (!_initialLocalRotations.ContainsKey(w))
                    _initialLocalRotations[w] = w.localRotation;
                if (!_initialLocalPositions.ContainsKey(w))
                    _initialLocalPositions[w] = w.localPosition;
                
                Transform visual = null;
                if (w.GetComponent<MeshRenderer>() != null || w.GetComponent<MeshFilter>() != null) visual = w;
                else
                {
                    var child = w.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t != w && (t.GetComponent<MeshRenderer>() != null || t.GetComponent<MeshFilter>() != null));
                    visual = child ?? w;
                }
                if (!_wheelVisuals.ContainsKey(w)) _wheelVisuals[w] = visual;
                if (!_initialVisualLocalRotations.ContainsKey(w)) _initialVisualLocalRotations[w] = (_wheelVisuals[w] != null) ? _wheelVisuals[w].localRotation : Quaternion.identity;
                 if (!_wheelRollAngles.ContainsKey(w))
                     _wheelRollAngles[w] = 0f;
            }
        }
        if (frontWheels != null)
        {
            foreach (var fw in frontWheels)
            {
                if (fw == null) continue;
                if (!_initialLocalRotations.ContainsKey(fw))
                    _initialLocalRotations[fw] = fw.localRotation;
                if (!_initialLocalPositions.ContainsKey(fw))
                    _initialLocalPositions[fw] = fw.localPosition;
                Transform visualFw = null;
                if (fw.GetComponent<MeshRenderer>() != null || fw.GetComponent<MeshFilter>() != null) visualFw = fw;
                else
                {
                    var childFw = fw.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t != fw && (t.GetComponent<MeshRenderer>() != null || t.GetComponent<MeshFilter>() != null));
                    visualFw = childFw ?? fw;
                }
                if (!_wheelVisuals.ContainsKey(fw)) _wheelVisuals[fw] = visualFw;
                if (!_initialVisualLocalRotations.ContainsKey(fw)) _initialVisualLocalRotations[fw] = (_wheelVisuals[fw] != null) ? _wheelVisuals[fw].localRotation : Quaternion.identity;
                 if (!_wheelRollAngles.ContainsKey(fw))
                     _wheelRollAngles[fw] = 0f;
            }
        }
    }

    
    void FixedUpdate()
    {
        
        if (Time.time - _lastDiagTime > _diagInterval)
        {
            _lastDiagTime = Time.time;
            if (_rb != null)
            {
                Debug.Log($"CarController('{gameObject.name}'): rb.velocity={_rb.linearVelocity} magnitude={_rb.linearVelocity.magnitude}");
            }
            Debug.Log($"CarController('{gameObject.name}'): Input W={InputHelper.GetKey(KeyCode.W)} S={InputHelper.GetKey(KeyCode.S)} Horizontal={InputHelper.GetHorizontal()}");
        }

        if (_rb == null) return;

        
        try
        {
            _rb.WakeUp();
            if (_rb.isKinematic)
            {
                _rb.isKinematic = false;
                Debug.Log($"CarController: Rigidbody en '{gameObject.name}' estaba kinematic; se forzó a false.");
            }
            if (!_rb.detectCollisions)
            {
                _rb.detectCollisions = true;
                Debug.Log($"CarController: detectCollisions activado en '{gameObject.name}'.");
            }
            if (_rb.mass > 500f)
            {
                Debug.LogWarning($"CarController: Rigidbody en '{gameObject.name}' tiene masa inusualmente alta ({_rb.mass}). Esto puede impedir movimiento con fuerzas pequeñas.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"CarController: Excepción al asegurar Rigidbody: {ex.Message}");
        }

        
        if (InputHelper.GetKey(KeyCode.W))
        {
            _rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }

        if (InputHelper.GetKey(KeyCode.S))
        {
            _rb.AddForce(-transform.forward * acceleration, ForceMode.Acceleration);
        }

        
        var horizontal = InputHelper.GetHorizontal();
        if ((InputHelper.GetKey(KeyCode.W) || InputHelper.GetKey(KeyCode.S) || Mathf.Abs(horizontal) > 0.1f) && _rb.linearVelocity.magnitude < 0.02f)
        {
            _rb.AddForce(transform.forward * 0.5f, ForceMode.VelocityChange);
            Debug.Log($"CarController: impulso inicial aplicado a '{gameObject.name}' para arrancar.");
        }

        
        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }

        var normalizedSpeed = _rb.linearVelocity.magnitude * 2f / (maxSpeed > 0 ? maxSpeed : 1f);
        var carRotation = horizontal * steeringSpeed * normalizedSpeed;
        transform.Rotate(0, carRotation * Time.fixedDeltaTime, 0);

        
        if (wheels == null || wheels.Length == 0)
        {
            Debug.LogWarning($"CarController('{gameObject.name}'): 'wheels' no está asignado o está vacío. Asigne los Transforms de las ruedas en el Inspector para ver la rotación visual.");
        }
        else
        {
            float rotationAmount = _rb.linearVelocity.magnitude * wheelRotationSpeed * Time.fixedDeltaTime;
            foreach (var wheel in wheels)
            {
                if (wheel == null) continue;

                
                float signedAmount = rotationAmount * Mathf.Sign(wheelRotationSign == 0 ? 1 : wheelRotationSign);
                if (!_wheelRollAngles.ContainsKey(wheel)) _wheelRollAngles[wheel] = 0f;
                _wheelRollAngles[wheel] += signedAmount;

                
                Quaternion initRot = _initialLocalRotations.ContainsKey(wheel) ? _initialLocalRotations[wheel] : wheel.localRotation;
                Vector3 initEuler = initRot.eulerAngles;

                
                 Vector3 steerDelta = Vector3.zero;
                 if (frontWheels != null && frontWheels.Contains(wheel))
                 {
                    float steerSignGlobal = (frontWheelSteerSign == 0 ? 1 : frontWheelSteerSign);
                    float steerAngle = horizontal * maxSteerAngle * steerSignGlobal;
                    
                    float sideSign = 0f;
                    if (frontLeftWheel != null && wheel == frontLeftWheel) sideSign = -1f;
                    else if (frontRightWheel != null && wheel == frontRightWheel) sideSign = 1f;
                    else
                    {
                        float sideX = transform.InverseTransformPoint(wheel.position).x;
                        sideSign = sideX >= 0f ? 1f : -1f;
                    }
                    float appliedSteer = steerAngle * sideSign;

                    switch (frontWheelSteerAxis)
                    {
                        case WheelSteerAxis.LocalX:
                            steerDelta.x = appliedSteer;
                            break;
                        case WheelSteerAxis.LocalY:
                            steerDelta.y = appliedSteer;
                            break;
                        case WheelSteerAxis.LocalZ:
                            steerDelta.z = appliedSteer;
                            break;
                    }
                 }

                
                var baseRot = Quaternion.Euler(initEuler + steerDelta);
                
                wheel.localRotation = Quaternion.Slerp(wheel.localRotation, baseRot, Time.fixedDeltaTime * 10f);
                
                Transform visualTransform = _wheelVisuals.ContainsKey(wheel) ? _wheelVisuals[wheel] : wheel;
                if (visualTransform == null) visualTransform = wheel;
                Quaternion initVisualRot = _initialVisualLocalRotations.ContainsKey(wheel) ? _initialVisualLocalRotations[wheel] : visualTransform.localRotation;
                
                Quaternion rollLocalQuat;
                switch (wheelRotationAxis)
                {
                    case WheelRotationAxis.LocalX:
                        rollLocalQuat = Quaternion.Euler(_wheelRollAngles[wheel], 0f, 0f);
                        break;
                    case WheelRotationAxis.LocalY:
                        rollLocalQuat = Quaternion.Euler(0f, _wheelRollAngles[wheel], 0f);
                        break;
                    default:
                        rollLocalQuat = Quaternion.Euler(0f, 0f, _wheelRollAngles[wheel]);
                        break;
                }
                visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, initVisualRot * rollLocalQuat, Time.fixedDeltaTime * 10f);
                 
                 if (preserveWheelLocalPosition && _initialLocalPositions.ContainsKey(wheel))
                 {
                     wheel.localPosition = _initialLocalPositions[wheel];
                 }
             }
         }
     }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (wheels != null)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                var w = wheels[i];
                if (w == null) continue;
                Gizmos.DrawSphere(w.position, 0.05f);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(w.position, w.position + w.forward * 0.2f);
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(w.position, w.position + w.right * 0.2f);
                Gizmos.color = Color.cyan;
            }
        }

        if (frontWheels != null)
        {
            Gizmos.color = Color.green;
            foreach (var fw in frontWheels)
            {
                if (fw == null) continue;
                Gizmos.DrawWireSphere(fw.position, 0.06f);
            }
        }
    }

    private void DetectWheelAxes()
    {
        
        var sample = wheels.FirstOrDefault(w => w != null);
        if (sample == null) return;

        var carRight = transform.right.normalized;
        var carUp = transform.up.normalized;

        
        var wr = sample.right.normalized;
        var wu = sample.up.normalized;
        var wf = sample.forward.normalized;

        
        float dotR = Mathf.Abs(Vector3.Dot(wr, carRight));
        float dotU = Mathf.Abs(Vector3.Dot(wu, carRight));
        float dotF = Mathf.Abs(Vector3.Dot(wf, carRight));
        if (dotR >= dotU && dotR >= dotF) { wheelRotationAxis = WheelRotationAxis.LocalX; wheelRotationSign = Vector3.Dot(wr, carRight) >= 0 ? 1 : -1; }
        else if (dotU >= dotR && dotU >= dotF) { wheelRotationAxis = WheelRotationAxis.LocalY; wheelRotationSign = Vector3.Dot(wu, carRight) >= 0 ? 1 : -1; }
        else { wheelRotationAxis = WheelRotationAxis.LocalZ; wheelRotationSign = Vector3.Dot(wf, carRight) >= 0 ? 1 : -1; }

        
        dotR = Mathf.Abs(Vector3.Dot(wr, carUp));
        dotU = Mathf.Abs(Vector3.Dot(wu, carUp));
        dotF = Mathf.Abs(Vector3.Dot(wf, carUp));
        if (dotR >= dotU && dotR >= dotF) { frontWheelSteerAxis = WheelSteerAxis.LocalX; frontWheelSteerSign = Vector3.Dot(wr, carUp) >= 0 ? 1 : -1; }
        else if (dotU >= dotR && dotU >= dotF) { frontWheelSteerAxis = WheelSteerAxis.LocalY; frontWheelSteerSign = Vector3.Dot(wu, carUp) >= 0 ? 1 : -1; }
        else { frontWheelSteerAxis = WheelSteerAxis.LocalZ; frontWheelSteerSign = Vector3.Dot(wf, carUp) >= 0 ? 1 : -1; }

        Debug.Log($"CarController: Auto-detected wheelRotationAxis={wheelRotationAxis} sign={wheelRotationSign}, frontWheelSteerAxis={frontWheelSteerAxis} sign={frontWheelSteerSign}");
    }
}
