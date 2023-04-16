using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingBoard : MonoBehaviour
{

    private void Start()
    {

    }


    public void UpdateFader(int channelNo, float value)
	{
        Core.dmxBrain.sendData(channelNo, value);
        //Debug.Log(channelNo + ":" + value);
    }
}
