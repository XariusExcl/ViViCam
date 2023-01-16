using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObject : MonoBehaviour
{
    public UdpTrackingReceiver udpTrackingReceiver;
    public string DeviceName{get; set;}
    

    float[] transformFloats;

    void Start()
    {
        if (udpTrackingReceiver is null)
            udpTrackingReceiver = GameObject.FindObjectOfType<UdpTrackingReceiver>();

        transformFloats = new float[7];
    }
    
    void Update()
    {
        transformFloats = udpTrackingReceiver.GetTransformData(DeviceName);
        transform.localPosition = new Vector3(transformFloats[0], transformFloats[1], -transformFloats[2]);
        transform.localRotation = new Quaternion(transformFloats[4], transformFloats[5], transformFloats[6], transformFloats[3]);

        // Ugly fix (There might be a quaternion swizzle to achieve that)
        transform.localRotation = Quaternion.Euler(-transform.localEulerAngles.x, -transform.localEulerAngles.y, transform.localEulerAngles.z);
        // Clean fix ?
        // transform.localRotation = new Quaternion(transformFloats[5], -transformFloats[4], transformFloats[3], transformFloats[6]);
    }
}
