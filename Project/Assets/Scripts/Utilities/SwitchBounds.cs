using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchConfinerShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchConfinerShape;
    }

    private void SwitchConfinerShape()
    {
        GameObject confinerObject = GameObject.FindGameObjectWithTag("BoundsConfiner");

        if (confinerObject != null)
        {
            PolygonCollider2D confinerShape = confinerObject.GetComponent<PolygonCollider2D>();

            if (confinerShape != null)
            {
                CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

                if (confiner != null)
                {
                    confiner.m_BoundingShape2D = confinerShape;
                    confiner.InvalidatePathCache();
                }
                else
                {
                    Debug.Log("CinemachineConfiner component not found on this GameObject.");
                }
            }
            else
            {
                Debug.Log("PolygonCollider2D component not found on the object with 'BoundsConfiner' tag.");
            }
        }
        else
        {
            Debug.Log("No object with 'BoundsConfiner' tag found.");
        }
    }
}

