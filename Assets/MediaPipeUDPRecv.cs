/*
 
    -----------------------
    UDP-Receiver
    -----------------------
   
   Receiving Media Pipe Data from Pose program at port 10001 on local host.
 
*/
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class MediaPipeUDPRecv : MonoBehaviour
{

    // receiving Thread
    private Thread receiveThread;
    private Mutex textLock = new Mutex();

    // udpclient object
    private UdpClient client;

    // private media pipe receiving port
    private int mediaPort;

    // Receiver Data
    public double handHeight;

    // Intrusion Flag Variables
    public GameObject banner;
    private bool intrusionFlag = false;

    private static void Main()
    {
        MediaPipeUDPRecv receiveObj = new MediaPipeUDPRecv();
        receiveObj.init();
    }

    public void Start() { 
        init();
        banner.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
        checkForIntrusion();
    }

    // init
    private void init()
    {

        // define mediaPort
        mediaPort = 10001;


        // ----------------------------
        // Receive Thread
        // ----------------------------
        // Define local endpoint (where messages will be received).
        // Create a new thread to receive incoming messages.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(mediaPort);
        while (true)
        {

            try
            {
                // Bytes received.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Encode bytes to text format using UTF8 encoding.
                string text = Encoding.UTF8.GetString(data);

                // Display the retrieved text
                JObject json = JObject.Parse(text);

                // begin thread lock
                textLock.WaitOne();

                handHeight = Convert.ToDouble(json["h3"]);

                // release thread lock
                textLock.ReleaseMutex();

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private void checkForIntrusion(){
        if (handHeight > 3) {
            intrusionFlag = true;
        } else {
            intrusionFlag = false;
        }
        banner.GetComponent<Renderer>().enabled = intrusionFlag;
    }
}