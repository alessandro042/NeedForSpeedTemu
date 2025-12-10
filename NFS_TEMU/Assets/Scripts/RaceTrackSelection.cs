using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceTrackSelection : MonoBehaviour
{
    public void RaceTrackOne()
    {
        
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Usted sera llevado al LEVEL I.");
    }
    
    public void RaceTrackOTwo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        Debug.Log("Usted sera llevado al LEVEL II.");
    }
    
    public void RaceTrackTree()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
        Debug.Log("Usted sera llevado al LEVEL III.");
    }
}
