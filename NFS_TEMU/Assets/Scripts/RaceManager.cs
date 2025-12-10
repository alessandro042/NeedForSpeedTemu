using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    [Header("Configuración")]
    public Text textoTiempo; 
    public GameObject panelFinal; 
    public Text textoTiempoFinal; 
    public Text textoMejorTiempo; 

    [Header("Ruta (Arrastra los Checkpoints en orden)")]
    public List<Checkpoint> checkpoints; 

    private float tiempoActual;
    private bool carreraActiva = false;
    private int siguienteCheckpoint = 0; 

    void Start()
    {
        
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].raceManager = this;
            checkpoints[i].index = i;
        }

        
        ActualizarMejorTiempoUI(); 

        
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
            siguienteCheckpoint++; 

            
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
    
        
        float recordAnterior = PlayerPrefs.GetFloat("MejorTiempo", 99999.0f);

        
        if (tiempoActual < recordAnterior)
        {
            
            PlayerPrefs.SetFloat("MejorTiempo", tiempoActual);
            PlayerPrefs.Save(); 
        
            Debug.Log("¡NUEVO RÉCORD!");
            if (textoTiempoFinal != null)
                textoTiempoFinal.text = "¡NUEVO RÉCORD! " + FormatearTiempo(tiempoActual);
            
            
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
        
        float mejorTiempo = PlayerPrefs.GetFloat("MejorTiempo", 99999.0f);

        if (mejorTiempo < 99999.0f)
        {
            
            if (textoMejorTiempo != null)
                textoMejorTiempo.text = "Récord: " + FormatearTiempo(mejorTiempo);
        }
        else
        {
            
            if (textoMejorTiempo != null)
                textoMejorTiempo.text = "Récord: --:--";
        }
    }
}