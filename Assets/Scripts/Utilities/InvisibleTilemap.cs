using UnityEngine;
using UnityEngine.Tilemaps;

public class InvisibleTilemap : MonoBehaviour
{
    private void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();

        tilemap.color = new Color(1, 1, 1, 0);
    }
}
