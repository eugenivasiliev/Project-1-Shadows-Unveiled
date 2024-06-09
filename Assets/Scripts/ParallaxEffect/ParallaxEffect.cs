using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera mainCamera;
    public float smoothing = 1f;
    private Vector3 lastCameraPosition;
    private Transform[] layers;

    private void Awake()
    {
        lastCameraPosition = mainCamera.transform.position;
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        Vector3 mainCameraPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10f);
        
        float deltaX = mainCameraPosition.x - lastCameraPosition.x;
        float deltaY =  mainCameraPosition.y - lastCameraPosition.y;

        for (int i = 0; i < layers.Length; i++)
        {
            float parallaxX = deltaX * (i + 1) * smoothing / layers.Length;
            float parallaxY = deltaY * (i + 1) * smoothing / layers.Length;

            Vector3 newPosition = layers[i].position + new Vector3(parallaxX, parallaxY, 0);
            layers[i].position = Vector3.Lerp(layers[i].position, newPosition, smoothing * Time.deltaTime);
        }

        lastCameraPosition = mainCamera.transform.position;
    }
}
