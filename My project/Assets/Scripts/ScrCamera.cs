using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrCamera : MonoBehaviour
{
    private Camera camera_obj;
    private const float zoomSpeed = 10f;
    private Vector3 panOrigin;
    private bool moveFlag = false;


    private void Start()
    {
        camera_obj = GetComponent<Camera>();
    }

    void Update()
    {
        //Zooming
        camera_obj.orthographicSize = Mathf.Clamp(camera_obj.orthographicSize - zoomSpeed * Input.GetAxis("Mouse ScrollWheel"), 2f, 67.5f);

        //Panning
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            panOrigin = Input.mousePosition;
            moveFlag = true;
        }

        if (moveFlag)
        {
            transform.position -= camera_obj.ScreenToWorldPoint(Input.mousePosition) - camera_obj.ScreenToWorldPoint(panOrigin);
            panOrigin = Input.mousePosition;
        }

        if (!Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonUp(0))
        {
            moveFlag = false;
        }

        //Clamp to view
        var clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, camera_obj.orthographicSize * camera_obj.aspect, 240 - (camera_obj.orthographicSize * camera_obj.aspect));
        clampedPosition.y = Mathf.Clamp(transform.position.y, camera_obj.orthographicSize, 135 - camera_obj.orthographicSize);
        transform.position = clampedPosition;

    }
}
