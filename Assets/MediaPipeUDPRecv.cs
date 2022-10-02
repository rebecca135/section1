/*
 
    -----------------------
    UDP-Receiver
    -----------------------
   
    // > receive
    // 127.0.0.1 : 1234
 
*/
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

public class MediaPipeUDPRecv : MonoBehaviour
{

    // receiving Thread
    Thread receiveThread;
    Mutex textLock = new Mutex();

    // udpclient object
    UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port;

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = "";

    //public Camera cam;
    private float x; 
    private float y;
    private Vector2 pos;

    private float globalScaleMultiplier = 1f;
    private float scaleMin = 0.25f;
    private float scaleMax = 0.6f;
    private float time = 0f;
    public float timeDelay;
    private float temp;
    private bool toggle = false;

    private static void Main()
    {
        MediaPipeUDPRecv receiveObj = new MediaPipeUDPRecv();
        receiveObj.init();
    }

    public void Start() { init(); }

    void randomizePosition()
    {
        x = UnityEngine.Random.Range(-8, 8);
        y = UnityEngine.Random.Range(-7, 7);
        pos = new Vector2(x, y);
        transform.position = pos;
    }

    void randomizeScale()
    {
        Vector3 randomizedScale = Vector3.one;
        float newScale = UnityEngine.Random.Range(scaleMin, scaleMax);
        randomizedScale = new Vector3(newScale, newScale, newScale);
        transform.localScale = randomizedScale * globalScaleMultiplier;
    }

    void randomizeColor() {
        Color myNewColor = new Color(
            (float)UnityEngine.Random.Range(0f, 1f),
            (float)UnityEngine.Random.Range(0f, 1f),
            (float)UnityEngine.Random.Range(0f, 1f)
        );

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        GetComponent<SpriteRenderer>().material.color = myNewColor;
        //s.color = myNewColor;
    }

    /*void randomizeTime() {
        temp = Random.Range(0, 1);
    }*/

    private void Update()
    {
        string latestData = getLatestUDPPacket();

        if (latestData.Length == 0)
        {
           // cam.backgroundColor = Color.yellow;
            return;
        }
        double curr = Convert.ToDouble(latestData);

        time = time + 1f * Time.deltaTime;
        if (time >= (timeDelay - curr)) {
            time = 0f;
            if (toggle) {
                toggle = false;
                GetComponent<Renderer>().enabled = toggle;
            } else if (toggle == false) {
                toggle = true;
                GetComponent<Renderer>().enabled = toggle;
            }
            //randomizeTime();
            randomizePosition();
            randomizeScale();
            randomizeColor();
        }

        /*if (curr < 0)
        {
            cam.backgroundColor = Color.blue;
        }
        else if (curr < .1)
        {
            cam.backgroundColor = Color.red;
        }
        else if (curr < .2)
        {
            cam.backgroundColor = Color.grey;
        }
        else
        {
            cam.backgroundColor = Color.black;
        }*/
    }

    // init
    private void init()
    {

        // define port
        port = 1234;

        // Camden Code:
        //cam = GetComponent<Camera>();
        //cam.clearFlags = CameraClearFlags.SolidColor;


        // ----------------------------
        // Abh�ren
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread f�r den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes received.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Encode bytes to text format using UTF8 encoding.
                string text = Encoding.UTF8.GetString(data);

                // Display the retrieved text.
                print(">> " + text);

                // begin thread lock
                textLock.WaitOne();

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                allReceivedUDPPackets = allReceivedUDPPackets + text;

                // release thread lock
                textLock.ReleaseMutex();

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // Return the latest UDP data, and empty the queue
    public string getLatestUDPPacket()
    {
        string data;
        textLock.WaitOne();
        data = String.Copy(lastReceivedUDPPacket);
        allReceivedUDPPackets = "";
        textLock.ReleaseMutex();
        return data;
    }
}