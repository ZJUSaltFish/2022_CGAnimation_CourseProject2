using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MetaballGlobal")]
public class Global : ScriptableObject
{
    public Material RayMarchingMat;//the raymarching material used to render metaballs
    public GameObject activeBlob;//the selected blob
    public GameObject[] blobList;//a list containing all the blobs

    #region Metabal raymarching parameters
    public Vector4[] positionList;//all positions
    public float[] rList;//list of effective radius
    public float[] density;//density list 
    public Vector4[] color;//color list
    public int number;//the number of metaballs
    public float threshold;//the throeshold that defines the equipotential surface
    public int steps;//the maximum steps of raymarching
    public float accuracy;//the steplength of raymarching
    #endregion

    #region shading parameters
    public float ambient;
    public float[] roughness;
    #endregion

    #region user interaction parameters
    public float moveSpeed;//the moving speed multiplyer of camera
    #endregion

    
}
