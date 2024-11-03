using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform centerEyeAnchor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (centerEyeAnchor != null)
        {
            Quaternion headRotation = centerEyeAnchor.rotation;
            transform.rotation= headRotation;
        }
    }
}
