using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Core
{
    public static SystemCore system;
    public static SingleWebCam camController;
    public static Color32[] frame;
    public static WebCamTexture webCamTexture;
    public static Vector2Int inputResolution;
    public static DMXbrain dmxBrain;
}

public enum ColorEffect
{
    Colorize,
    ColorSpread,
    None
}

public class SystemCore : MonoBehaviour
{
    [Title("DMX Settings")]
    public float colorFadeSpeed = 1f;
    public ColourToDmxChannel singleChannel;

    [Title("Colour Settings")]
    public float brightness = 1f;    
    public ColorEffect colorEffect = ColorEffect.None;
    [Title("Colorize")]
    public float colorizeFactor = 0.5f;
    [Title("Colour Spreading")]
    public float spreadThreshold = 0.1f;
    public float minimumSpread = 0.25f;

    [Title("System Settings")]
    public bool lockFrameRate = false;

    [ShowIf("lockFrameRate")]
    public int systemFrameRate = 30;

    [Title("Camera Settings")]
    public Vector2Int resolution = new Vector2Int(100, 100);

    [Title("Grabber Settings")]
    public Vector2Int outputResolution = new Vector2Int(10, 10);
    public float frameRate = 1f;
    public float currentFrame = 0f;
    public int pixelSkip = 1;

    public bool useActiveSelection = false;
    public bool hideInactiveSelection = false;
    public Vector2Int minBounds;
    public Vector2Int maxBounds;


    [Title("Modules")]
    public List<ColourGrabber> colourGrabbers;
    public SingleWebCam camController;

    [Title("System")]
    public bool updateScale = false;
    public Transform colorGrabberPrefab;
    public Transform colorGrabberRoot;
    public Vector2Int inputResolution;
    public float minimumScaleResolution = 10f;
    public float outputScaleMultiplyer = 0.25f;
    public BoundingBox2D colorGrabberGrabBox;

    public int frames = 0;

    private void Awake()
    {
        Core.system = this;
        Core.dmxBrain = FindObjectOfType(typeof(DMXbrain)) as DMXbrain;

        SystemSetup();

        


        //  INITIALIZE MODULES
        camController.Init();


        BuildColorGrabbers();
    }

    void SystemSetup()
    {
        if (lockFrameRate)
        {
            Application.targetFrameRate = systemFrameRate;
        };
    }


    void BuildColorGrabbers()
    {
        colorGrabberGrabBox = new BoundingBox2D {
            x = 0,
            y = 0,
            width = inputResolution.x / outputResolution.x,
            height = inputResolution.y / outputResolution.y,
        };

        Vector2 placementPosition = Vector2.zero;
        Vector2Int matrixId = Vector2Int.zero;

        for (int y = 0; y < (inputResolution.y - colorGrabberGrabBox.height); y += colorGrabberGrabBox.height)
        {
            for (int x = 0; x < (inputResolution.x - colorGrabberGrabBox.width); x += colorGrabberGrabBox.width)
            {
                GameObject newColorGrabber = (GameObject)Instantiate(colorGrabberPrefab.gameObject, Vector3.zero, Quaternion.identity);

                newColorGrabber.transform.parent = colorGrabberRoot;
                newColorGrabber.transform.localPosition = placementPosition;
                newColorGrabber.transform.localRotation = Quaternion.identity;

                ColourGrabber colourGrabber = newColorGrabber.GetComponent<ColourGrabber>();

                colorGrabberGrabBox.x = x;
                colorGrabberGrabBox.y = y;

                BoundingBox2D newGrabBox = new BoundingBox2D
                {
                    x = colorGrabberGrabBox.x,
                    y = colorGrabberGrabBox.y,
                    width = colorGrabberGrabBox.width,
                    height = colorGrabberGrabBox.height,
                    matrixId = new Vector2Int(matrixId.x, matrixId.y)
                };


                colourGrabber.grabBox = newGrabBox;
                colourGrabber.pixelSkip = pixelSkip;


                colourGrabbers.Add(colourGrabber);

                colourGrabber.Init();


                Debug.LogWarning("ALERT!!! >> THIS IS ONLY SET TO A SINGLE 3 CHANNEL OUTPUT");
                singleChannel.colourGrabber = colourGrabber;



                placementPosition.x--;

                matrixId.x++;
            }

            matrixId.y++;
            matrixId.x = 0;

            placementPosition.y--;
            placementPosition.x = 0;
        }




        

        float newResolution = (minimumScaleResolution / outputResolution.x);

        colorGrabberRoot.localScale = new Vector3(newResolution, newResolution, newResolution);
    }

    public void LiveUpdateScale()
    {
        if (updateScale)
        {
            float newResolution = (minimumScaleResolution / outputResolution.x);

            colorGrabberRoot.localScale = new Vector3(newResolution, newResolution, newResolution);
        };
        
    }

    public void UpdateInputResolution()
    {
        Core.inputResolution = new Vector2Int(Core.webCamTexture.width, Core.webCamTexture.height);
        inputResolution = Core.inputResolution;
    }

    void Update()
    {
        LiveUpdateScale();


        if (currentFrame >= (1 / frameRate))
        {
            FrameUpdate();
            currentFrame = 0f;
            return;
        };


        currentFrame += Time.deltaTime;
    }

    void HideInactiveSelection()
    {

        if (!hideInactiveSelection)
        {
            return;
        };
        colourGrabbers.ForEach(colourGrabber => {
            if (
            colourGrabber.grabBox.matrixId.x >= minBounds.x && colourGrabber.grabBox.matrixId.x <= maxBounds.x &&
            colourGrabber.grabBox.matrixId.y >= minBounds.y && colourGrabber.grabBox.matrixId.y <= maxBounds.y
            )
            {
                colourGrabber.gameObject.SetActive(true);
            }
            else
            {
                colourGrabber.gameObject.SetActive(false);
            };
        });
    }

    void FrameUpdate()
    {
        if (useActiveSelection)
        {
            colourGrabbers.ForEach(colourGrabber => {
                if (
                colourGrabber.grabBox.matrixId.x >= minBounds.x && colourGrabber.grabBox.matrixId.x <= maxBounds.x &&
                colourGrabber.grabBox.matrixId.y >= minBounds.y && colourGrabber.grabBox.matrixId.y <= maxBounds.y
                )
                {
                    colourGrabber.FrameUpdate();
                };
            });

            HideInactiveSelection();
        }
        else {
            colourGrabbers.ForEach(colourGrabber => {
                colourGrabber.FrameUpdate();
            });
        };

        


        frames++;
    }
}
