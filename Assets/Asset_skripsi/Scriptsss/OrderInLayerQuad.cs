using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OrderInLayerQuad : MonoBehaviour
{
    public string sortingLayerName = "Ground"; // Or match your Tilemap layer
    public int orderInLayer = 1; // Higher number means render on top

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = orderInLayer;
    }
}
