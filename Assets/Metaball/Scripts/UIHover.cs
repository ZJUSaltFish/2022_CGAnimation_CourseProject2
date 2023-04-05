using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIHover : MonoBehaviour
{

    SceneController controlScript;
    void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject())        {
            Camera.main.TryGetComponent<SceneController>(out controlScript);
            if(controlScript != null){
                controlScript.onUI = true;
            }
        }else{
            if(controlScript != null){
                controlScript.onUI= false;
            }
        }
        
    }
}