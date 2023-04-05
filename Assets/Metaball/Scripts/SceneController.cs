using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SceneController : MonoBehaviour
{
    public Global global;
    public GameObject metaball;
    
    #region UI
    public bool onUI = false;
    public GameObject AccuracyController;
    public GameObject StepsController;
    public GameObject ThresholdController;
    public GameObject PropertiesSetting;
    public GameObject DensityController;
    public GameObject RadiusController;
    public GameObject ColorController;
    public GameObject RoughnessController;
    #endregion

    int activeID;//the id of active metaball
    bool hold;
    Vector2 deltaMouse;//delta move of mouse
    Vector2 screenX,screenY,screenZ;//the projection of world xyz on the space.
    void Start()
    {
        InitializeSliders();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !onUI)
        {
            Select();
        }
        if(activeID >= 0)MoveBlob();
    }
    #region  about adding blobs
    public void AddBall(Vector3 pos)
    {
        GameObject newBall = GameObject.Instantiate(metaball, pos, new Quaternion(1,0,0,0));
        global.blobList[global.number] = newBall;
        global.positionList[global.number] = newBall.transform.position;
        global.rList[global.number] = 1.0f;
        global.density[global.number] = 1.0f;
        global.color[global.number] = new Vector4(0.5f,0.5f,0.5f,1.0f);
        global.roughness[global.number] = 0.5f;
        global.number += 1;
    }

    public void AddRandom()
    {
        float x = Random.Range(-5,5);
        float y = Random.Range(-5,5);
        float z = Random.Range(-1,1);
        AddBall(new Vector3(x,y,z));
    }

    public void AddAtCenter()
    {
        AddBall(Vector3.zero);
    }
    #endregion

    #region about moving blobs
    void Select()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200, 1 << 3))
        {
            //Vector3 hitPoint = ray.origin + ray.direction * hit.distance;
           //GameObject.Instantiate(selectPoint, hitPoint, transform.rotation);
           //paras.destination = hitPoint;
           activeID = hit.transform.gameObject.GetComponent<Metaball>().id;
           PropertiesSetting.SetActive(true);
           NewProperties();
        }else{
            activeID = -1;
            PropertiesSetting.SetActive(false);
        }
    }

    void MoveBlob()
    {
        if(Input.GetMouseButtonDown(2)){
            hold = true;
        }else if(Input.GetMouseButtonUp(2)){
            hold = false;
        }
        if(hold){
            deltaMouse = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
            if(deltaMouse.magnitude == 0)return;
            //Debug.Log(deltaMouse);
            Vector3 zeroPoint =  Camera.main.WorldToScreenPoint(Vector3.zero);
            screenZ = Camera.main.WorldToScreenPoint(Vector3.forward) - zeroPoint;
            //screenZ = screenZ.normalized;
            screenY = Camera.main.WorldToScreenPoint(Vector3.up) - zeroPoint;
            //screenY = screenY.normalized;
            screenX = Camera.main.WorldToScreenPoint(Vector3.right) - zeroPoint;
            //screenX = screenX.normalized;
            float dotX = Vector2.Dot(deltaMouse, screenX);
            float dotY = Vector2.Dot(deltaMouse, screenY);
            float dotZ = Vector2.Dot(deltaMouse, screenZ);
            float absX = Mathf.Abs(dotX);
            float absY = Mathf.Abs(dotY);
            float absZ = Mathf.Abs(dotZ);
            if(absX > absY && absX > absZ){//if absX is max: move along x
                global.blobList[activeID].transform.position += deltaMouse.magnitude * Vector3.right * dotX/absX * global.moveSpeed;
                global.positionList[activeID] = global.blobList[activeID].transform.position;
            }else if(absY > absZ){//if along Y
                global.blobList[activeID].transform.position += deltaMouse.magnitude * Vector3.up * dotY/absY * global.moveSpeed;
                global.positionList[activeID] = global.blobList[activeID].transform.position;
            }else{//is along Z
                global.blobList[activeID].transform.position += deltaMouse.magnitude * Vector3.forward * dotZ/absZ * global.moveSpeed;
                global.positionList[activeID] = global.blobList[activeID].transform.position;
            }
        }
    }
    #endregion

    #region about parameter settings
    void InitializeSliders()//initialize all the slider values
    {
        ThresholdController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.threshold;
        StepsController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.steps;
        AccuracyController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.accuracy;
        PropertiesSetting.SetActive(false);
    }

    void NewProperties()//this function will create a new property setting bar.
    {
        RadiusController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.rList[activeID];
        DensityController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.density[activeID];
        RoughnessController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = global.roughness[activeID];
        Vector3 hsv = RGB2HSV(global.color[activeID]);
        ColorController.transform.GetChild(0).gameObject.GetComponent<Slider>().value = hsv.x;
        ColorController.transform.GetChild(1).gameObject.GetComponent<Slider>().value = hsv.y;
        ColorController.transform.GetChild(2).gameObject.GetComponent<Slider>().value = hsv.z;
        ColorController.transform.GetChild(3).gameObject.GetComponent<Slider>().value = global.color[activeID].w;
        ColorController.transform.GetChild(4).GetComponent<Image>().color = global.color[activeID];
    }

    public void SetThreshold()//apply slider values to parameters
    {
        float value = ThresholdController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.threshold = value;
        ThresholdController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }

    public void SetSteps()
    {
        int value = (int)StepsController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.steps = value;
        StepsController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }

    public void SetAccuracy()
    {
        float value = AccuracyController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.accuracy = value;
        AccuracyController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }

    public void SetDensity()
    {
        float value = DensityController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.density[activeID] = value;
        DensityController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }

    public void SetRadius()
    {
        float value = RadiusController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.rList[activeID] = value;
        RadiusController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }

    public void SetColor()
    {
        float H = ColorController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        float S = ColorController.transform.GetChild(1).gameObject.GetComponent<Slider>().value;
        float V = ColorController.transform.GetChild(2).gameObject.GetComponent<Slider>().value;
        Vector3 hsv = new Vector3(H,S,V);
        Vector3 rgb = HSV2RGB(hsv); 
        global.color[activeID] = rgb;
        global.color[activeID].w = ColorController.transform.GetChild(3).gameObject.GetComponent<Slider>().value;
        Color examp = new Color(rgb.x, rgb.y, rgb.z ,global.color[activeID].w);
        ColorController.transform.GetChild(4).GetComponent<Image>().color = examp; 
    }

    public void SetRoughness()
    {
        float value = RoughnessController.transform.GetChild(0).gameObject.GetComponent<Slider>().value;
        global.roughness[activeID] = value;
        RoughnessController.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = value.ToString("f2");
    }
    Vector3 HSV2RGB(Vector3 hsv)
    {
        Vector3 rgb = new Vector3(0,0,0);
        float C = hsv.y * hsv.z;
        float X = C * (1 - Mathf.Abs((hsv.x / 60) % 2 - 1));
        float M = hsv.z -C;
        if(hsv.x < 60){
            rgb.x = hsv.z; rgb.y = X + M; rgb.z = M;
        }else if(hsv.x <120){
            rgb.y = hsv.z; rgb.x = X + M; rgb.z = M;
        }else if(hsv.x <180){
            rgb.y = hsv.z; rgb.z = X + M; rgb.x = M;
        }else if(hsv.x < 240){
            rgb.z = hsv.z; rgb.y = X + M; rgb.x = M;
        }else if(hsv.x < 300){
            rgb.z = hsv.z; rgb.x = X + M; rgb.y = M;
        }else{
            rgb.x = hsv.z; rgb.z = X + M; rgb.y = M;
        }
        return rgb;
    }

    Vector3 RGB2HSV(Vector3 rgb)
    {
        Vector3 hsv = new Vector3(0,0,0);
        float max = (rgb.x > rgb.y ? rgb.x : rgb.y) > rgb.z ? (rgb.x > rgb.y ? rgb.x : rgb.y) : rgb.z;
        float min = (rgb.x < rgb.y ? rgb.x : rgb.y) < rgb.z ? (rgb.x < rgb.y ? rgb.x : rgb.y) : rgb.z;
        float diff = max -min;
        if(rgb.x == rgb.y && rgb.y == rgb.z){
            hsv.x = 0;
        }else if(rgb.x >= rgb.y && rgb.y >= rgb.z){
            hsv.x = 60 * (rgb.y - rgb.z) / diff;
        }else if(rgb.x >= rgb.z && rgb.y < rgb.z){
            hsv.x = 60 * (rgb.y - rgb.z) / diff + 360;
        }else if(-rgb.y > rgb.z){
            hsv.x = 60 * (rgb.z - rgb.x) / diff + 120;
        }else{
            hsv.x = 60 * (rgb.x - rgb.y) / diff + 240;
        }
        if(max == 0){
            hsv.y = 0;
        }else{
            hsv.y = 1 - min/max;
        }
        hsv.z = max;
        return hsv;
    }
    #endregion
}
