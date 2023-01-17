using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScreenManager : MonoBehaviour
{
    public TrackedObject trackedObject;
    [Space(10)]
    public UdpTrackingReceiver udpTrackingReceiver;
    public TMP_Dropdown trackingSourceDropdown; 
    public RawImage trackingServerStatus;
    public TMP_Text trackingServerStatusText;
    public RawImage performanceStatus;
    public TMP_Text performanceStatusText;
    public new Camera camera;

    List<TMP_Dropdown.OptionData> dropdownOptions;

    void Start()
    {
        if (udpTrackingReceiver is null)
            udpTrackingReceiver = GameObject.FindObjectOfType<UdpTrackingReceiver>();

        dropdownOptions = new List<TMP_Dropdown.OptionData>();
        if (camera is null)
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    float _trackingToFrameRatio;
    float _frameRate;
    bool _isFirstFrameofConnected = true;
    void Update()
    {
        if(udpTrackingReceiver.IsServerConnected)
        {
            if (_isFirstFrameofConnected){
                UpdateTrackingSources();
                _isFirstFrameofConnected = false;
            }

            _trackingToFrameRatio = 1000f * Time.deltaTime / udpTrackingReceiver.MeasuredTickTime;

            if (_trackingToFrameRatio > 2f)
                trackingServerStatus.color = Color.green;
            else if (_trackingToFrameRatio > 1f)
                trackingServerStatus.color = Color.yellow;
            else
                trackingServerStatus.color = Color.red;

            trackingServerStatusText.text = $"Serveur connecté ({udpTrackingReceiver.MeasuredFrequency}Hz)";
        } else {
            UpdateTrackingSources();
            _isFirstFrameofConnected = true;
            trackingServerStatus.color = Color.red;
            trackingServerStatusText.text = "Serveur de tracking déconnecté";
        }

        _frameRate = 1f/Time.deltaTime;
        
        if (_frameRate > 50f)
            performanceStatus.color = Color.green;
        else if (_frameRate > 25f)
            performanceStatus.color = Color.yellow;
        else
            performanceStatus.color = Color.red;

        performanceStatusText.text = $"Performance : {((int)_frameRate).ToString()} ips";
    }

    void UpdateTrackingSources()
    {
        dropdownOptions = new List<TMP_Dropdown.OptionData>();
        if (udpTrackingReceiver.IsServerConnected)
        {
            trackingSourceDropdown.interactable = true;
            /*
            List<string> _currentDropdownOptions = new List<string>();
            foreach(TMP_Dropdown.OptionData option in trackingSourceDropdown.options)
            {
                _currentDropdownOptions.Add(option.text);
            }
            */

            foreach(string option in udpTrackingReceiver.GetTrackingDevicesNames())
            {
                // Debug.Log(option);
                // if (!_currentDropdownOptions.Contains(option))
                    dropdownOptions.Add(new TMP_Dropdown.OptionData(option));
            }

        } else {
            dropdownOptions.Add(new TMP_Dropdown.OptionData("Aucune (déconnecté)"));
            trackingSourceDropdown.interactable = false;
        }
        trackingSourceDropdown.options = dropdownOptions;
    }

    public void SetTrackingSource(int source)
    {
        trackedObject.DeviceName = dropdownOptions[source].text;
        Debug.Log($"Set tracking source to {dropdownOptions[source].text}");
    }

    public void SetCameraFocalLength(string length)
    {
        camera.focalLength = float.Parse(length);
    }

    public void SetSceneVSyncCount(string count)
    {
        Debug.Log("set vsyng to "+ count);
        QualitySettings.vSyncCount = int.Parse(count);
    }
}
