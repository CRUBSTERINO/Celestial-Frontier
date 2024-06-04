using UnityEngine;

// Point on pathfinding surface that is used by NPCs when searching for a player
public class SearchPoint : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
