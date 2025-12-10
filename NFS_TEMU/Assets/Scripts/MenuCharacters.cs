using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacters", menuName = "Vehiculo")]
public class MenuCharacters : ScriptableObject
{
    public GameObject vehiculoJugable;
    public Sprite image;
    public string nombre;
}
