using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SendCOM : MonoBehaviour {
    
    //public static SerialPort sp = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
	public string COM_port;
	public static SerialPort sp;
	public string message2;
	float timePassed = 0.0f;
	// Use this for initialization
	void Start () {
		sp = new SerialPort(COM_port, 9600);
		OpenConnection();

        foreach (string str in SerialPort.GetPortNames())
        {
            Debug.Log(string.Format(str));
        }

    }
	
	// Update is called once per frame
	void Update () {
		//timePassed+=Time.deltaTime;
		//if(timePassed>=0.2f){
		//sp.Write("r");

			//print("BytesToRead" +sp.BytesToRead);
			
		//	timePassed = 0.0f;
		//}

	}

	public void OpenConnection() 
    {
       if (sp != null) 
       {
         if (sp.IsOpen) 
         {
          sp.Close();
          print("Closing port, because it was already open!");
         }
         else 
         {
          sp.Open();  // opens the connection
          sp.ReadTimeout = 16;  // sets the timeout value before reporting error
          print("Port Opened!");
		//		message = "Port Opened!";
         }
       }
       else 
       {
         if (sp.IsOpen)
         {
          print("Port is already open");
         }
         else 
         {
          print("Port == null");
         }
       }
    }

    void OnApplicationQuit() 
    {
       sp.Close();
    }

    public static void sendYellow(){
    	sp.Write("y");
    }

    public static void sendGreen(){
    	sp.Write("g");
    	//sp.Write("\n");
    }

	public static void sendBeat(){
		//debugger.debuggerTextUpdate ("sending Beat");
		sp.Write("B");
		//sp.Write("X");
	}

    public static void sendRed(){
    	sp.Write("r");
    }

	public static void readSerial () {

		//debugger.debuggerTextUpdate (sp.ReadLine());
	}
}
