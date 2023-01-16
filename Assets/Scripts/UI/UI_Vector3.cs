using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UI_Vector3 : MonoBehaviour
{
    public TMP_InputField xInputField;
    public TMP_InputField yInputField;
    public TMP_InputField zInputField;
    

    public Vector3 Value{get; private set;}
  
    [Space(10)]
  
    public UnityEvent OnValueChanged;

    public void SetFields(Vector3 value)
    {   
        xInputField.text = value.x.ToString();
        yInputField.text = value.y.ToString();
        zInputField.text = value.z.ToString();
    }

    public void UpdateValue()
    {
        Value = new Vector3(
            float.Parse(xInputField.text),
            float.Parse(yInputField.text),
            float.Parse(zInputField.text)
        );

        OnValueChanged.Invoke();
    }

    public float XValue{get; private set;}
    public float YValue{get; private set;}
    public float ZValue{get; private set;}
}
