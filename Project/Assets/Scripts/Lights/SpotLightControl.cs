using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class SpotlightControl : MonoBehaviour
{
    public Light2D spotlight; // Reference to the 2D spotlight
    public float rotationSpeed = 30f; // Rotation speed in degrees per second
    public float flashInterval = 1f; // Interval between flashes in seconds
    public float flashDuration = 0.2f; // Duration of each flash in seconds

    public float minRotationAngle = -45f; // Minimum rotation angle in degrees
    public float maxRotationAngle = 45f; // Maximum rotation angle in degrees

    //private bool isFlashing = false;
    private float currentRotationAngle;

    void Start()
    {
        if (spotlight == null)
        {
            spotlight = GetComponent<Light2D>();
        }
        currentRotationAngle = transform.eulerAngles.z;
        StartCoroutine(FlashLightRoutine());
    }

    void Update()
    {
        RotateSpotlight();
    }

    void RotateSpotlight()
    {
        float rotation = rotationSpeed * Time.deltaTime;
        currentRotationAngle += rotation;

        // Clamp the rotation angle within the specified range
        currentRotationAngle = Mathf.Clamp(currentRotationAngle, minRotationAngle, maxRotationAngle);

        transform.rotation = Quaternion.Euler(0, 0, currentRotationAngle);

        // Reverse rotation direction if the angle exceeds the bounds
        if (currentRotationAngle == minRotationAngle || currentRotationAngle == maxRotationAngle)
        {
            rotationSpeed = -rotationSpeed;
        }
    }

    IEnumerator FlashLightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flashInterval);
            StartCoroutine(FlashLight());
        }
    }

    IEnumerator FlashLight()
    {
        //isFlashing = true;
        spotlight.enabled = false;
        yield return new WaitForSeconds(flashDuration);
        spotlight.enabled = true;
        //isFlashing = false;
    }
}
