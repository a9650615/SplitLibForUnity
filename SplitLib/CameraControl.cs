using System;
using UnityEngine;
using UnityEngine.UI;

namespace SplitLib
{

    internal class CameraControl
    {
        GameObject outputCanvas;
        Camera cameraLeft;
        Camera cameraRight;
        Camera outputCamera;

        private Texture2D lastScreen;
        private RenderTexture cameraLeft_Texture;
        private RenderTexture cameraRight_Texture;
        internal Texture2D outputScreen;
        private int viewWidth;
        private int viewHeight;
        RawImage outputImage;

        public CameraControl(GameObject targetObj)
        {
            viewWidth = Screen.width;
            viewHeight = Screen.height;

            outputScreen = new Texture2D(viewWidth * 2, viewHeight);
            lastScreen = new Texture2D(viewWidth, viewHeight);

            cameraLeft_Texture = new RenderTexture(Screen.width, Screen.height, 24);
            cameraRight_Texture = new RenderTexture(Screen.width, Screen.height, 24);
            cameraLeft = targetObj.AddComponent<Camera>();
            cameraLeft.cullingMask &= ~(1 << 5); // except UI layer

            GameObject rightObject = new GameObject();
            //rightObject.hideFlags = HideFlags.HideInHierarchy;
            cameraRight = rightObject.AddComponent<Camera>();
            cameraRight.name = "right";
            cameraRight.transform.SetParent(cameraLeft.transform);
            cameraRight.transform.localPosition = new Vector3(0.2f, 0, 0);
            cameraRight.cullingMask &= ~(1 << 5); // except UI layer
            cameraLeft.targetTexture = cameraLeft_Texture;
            cameraRight.targetTexture = cameraRight_Texture;

            /** Setup canvas for output **/
            GameObject outputCameraObject = new GameObject();
            outputCamera = outputCameraObject.AddComponent<Camera>();
            outputCamera.clearFlags = CameraClearFlags.Nothing;

            outputCanvas = new GameObject();
            outputCanvas.AddComponent<Canvas>();
            outputCanvas.name = "outputCanvas";
            outputCanvas.transform.position = new Vector3(99, 99, 99);
            outputCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            outputCanvas.GetComponent<Canvas>().worldCamera = outputCamera;
            outputCanvas.GetComponent<Canvas>().planeDistance = 2;
            //outputCanvas.hideFlags = HideFlags.HideInHierarchy;
            outputCanvas.layer = 5;

            outputImage = new GameObject().AddComponent<RawImage>();
            outputImage.transform.SetParent(outputCanvas.transform);
            outputImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            outputImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.localScale = new Vector3(1, 1, 1);
            //outputImage.rectTransform.offsetMin = new Vector2(0, 0);
            //outputImage.rectTransform.offsetMax = new Vector2(0, 0);
            outputImage.texture = lastScreen;
            //outputImage.uvRect = new Rect(0, 0, 1, 1);
            outputImage.SetNativeSize();
            outputImage.GetComponent<RectTransform>().sizeDelta = new Vector2(viewWidth, viewHeight);
        }

        public Color[] UpdateCameraViewToTexture()
        {
            RenderTexture.active = cameraLeft_Texture;
            outputScreen.ReadPixels(new Rect(0, 0, viewWidth, viewHeight), 0, 0);
            RenderTexture.active = cameraRight_Texture;
            outputScreen.ReadPixels(new Rect(0, 0, viewWidth, viewHeight), viewWidth, 0);
            outputScreen.Apply();

            return outputScreen.GetPixels(0, 0, viewWidth * 2, viewHeight, 0);
        }

        public void UpdateLastScreen(Color[] splitData)
        {
            //Graphics.CopyTexture(outputScreen, 0, 0, 0, 0, viewWidth, viewHeight, lastScreen, 0, 0, 0, 0);
            lastScreen.SetPixels(splitData);
            lastScreen.Apply();
        }
    }

}
