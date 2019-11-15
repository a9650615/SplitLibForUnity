using System;
using UnityEditor;
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

            cameraLeft_Texture = new RenderTexture(viewWidth, viewHeight, 24);
            cameraRight_Texture = new RenderTexture(viewWidth, viewHeight, 24);
            if (targetObj.GetComponent<Camera>())
            {
                cameraLeft = targetObj.GetComponent<Camera>();
            }
            else
            {
                cameraLeft = targetObj.AddComponent<Camera>();
            }
            cameraLeft.cullingMask &= ~(1 << 5); // except UI layer

            GameObject rightObject = new GameObject();
            rightObject.hideFlags = HideFlags.HideInHierarchy;
            cameraRight = rightObject.AddComponent<Camera>();
            cameraRight.name = "right";
            cameraRight.transform.SetParent(cameraLeft.transform);
            cameraRight.transform.localPosition = new Vector3(0.2f, 0, 0);
            cameraRight.cullingMask &= ~(1 << 5); // except UI layer
            cameraLeft.targetTexture = cameraLeft_Texture;
            cameraRight.targetTexture = cameraRight_Texture;

            /** Setup canvas for output **/
            GameObject outputCameraObject = new GameObject();
            outputCameraObject.hideFlags = HideFlags.HideInHierarchy;
            outputCamera = outputCameraObject.AddComponent<Camera>();
            outputCamera.clearFlags = CameraClearFlags.Nothing;
            outputCamera.name = "outputCamera";
            outputCamera.depth = 999;
            outputCamera.cullingMask = (1 << 5); // Only UI layer

            outputCanvas = new GameObject();
            outputCanvas.AddComponent<Canvas>();
            outputCanvas.name = "outputCanvas";
            outputCanvas.transform.localPosition = new Vector3(99, 99, 99);
            outputCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            outputCanvas.GetComponent<Canvas>().worldCamera = outputCamera;
            outputCanvas.GetComponent<Canvas>().planeDistance = 2;
            outputCanvas.layer = 5;
            SceneVisibilityManager.instance.Hide(outputCanvas, true);

            GameObject imageObj = new GameObject();
            //imageObj.hideFlags = HideFlags.HideAndDontSave;

            outputImage = imageObj.AddComponent<RawImage>();
            outputImage.transform.SetParent(outputCanvas.transform);
            SceneVisibilityManager.instance.Hide(imageObj, false);
            outputImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            outputImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.localScale = new Vector3(1, 1, 1);
            outputImage.texture = lastScreen;
            //outputImage.uvRect = new Rect(0, 0, 1, 1);
            outputImage.SetNativeSize();
            outputImage.GetComponent<RectTransform>().sizeDelta = new Vector2(viewWidth, viewHeight);
            outputCanvas.hideFlags = HideFlags.HideInHierarchy;
        }

        private void UpdateEnv()
        {
            if (viewWidth != Screen.width || viewHeight != Screen.height)
            {
                viewWidth = Screen.width;
                viewHeight = Screen.height;
                outputScreen.Resize(viewWidth * 2, viewHeight);
                lastScreen.Resize(viewWidth, viewHeight);
                cameraLeft_Texture = new RenderTexture(viewWidth, viewHeight, 24);
                cameraRight_Texture = new RenderTexture(viewWidth, viewHeight, 24);
                cameraLeft.targetTexture = cameraLeft_Texture;
                cameraRight.targetTexture = cameraRight_Texture;
                outputImage.SetNativeSize();
                outputImage.GetComponent<RectTransform>().sizeDelta = new Vector2(viewWidth, viewHeight);
            }
        }

        public Color[] UpdateCameraViewToTexture()
        {
            UpdateEnv();
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
