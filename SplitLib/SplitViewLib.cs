using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleToAttribute("Birdyo")]

namespace SplitLib
{

    public class SplitViewLib
    {
        internal int piece = 4;
        internal int lastWidth;
        internal int lastHeight;
        internal Texture2D screenShot;
        internal RenderTexture rt;



        static void Main() { }

        public SplitViewLib()
        {
            screenShot = new Texture2D(100, 100, TextureFormat.RGB24, false);
            rt = new RenderTexture(100, 100, 24);
        }

        public Texture2D GetOutPutTextureFromCamera(Camera camera)
        {
            int resWidth = Screen.width;
            int resHeight = Screen.height;

            camera.targetTexture = rt; //Create new renderTexture and assign to camera
            if (lastWidth != resWidth || lastHeight != resHeight)
            {
                lastWidth = resWidth;
                lastHeight = resHeight;
                rt = new RenderTexture(resWidth, resHeight, 24);
                screenShot.Resize(resWidth, resHeight);
            }

            camera.Render();

            RenderTexture.active = rt;

            int cutWidth = resWidth / piece;
            int cutHeight = resWidth / piece;
            for (var i = 0; i < piece; i++)
            {
                for (var j = 0; j < piece / 2; j += 2)
                {
                    int posX = cutWidth * j;
                    int posY = cutHeight * i;
                    //copy left
                    screenShot.ReadPixels(new Rect(posX, posY, cutWidth, cutHeight), posX, posY);
                    //copy right
                    screenShot.ReadPixels(new Rect(posX + resWidth / 2, posY, cutWidth, cutHeight), posX + cutWidth, posY);
                }

                for (var j = piece / 2; j < piece; j += 2)
                {
                    int posX = cutWidth * j;
                    int posY = cutHeight * i;
                    //copy left
                    screenShot.ReadPixels(new Rect(posX - resWidth / piece, posY, cutWidth, cutHeight), posX, posY);
                    //copy right
                    screenShot.ReadPixels(new Rect(posX + cutWidth, posY, cutWidth, cutHeight), posX + cutWidth, posY);
                }
            }

            //screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0); //Apply pixels from camera onto Texture2D
            screenShot.Apply();



            //outPutView.texture = (Texture)screenShot;
            //byte[] _bytes = screenShot.EncodeToPNG();
            //System.IO.File.WriteAllBytes("./www.png", _bytes);
            //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);

            camera.targetTexture = null;
            RenderTexture.active = null; //Clean
            //Destroy(rt); //Free memory
            return screenShot;
        }
    }

}
