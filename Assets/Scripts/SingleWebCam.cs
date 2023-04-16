using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;




public class SingleWebCam : MonoBehaviour
{
    [FoldoutGroup("Frame Capture")]
    public int framesCaptures = 0;
    [FoldoutGroup("Frame Capture")] 
    public Color32[] data;


    [Title("Capture Settings")]
    public float frameRate = 1f;
    public float currentFrame = 0f;

    

    


    [Title("System")]
    public Renderer webCameraRenderer;


    private void OnValidate()
    {
        ConnectRenderer();
    }


    public void Init()
    {
        Core.camController = this;
        Core.frame = data;
        SetupCamera();
        Core.system.UpdateInputResolution();
    }





    

    void SetupCamera()
    {
        ConnectRenderer();

        //Initialize the webCamTexture  
        Core.webCamTexture = new WebCamTexture(Core.system.resolution.x, Core.system.resolution.y);
        //Assign the images captured by the first available webcam as the texture of the containing game object  
        webCameraRenderer.material.mainTexture = Core.webCamTexture;
        //Start streaming the images captured by the webcam into the texture  
        Core.webCamTexture.Play();

        //data = new Color32[Core.webCamTexture.width * Core.webCamTexture.height];
    }



    void ConnectRenderer()
    {
        if (!webCameraRenderer)
        {
            webCameraRenderer = GetComponent<Renderer>();
        };

        if (!webCameraRenderer)
        {
            Debug.LogError("Single Web Cam GameObject Needs a renderer to display the webcam output");
        };
    }



    void Update()
    {

        if( currentFrame >= (1/frameRate) )
        {
            FrameUpdate();
            currentFrame = 0f;
            return;
        };


        currentFrame += Time.deltaTime;
    }


    void FrameUpdate()
    {
        //Core.webCamTexture.GetPixels32(data);
        framesCaptures++;
    }
}
