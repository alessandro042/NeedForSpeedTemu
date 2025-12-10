using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    [HideInInspector] public RaceManager raceManager; 
    
    
    public int index; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player")) 
        {
            
            raceManager.JugadorTocoCheckpoint(this);
        }
    }
}