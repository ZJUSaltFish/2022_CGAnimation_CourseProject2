using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraMove : MonoBehaviour
{
    [Range(0,100)]
    public float cameraSpeed = 10;
    public bool enableMove = true;
    float delX = 0 ;
    float delY = 0 ;
    bool isHold = false;
    float scroll;
    Vector3 focusPoint = Vector3.zero;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enableMove && Input.GetMouseButtonDown(1)){
            isHold = true;
        }else
        if(enableMove && Input.GetMouseButtonUp(1)){
            isHold = false;
        }else
        if(enableMove && isHold){
            delX = Input.GetAxis("Mouse X");
            delY = Input.GetAxis("Mouse Y");
            transform.RotateAround(focusPoint, Vector3.Cross(transform.forward, transform.up).normalized, delY * cameraSpeed);
            transform.RotateAround(focusPoint, Vector3.up, delX * cameraSpeed);
        }
        if(enableMove && (scroll = Input.GetAxis("Mouse ScrollWheel")) != 0){
            if((transform.localPosition.magnitude <50 && transform.localPosition.magnitude > 8)
             || (transform.localPosition.magnitude < 8 && scroll < 0)
             || (transform.localPosition.magnitude > 50 && scroll > 0)){
                transform.localPosition += transform.forward * scroll * cameraSpeed;
            }
        }
    }
}
