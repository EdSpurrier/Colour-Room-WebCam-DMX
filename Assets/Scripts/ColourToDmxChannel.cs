using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ColourToDmxChannel : MonoBehaviour
{
    [Title("System")]
    public float fadeTime = 1f;

    [Title("Current Color")]
    public Color outputColor;
    public Color targetColor;
    public Color sendColor;

    [Title("DMX Channels")]
    public int redChannel = 0;
    public int greenChannel = 0;
    public int blueChannel = 0;

    public float red = 0;
    public float green = 0;
    public float blue = 0;

    public ColourGrabber colourGrabber;



    private void Update()
    {
        if (!colourGrabber)
        {
            return;
        };

        Color newTarget = colourGrabber.colorGrab;


        if (targetColor != newTarget)
        {
            outputColor = sendColor;

            targetColor = newTarget;

            fadeTime = 0;
        };


        if (fadeTime < 0f)
        {
            return;
        };



        fadeTime += Time.deltaTime * Core.system.colorFadeSpeed;

        if (fadeTime > 1f)
        {
            fadeTime = 1f;
        };

        sendColor = Color.Lerp(outputColor, targetColor, (fadeTime));

        red = Mathf.Clamp(sendColor.r, 0f, 1f);
        green = Mathf.Clamp(sendColor.g, 0f, 1f);
        blue = Mathf.Clamp(sendColor.b, 0f, 1f);

        Core.dmxBrain.sendData(redChannel, red);
        Core.dmxBrain.sendData(greenChannel, green);
        Core.dmxBrain.sendData(blueChannel, blue);
    }



}
