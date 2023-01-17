// Recieves data from udp_emmiter.py and parses it to pass along TrackedObject.cs instances.

using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class UdpTrackingReceiver : MonoBehaviour {
    Thread receiveThread;
    UdpClient client;
    public int port = 8051;
    Dictionary<string, float[]> trackingPoints;
    DateTime lastMessageReceivedTime;
    int trackingPointsCount;
    public bool IsServerConnected{get; private set;}
    public float MeasuredTickTime{get; private set;}
    public int MeasuredFrequency{get; private set;}

    void Start()
    {
        IsServerConnected = false;
        trackingPoints = new Dictionary<string, float[]>();
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // Check if last message arrived in less than a second, otherwise consider server is disconnected
        TimeSpan difference = DateTime.UtcNow - lastMessageReceivedTime;
        IsServerConnected = (difference.TotalMilliseconds < 1000f);
        
        /*
        string names = "";
        foreach(KeyValuePair<string, float[]> entry in trackingPoints)
        {
            names += "\n" + entry.Key;
            foreach(float val in entry.Value){
                names += val + " ";
            }
        }
        UnityEngine.Debug.Log(names);
        */
        

        // UnityEngine.Debug.Log($"Server connected: {IsServerConnected}, Measured Tick Time: {MeasuredTickTime.ToString("0.00")}ms.");
    }
	
    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        client.Close();
    }

    public string[] GetTrackingDevicesNames()
    {
        return trackingPoints.Keys.ToArray();
    }

    public float[] GetTransformData(string key)
    {
        if (key is not null && trackingPoints.ContainsKey(key))
        {
            return trackingPoints[key];
        } else {
            return new float[7];
        }
    }

    private void ReceiveData()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        TimeSpan lastMeasuredTime = stopwatch.Elapsed;
        float measuredTime;

        client = new UdpClient(port);
        UnityEngine.Debug.Log("Starting Listener");
        while (true)
        {
            try
            {
                IPEndPoint _anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = client.Receive(ref _anyIP);

                measuredTime = (float)(stopwatch.Elapsed - lastMeasuredTime).TotalMilliseconds;
                
                if (measuredTime > 1d){
                    MeasuredTickTime = measuredTime;
                    MeasuredFrequency = (int)(1000f/measuredTime);
                }
                    
                lastMeasuredTime = stopwatch.Elapsed;

                lastMessageReceivedTime = DateTime.UtcNow;

                // Parse data
                StringBuilder _sb = new StringBuilder();
                int _state = 0;
                string _deviceName = "";

                for (int i = 0; i < _data.Length; i++)
                {
                    switch(_state){
                        case 0: // Get count of tracked points
                            trackingPointsCount = BitConverter.ToInt32(_data, i);
                            i += 3;
                            _state++;
                        break;
                        case 1: // Finding the device string
                            char c = (char)_data[i];
                            if (c == ':')
                            {
                                _deviceName = _sb.ToString();
                                _sb.Clear();
                                _state++;
                                break;
                            }
                            _sb.Append(c);
                        break;
                        case 2: // Getting the XYZ position values and WXYZ rotation quaternion
                            float[] _floats = new float[7];
                            for (int j = 0; j < 7; j++)
                            {
                                _floats[j] = BitConverter.ToSingle(_data, i);
                                i += 4;
                            }
                            i -= 1;
                            trackingPoints[_deviceName] = _floats;
                            _state = 1;
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                UnityEngine.Debug.Log(err.ToString());
            }
        }
    }
}
