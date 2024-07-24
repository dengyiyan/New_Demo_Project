using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class FireLight : MonoBehaviour
{
    public Light2D fireLight; // Reference to the 2D light component
    public float intensityMin = 0.5f; // Minimum light intensity
    public float intensityMax = 1.5f; // Maximum light intensity
    public float flickerSpeed = 0.1f; // Speed of the flicker effect

    public Color color1 = new Color(1.0f, 0.64f, 0.0f); // Primary color of the fire
    public Color color2 = new Color(1.0f, 0.4f, 0.0f); // Secondary color of the fire

    private void Start()
    {
        if (fireLight == null)
        {
            fireLight = GetComponent<Light2D>();
        }

        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            float randomIntensity = Random.Range(intensityMin, intensityMax);
            fireLight.intensity = randomIntensity;

            Color randomColor = Color.Lerp(color1, color2, Random.Range(0f, 1f));
            fireLight.color = randomColor;

            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}