using UnityEngine;
using UnityEngine.InputSystem; // 1. ¡Importante! Añadimos el nuevo sistema de Input

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    // Variables públicas para ajustar en Unity
    public float moveSpeed = 1500f;
    public float turnSpeed = 400f;

    // Variables privadas
    private Rigidbody rb;
    private PlayerControls playerControls; // 2. Referencia a nuestro archivo PlayerControls

    // Variables para guardar los valores de input
    private float horizontalInput; // Para Girar (A/D)
    private float verticalInput;   // Para Acelerar (W)
    private float brakeInput;      // Para Frenar (S)

    // Awake() se llama antes que Start()
    void Awake()
    {
        // 3. Obtenemos el Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // 4. Creamos una NUEVA instancia de nuestros controles
        playerControls = new PlayerControls();
    }

    // 5. Se llama cuando el script se activa
    void OnEnable()
    {
        // Activamos nuestro "mapa de acciones" (el que llamamos "Carrera")
        playerControls.Race.Enable();
    }

    // 6. Se llama cuando el script se desactiva
    void OnDisable()
    {
        // Desactivamos el mapa de acciones para evitar problemas
        playerControls.Race.Disable();
    }

    // Update() se llama en cada frame
    
    void Update()
    {
        // 7. Leemos el valor (float) de cada acción que creamos
        horizontalInput = playerControls.Race.Girar.ReadValue<float>();
        verticalInput = playerControls.Race.Acelerar.ReadValue<float>();
        brakeInput = playerControls.Race.Frenar.ReadValue<float>();
        
        Debug.Log("Valor de Giro: " + horizontalInput);
    }

    // FixedUpdate() se llama en intervalos de física
    
    void FixedUpdate()
    {
        // 8. ¡La lógica clave!
        // Restamos el freno de la aceleración.
        // - Si presionas W: (1.0 - 0.0) = 1 (Acelera)
        // - Si presionas S: (0.0 - 1.0) = -1 (Frena/Reversa)
        // - Si no presionas nada: (0.0 - 0.0) = 0 (Neutral)
        float combinedMoveInput = verticalInput - brakeInput;

        // 1. Aceleración y Freno/Reversa (aplicado al eje Z local)
        rb.AddForce(transform.forward * combinedMoveInput * moveSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);

        // 2. Giro (aplicado al eje Y local)
        rb.AddTorque(transform.up * horizontalInput * turnSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
}