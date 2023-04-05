using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metaball : MonoBehaviour
{
    public Global global;
    public int id{
        get{return _id; }
    }
    int _id;
    void Awake()
    {
        _id = global.number;
    }


}
