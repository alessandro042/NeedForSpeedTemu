using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    [Header("Configuración")]
    public Text textoTiempo; 
    public GameObject panelFinal; 
    public Text textoTiempoFinal; 
    public Text textoMejorTiempo; // Arrastra aquí el texto dorado que acabas de crear

    [Header("Ruta (Arrastra los Checkpoints en orden)")]
    public List<Checkpoint> checkpoints; 

    private float tiempoActual;
    private bool carreraActiva = false;
    private int siguienteCheckpoint = 0; 

    void Start()
    {
        // 1. Configurar los checkpoints (Esto ya lo tenías)
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].raceManager = this;
            checkpoints[i].index = i;
        }

        // 2. Mostrar el récord guardado (ESTA ES LA LÍNEA NUEVA)
        ActualizarMejorTiempoUI(); 

        // 3. Arrancar la carrera
        IniciarCarrera();
    }

    void IniciarCarrera()
    {
        tiempoActual = 0f;
        siguienteCheckpoint = 0; 
        carreraActiva = true;
        if(panelFinal != null) panelFinal.SetActive(false);
    }

    void Update()
    {
        if (carreraActiva)
        {
            tiempoActual += Time.deltaTime;
            ActualizarRelojUI();
        }
    }

    public void JugadorTocoCheckpoint(Checkpoint cp)
    {
       
        
        if (cp.index == siguienteCheckpoint)
        {
            Debug.Log("Checkpoint " + cp.index + " validado.");
            siguienteCheckpoint++; // Ahora esperamos el siguiente

            
            if (siguienteCheckpoint >= checkpoints.Count)
            {
                FinalizarCarrera();
            }
        }
        else
        {
           
            Debug.Log("¡Trampa! Te saltaste un checkpoint o vas al revés.");
        }
    }

    void FinalizarCarrera()
    {
        carreraActiva = false;
    
        // 1. Leemos el récord anterior
        float recordAnterior = PlayerPrefs.GetFloat("MejorTiempo", 99999.0f);

        // 2. Comparamos: ¿El tiempo que acabas de hacer es MENOR (más rápido) que el anterior?
        if (tiempoActual < recordAnterior)
        {
            // ¡SÍ! Guardamos el nuevo tiempo en la memoria
            PlayerPrefs.SetFloat("MejorTiempo", tiempoActual);
            PlayerPrefs.Save(); // Confirmamos el guardado
        
            Debug.Log("¡NUEVO RÉCORD!");
            if (textoTiempoFinal != null)
                textoTiempoFinal.text = "¡NUEVO RÉCORD! " + FormatearTiempo(tiempoActual);
            
            // Actualizamos el texto dorado inmediatamente
            ActualizarMejorTiempoUI();
        }
        else
        {
            Debug.Log("No superaste el récord.");
            if (textoTiempoFinal != null)
                textoTiempoFinal.text = "Tiempo: " + FormatearTiempo(tiempoActual);
        }

        if (panelFinal != null) panelFinal.SetActive(true);
    }

    void ActualizarRelojUI()
    {
        if (textoTiempo != null)
            textoTiempo.text = FormatearTiempo(tiempoActual);
    }

   
    string FormatearTiempo(float tiempo)
    {
        int min = Mathf.FloorToInt(tiempo / 60);
        int sec = Mathf.FloorToInt(tiempo % 60);
        int mil = Mathf.FloorToInt((tiempo * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", min, sec, mil);
    }
    
    void ActualizarMejorTiempoUI()
    {
        // Leemos el tiempo guardado. Si no existe, usamos 9999 (un número muy alto)
        float mejorTiempo = PlayerPrefs.GetFloat("MejorTiempo", 99999.0f);

        if (mejorTiempo < 99999.0f)
        {
            // Si hay un récord real, lo mostramos
            if (textoMejorTiempo != null)
                textoMejorTiempo.text = "Récord: " + FormatearTiempo(mejorTiempo);
        }
        else
        {
            // Si es la primera vez que juegas
            if (textoMejorTiempo != null)
                textoMejorTiempo.text = "Récord: --:--";
        }
    }
}