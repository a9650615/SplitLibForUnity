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
        Camera inputCanvasCamera;

        private GameObject inputCanvasObj;
        private Texture2D lastScreen;
        private RenderTexture cameraLeft_Texture;
        private RenderTexture cameraRight_Texture;
        private RenderTexture canvas_Texture;
        internal Texture2D canvasTexture2d;
        internal Texture2D outputScreen;
        private int viewWidth;
        private int viewHeight;
        RawImage outputImage;

        public CameraControl()
        {
            viewWidth = Screen.width;
            viewHeight = Screen.height;

            outputScreen = new Texture2D(viewWidth * 2, viewHeight, TextureFormat.ARGB32, false);
            lastScreen = new Texture2D(viewWidth, viewHeight);
            canvasTexture2d = new Texture2D(viewWidth, viewHeight, TextureFormat.ARGB32, false);
            cameraLeft_Texture = new RenderTexture(viewWidth, viewHeight, 24);
            cameraRight_Texture = new RenderTexture(viewWidth, viewHeight, 24);
            canvas_Texture = new RenderTexture(viewWidth, viewHeight, 24, RenderTextureFormat.ARGB32);
            UpdateEnv();

        }

        public void BindCanvas(GameObject targetObj)
        {
            inputCanvasObj = targetObj;
            GameObject inputCanvasCameraObj = new GameObject();
            inputCanvasCameraObj.name = "inputCanvasCamera";
            inputCanvasCamera = inputCanvasCameraObj.AddComponent<Camera>();
            inputCanvasCamera.clearFlags = CameraClearFlags.Nothing;
            //inputCanvasCamera.backgroundColor = Color.clear;
            //inputCanvasCamera.clearFlags = CameraClearFlags.Nothing;
            inputCanvasCamera.targetTexture = canvas_Texture;
            inputCanvasCamera.cullingMask = (1 << 5);

            inputCanvasObj.layer = 5;
            Canvas inputCanvas = inputCanvasObj.GetComponent<Canvas>();
            inputCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            inputCanvas.worldCamera = inputCanvasCamera;

            HideGameObject(inputCanvasCameraObj); //Hide camera
        }

        private void HideGameObject(GameObject obj, bool hideVisibility = false)
        {
            obj.hideFlags = HideFlags.HideInHierarchy;
            if (hideVisibility)
            {
                if (Type.GetType("SceneVisibilityManager") != null)
                {
                    SceneVisibilityManager.instance.Hide(obj, true);
                }
            }
        }

        public void BindCamera(GameObject targetObj)
        {
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
            outputCamera.cullingMask = (1 << 8); // Only UI layer

            outputCanvas = new GameObject();
            outputCanvas.AddComponent<Canvas>();
            outputCanvas.name = "outputCanvas";
            outputCanvas.transform.localPosition = new Vector3(99, 99, 99);
            outputCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            outputCanvas.GetComponent<Canvas>().worldCamera = outputCamera;
            outputCanvas.GetComponent<Canvas>().planeDistance = 2;
            outputCanvas.layer = 8;
            HideGameObject(outputCanvas);

            GameObject imageObj = new GameObject();
            //imageObj.hideFlags = HideFlags.HideAndDontSave;

            outputImage = imageObj.AddComponent<RawImage>();
            outputImage.transform.SetParent(outputCanvas.transform);
            //SceneVisibilityManager.instance.Hide(imageObj, false);
            outputImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            outputImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            outputImage.rectTransform.localScale = new Vector3(1, 1, 1);
            outputImage.texture = lastScreen;
            //outputImage.uvRect = new Rect(0, 0, 1, 1);
            outputImage.SetNativeSize();
            outputImage.GetComponent<RectTransform>().sizeDelta = new Vector2(viewWidth, viewHeight);
            HideGameObject(imageObj);
        }

        private void UpdateEnv()
        {
            if (viewWidth == 0 || viewHeight == 0 || viewWidth != Screen.width || viewHeight != Screen.height)
            {
                viewWidth = Screen.width;
                viewHeight = Screen.height;
                outputScreen.Resize(viewWidth * 2, viewHeight);
                lastScreen.Resize(viewWidth, viewHeight);
                cameraLeft_Texture = new RenderTexture(viewWidth, viewHeight, 24);
                cameraRight_Texture = new RenderTexture(viewWidth, viewHeight, 24);
                canvasTexture2d = new Texture2D(viewWidth, viewHeight);
                canvas_Texture = new RenderTexture(viewWidth, viewHeight, 24, RenderTextureFormat.ARGB32);
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

            RenderTexture.active = null;

            return outputScreen.GetPixels(0, 0, viewWidth * 2, viewHeight, 0);
        }

        public Color[] GetCanvasColor()
        {
            RenderTexture.active = canvas_Texture;
            canvasTexture2d.ReadPixels(new Rect(0, 0, viewWidth, viewHeight), 0, 0);
            canvasTexture2d.Apply();
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            return canvasTexture2d.GetPixels(0, 0, viewWidth, viewHeight, 0);
        }

        public void UpdateLastScreen(Color[] splitData)
        {
            //Graphics.CopyTexture(outputScreen, 0, 0, 0, 0, viewWidth, viewHeight, lastScreen, 0, 0, 0, 0);
            lastScreen.SetPixels(splitData);
            lastScreen.Apply();
        }
    }

}
