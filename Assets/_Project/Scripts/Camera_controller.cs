using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_controller : MonoBehaviour
{
    public float dragSpeed = 2.0f;       
    public float zoomSpeed = 2.0f;    
    public float minZoom = 2.0f;        
    public float maxZoom = 10.0f;        

    private Vector3 dragOrigin;

    void Update()
    {
        HandleDragging();
        HandleZooming();
    }

    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - Input.mousePosition;

            Vector3 move = new Vector3(difference.x, difference.y, 0);

            transform.position += move * dragSpeed * Time.deltaTime;

            dragOrigin = Input.mousePosition;
        }
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
    }
}

