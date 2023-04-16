using UnityEngine;
using System.Collections;
using ETC.Platforms;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;

public class DMXbrain : MonoBehaviour {

	private DMX dmx;
	public Transform ComPortButtonPrefab;
	public Transform DmxUiPanel;
	public bool dmxActive = false;
	public Text dmxSendData;
	int noDMXsendData = 0;

	public List<int> DMXChannels;
	public int noDMXChannels = 50;

	public int frameLimiter = 2;
	public int frameCount = 1;

	public Slider masterFaderSlider;
	public float masterFaderValue = 1.0f;

	// Use this for initialization
	void Start () {
		int buttonPosition = -30;
		DMXChannels = new List<int>(noDMXChannels);

		foreach (string str in SerialPort.GetPortNames())
		{

			print (str);
			Transform thisObject = Instantiate(ComPortButtonPrefab);
			thisObject.SetParent(DmxUiPanel);

			//	BUTTON TEXT
			Text thisText = thisObject.GetComponentInChildren<Text>(); 
			thisText.text = str;

			//	BUTTON POSITION
			RectTransform thisPos = thisObject.GetComponent<RectTransform>();
			thisPos.localPosition = new Vector2 (0,buttonPosition);
			buttonPosition +=30;

			//	BUTTON ACTION
			Button thisButton = thisObject.GetComponent<Button>();
			string thisPort = str;
			thisButton.onClick.AddListener(() => connectDMXtoCOM(thisPort));

		}

/*

*/

		//DMXChannels = gameObject.GetComponentsInChildren<DMXchannelAction> ();
		//noDMXChannels = DMXChannels.Length;
	}
	
	// Update is called once per frame
	void Update () {
		if (frameCount < frameLimiter) {
			frameCount++;
			if (dmxActive) {
				this.dmx.Send ();
			};
		} else {
			frameCount = 0;
		};

	}
	
	public void updateMasterFaderValue () {
		masterFaderValue = (masterFaderSlider.value/255);
	}


	public void sendData (int channel, float value) {
			if (dmxActive) {
				this.dmx.Channels [(channel)] = (byte)(Mathf.RoundToInt ((value * 255) * masterFaderValue));
			};
	}

	public void connectDMXtoCOM (string COMPORT) {
		if (!dmxActive) {
			this.dmx = new DMX (COMPORT);
			dmxActive = true;
		} else {
			this.dmx.changePortAddress(COMPORT);
		};
		StopAllCoroutines ();
		StartCoroutine ( DMXtestFlash() );

		Debug.Log("SENDING > " + string.Format(COMPORT));

	}

	IEnumerator DMXtestFlash () {

		this.dmx.Channels[1] = 255;
		this.dmx.Channels[2] = 255;
		this.dmx.Channels[3] = 255;
		this.dmx.Send();

		yield return new WaitForSeconds (0.5f);

		this.dmx.Channels[1] = 0;
		this.dmx.Channels[2] = 0;
		this.dmx.Channels[3] = 0;
		this.dmx.Send();
	}

	public void blackoutAll () {
		for(int i = 1; i < 256; i++) {
			this.dmx.Channels[i] = 0;
		};
		this.dmx.Send();
	}

	public void flashAll () {
		for(int i = 1; i < 256; i++) {
			this.dmx.Channels[i] = 50;
		};
		this.dmx.Send();
	}
}
