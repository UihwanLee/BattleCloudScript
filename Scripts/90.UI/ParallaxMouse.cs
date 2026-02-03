using UnityEngine;

public class ParallaxMouse : MonoBehaviour
{
    public Transform[] layers;
    public float[] parallaxFactors;

    private Vector2 screenCenter;
    private Vector3[] basePositions;

    void Start()
    {
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        basePositions = new Vector3[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] != null)
                basePositions[i] = layers[i].localPosition;
        }
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 offset = (mousePos - screenCenter) / screenCenter;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] != null)
            {
                Vector3 targetPos = basePositions[i] + new Vector3(
                    -offset.x * parallaxFactors[i],
                    -offset.y * parallaxFactors[i],
                    0
                );

                layers[i].localPosition = targetPos;
            }
        }
    }
}
