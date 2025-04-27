using UnityEngine;

public class GlassColor : MonoBehaviour
{
    public string colorName;
    public Renderer glassRenderer;

    void Start()
    {
        ApplyColor();
    }

    void ApplyColor()
    {
        if (glassRenderer == null)
            glassRenderer = GetComponent<Renderer>();

        if (glassRenderer != null)
        {
            Color color = Color.white;

            switch (colorName.ToLower())
            {
                case "red":
                    color = Color.red;
                    break;
                case "green":
                    color = Color.green;
                    break;
                case "blue":
                    color = Color.blue;
                    break;
                case "yellow":
                    color = Color.yellow;
                    break;
            }

            glassRenderer.material.color = color;
        }
        else
        {
            Debug.LogWarning("Glass Renderer not found on " + gameObject.name);
        }
    }
}
