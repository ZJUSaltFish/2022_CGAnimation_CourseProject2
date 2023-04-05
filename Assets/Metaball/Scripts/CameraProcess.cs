using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraProcess : MonoBehaviour
{
    public Global global;//this scriptable object contains all paras needed.

    // Start is called before the first frame update
    void Start()
    {
        global.blobList = new GameObject[100];
        global.positionList = new Vector4[100];
        global.color = new Vector4[100];
        global.rList = new float[100];
        global.density = new float[100];
        global.roughness = new float[100];
        global.number = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!global.RayMarchingMat)//if no mat selected: no ray marching
        {
            Graphics.Blit(src, dest);
            return;
        }
        global.RayMarchingMat.SetMatrix("_InvViewM", Camera.main.cameraToWorldMatrix);
        global.RayMarchingMat.SetMatrix("_InvProjectionM", Camera.main.projectionMatrix.inverse);

        global.RayMarchingMat.SetVectorArray("_Blobs", global.positionList);
        global.RayMarchingMat.SetVectorArray("_Color", global.color);

        global.RayMarchingMat.SetFloatArray("_REffect", global.rList);
        global.RayMarchingMat.SetFloatArray("_Density", global.density);
        global.RayMarchingMat.SetFloatArray("_Roughness", global.roughness);
        
        global.RayMarchingMat.SetFloat("_Threshold", global.threshold);
        global.RayMarchingMat.SetFloat("_Accuracy", global.accuracy);       
        global.RayMarchingMat.SetFloat("_Ambient", global.ambient);
        global.RayMarchingMat.SetInt("_Number", global.number);
        global.RayMarchingMat.SetFloat("_Steps", global.steps);
        
        RenderTexture tmp = RenderTexture.GetTemporary(src.width, src.height);
        Graphics.Blit(src,tmp,global.RayMarchingMat, -1);
        Graphics.Blit(tmp,dest);
        RenderTexture.ReleaseTemporary(tmp);
    }
}
