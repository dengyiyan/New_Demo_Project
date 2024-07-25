using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class BrokenRoadLamp : MonoBehaviour
{
    public Light2D roadLampLight; // Reference to the 2D light component
    public float minTimeBetweenFlashes = 5f; // Minimum time between flashes
    public float maxTimeBetweenFlashes = 10f; // Maximum time between flashes
    public float flashDuration = 0.1f; // Duration of each flash

    private void Start()
    {
        if (roadLampLight == null)
        {
            roadLampLight = GetComponent<Light2D>();
        }

        StartCoroutine(FlashLightRoutine());
    }

    private IEnumerator FlashLightRoutine()
    {
        while (true)
        {
            // Light stays on for a random interval
            float randomInterval = Random.Range(minTimeBetweenFlashes, maxTimeBetweenFlashes);
            yield return new WaitForSeconds(randomInterval);

            // Flash the light on and off
            roadLampLight.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            roadLampLight.enabled = true;
        }
    }
}
