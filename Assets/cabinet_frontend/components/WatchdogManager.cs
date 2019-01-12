using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class WatchdogManager : MonoBehaviour {
    
    static long _previous_watchdog_checkin = 0;
    public static long GetWatchdogDuration() {
        return 110000000; // 11 seconds
    }
    public static long GetWatchdogTimer() {
        return System.DateTime.Now.Ticks - _previous_watchdog_checkin;
    }
    public static void WatchdogCheckin() {
        _previous_watchdog_checkin = System.DateTime.Now.Ticks;
        Debug.Log("Checkin!");
    }

    // Use this for initialization
    void Start () {
        //Thread t = new Thread (StartListening);
        //t.Start ();
    }

    void acceptCallback( IAsyncResult ar) {
        // Add the callback code here.
        //StateObject state = (StateObject) ar.AsyncState;
        //Socket handler = ar.

        Socket handler = listener.EndAccept (ar);
        //handler.beginR
        // Read data from the client socket. 
        int bytesRead = handler.EndReceive(ar);
        WatchdogCheckin ();
    }


    Socket listener = null;
    public void StartListening () {
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 11000);

        listener = new Socket( localEP.Address.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp );
        
        try { 
            listener.Bind(localEP);
            listener.Listen(10);

            while (true) {

                listener.BeginAccept(
                    new AsyncCallback(acceptCallback), 
                    listener );


            }
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }

        Debug.Log( "Closing the listener...");
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    void _CleanupNetworkComms () {
        /*if (listener != null && listener.Connected) {
            listener.Close ();
        }*/
    }

    void OnApplicationQuit() {
        Debug.Log ("closing socket...");
        _CleanupNetworkComms ();
    }
}
