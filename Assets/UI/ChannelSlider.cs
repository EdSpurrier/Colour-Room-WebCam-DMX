using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChannelSlider : MonoBehaviour
{
    public int channel;
    public Slider slider;
    public MixingBoard mixer;
    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    public void UpdateSlider()
    {
        mixer.UpdateFader(channel, slider.value);
    }

}
