using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuCharacterSelection : MonoBehaviour
{
    private int index;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nombre;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        index = PlayerPrefs.GetInt("JugadorIndex");
        if (index > gameManager.vehiculos.Count - 1)
        {
            index = 0;
        }
        
        ChangeScreen();
    }

    private void ChangeScreen()
    {
        PlayerPrefs.SetInt("JugadorIndex", index);
        image.sprite = gameManager.vehiculos[index].image;
        nombre.text = gameManager.vehiculos[index].nombre;
    }

    public void NextVehicule()
    {
        if (index == gameManager.vehiculos.Count - 1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }
        
        ChangeScreen();
    }
    
    public void LastVehicule()
    {
        if (index == 0)
        {
            index = gameManager.vehiculos.Count - 1;
        }
        else
        {
            index -= 1;
        }
        
        ChangeScreen();
    }

    public void BtnContinue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
