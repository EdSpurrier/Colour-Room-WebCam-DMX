using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoundingBox2D
{
    public int x = 1;
    public int y = 1;
    public int width = 10;
    public int height = 10;
    public Vector2Int matrixId;
}

public enum ColorPigmentRGB
{
    Red,
    Green,
    Blue
}


public enum ColorStrength
{
    High,
    Medium,
    Low
}

[System.Serializable]
public class ColorValue
{
    public float value;
    public ColorStrength strength;
}

[System.Serializable]
public class ColorSpread
{
    public ColorValue red;
    public ColorValue green;
    public ColorValue blue;
}

[System.Serializable]
public class ColorizeColor
{
    public float value;
    public ColorPigmentRGB pigment;
}


public class ColourGrabber : MonoBehaviour
{
    public bool outputToMaterial = true;
    public Renderer outputRenderer;


    public float red = 0;
    public float green = 0;
    public float blue = 0;


    public ColorSpread colorSpread;

    public List<ColorizeColor> colorizeColors;

    public Color colorGrab;

    public int pixelSkip = 1;
    
    public BoundingBox2D grabBox;

    public Color[] pixelGrab;

    public float pixelCount = 0;

    public bool active = true;


    public int currentPixel = 0;
    public void Init()
    {
        pixelCount = (float)((grabBox.width) * (grabBox.height));
        
        colorizeColors = new List<ColorizeColor>(3);
        colorizeColors.Add( new ColorizeColor { 
            pigment = ColorPigmentRGB.Red,
            value = 0f
        });
        colorizeColors.Add(new ColorizeColor
        {
            pigment = ColorPigmentRGB.Green,
            value = 0f
        });
        colorizeColors.Add(new ColorizeColor
        {
            pigment = ColorPigmentRGB.Blue,
            value = 0f
        });
    }

    public void GrabPixelsFromTexture ()
    {
        pixelGrab = Core.webCamTexture.GetPixels(grabBox.x, grabBox.y, grabBox.width, grabBox.height);
    }

    public void AverageColor()
    {
        float correctedPixelCount = pixelCount / pixelSkip;

        foreach (Color pixel in pixelGrab)
        {
            if (pixelSkip == currentPixel)
            {
                red += pixel.r;
                green += pixel.g;
                blue += pixel.b;
                currentPixel = 0;
            };

            currentPixel++;
        };

        red = red / correctedPixelCount;
        green = green / correctedPixelCount;
        blue = blue / correctedPixelCount;

        

        
    }

    public void FrameUpdate()
    {
        if (!active)
        {
            return;
        };

        GrabPixelsFromTexture();
        AverageColor();


        ColorEffects();



        SetColor();
        OutputToRenderer();
    }


    
   

    bool InThreshold(float value1, float value2)
    {
        if (Mathf.Abs(value1 - value2) > Core.system.spreadThreshold)
        {
            return false;
        };

        return true;
    }


    void ColorEffects ()
    {
        if (Core.system.colorEffect == ColorEffect.Colorize)
        {
            Colorize();

            colorizeColors.ForEach(colorizeColor =>
            {
                if (colorizeColor.pigment == ColorPigmentRGB.Red)
                {
                    colorGrab.r = colorizeColor.value;
                }
                else if (colorizeColor.pigment == ColorPigmentRGB.Green)
                {
                    colorGrab.g = colorizeColor.value;
                }
                else if (colorizeColor.pigment == ColorPigmentRGB.Blue)
                {
                    colorGrab.b = colorizeColor.value;
                };
            });
        }
        else if (Core.system.colorEffect == ColorEffect.ColorSpread)
        {
            SpreadColor();
            colorGrab = new Color(colorSpread.red.value, colorSpread.green.value, colorSpread.blue.value);
        }
        else {
            colorGrab = new Color(red, green, blue);
        };

        colorGrab *= Core.system.brightness;
    }

    void Colorize()
    {



        colorizeColors.ForEach(colorizeColor => {
            if (colorizeColor.pigment == ColorPigmentRGB.Red)
            {
                colorizeColor.value = red;
            }
            else if (colorizeColor.pigment == ColorPigmentRGB.Green)
            {
                colorizeColor.value = green;
            }
            else if (colorizeColor.pigment == ColorPigmentRGB.Blue)
            {
                colorizeColor.value = blue;
            };
        });

        colorizeColors = colorizeColors.OrderByDescending(colorizeColor => colorizeColor.value).ToList();



        if (colorizeColors[0].value > Core.system.colorizeFactor)
        {
            colorizeColors[0].value += Core.system.colorizeFactor;
            colorizeColors[2].value -= Core.system.colorizeFactor;
        };

    }


    void SpreadColor()
    {
        colorSpread.red.value = red;
        colorSpread.green.value = green;
        colorSpread.blue.value = blue;




        colorSpread.red.strength = ColorStrength.High;



        //  GET RANGE FROM SMALLEST TO LARGEST
        //  EXPAND THE RANGE?

        if ( 
            InThreshold(colorSpread.red.value, colorSpread.green.value) &&
            InThreshold(colorSpread.red.value, colorSpread.blue.value)
            )
        {
            //  = white
            //  do nothing
            return;  
        };

        if (
            InThreshold(colorSpread.red.value, colorSpread.green.value) &&
            colorSpread.blue.value < colorSpread.red.value
            )
        {

            //  = orange

            if (Mathf.Abs(colorSpread.blue.value - colorSpread.red.value) < Core.system.minimumSpread)
            {
                colorSpread.blue.value = colorSpread.red.value - Core.system.minimumSpread;
            };

            
            return;
        };



        if (
            InThreshold(colorSpread.blue.value, colorSpread.green.value) &&
            colorSpread.red.value < colorSpread.blue.value
            )
        {

            //  = Aqua
            if (Mathf.Abs(colorSpread.blue.value - colorSpread.red.value) < Core.system.minimumSpread)
            {
                colorSpread.red.value = colorSpread.blue.value - Core.system.minimumSpread;
            };
            return;
        };


        if (
            InThreshold(colorSpread.blue.value, colorSpread.red.value) &&
            colorSpread.green.value < colorSpread.blue.value
            )
        {
            //  = Purple
            if (Mathf.Abs(colorSpread.green.value - colorSpread.blue.value) < Core.system.minimumSpread)
            {
                colorSpread.green.value = colorSpread.blue.value - Core.system.minimumSpread;
            };
            return;
        };




    }

    void SetColor()
    {
        
    }

    public void OutputToRenderer()
    {
        if (!outputToMaterial)
        {
            return;
        };

        outputRenderer.material.color = colorGrab;
    }
}
