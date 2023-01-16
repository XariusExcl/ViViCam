using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Transform : MonoBehaviour
{
    public Transform ManagedObject;

    public UI_Vector3 UIPositionField;
    public UI_Vector3 UIRotationField;

    void Start()
    {
        UIPositionField.SetFields(ManagedObject.localPosition);
        UIRotationField.SetFields(ManagedObject.localRotation.eulerAngles);
    }
    
    public void UpdatePosition()
    {
        if (ManagedObject is not null)
            ManagedObject.localPosition = UIPositionField.Value;
    }

    public void UpdateRotation()
    {
        if (ManagedObject is not null)
            ManagedObject.localRotation = Quaternion.Euler(UIRotationField.Value);
    }
}
