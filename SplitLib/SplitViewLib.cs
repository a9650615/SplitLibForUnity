using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleToAttribute("Birdyo")]

namespace SplitLib
{

    public class SplitViewLib
    {
        public int pieceW = 20;
        public int pieceH = 20;
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
                RenderTexture.active = rt;
                screenShot.Resize(resWidth, resHeight);
            }

            //camera.Render();

            //SplitTexture();
            //screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            int cutWidth = lastWidth / pieceW;
            int cutHeight = lastHeight / pieceH;
            for (var i = 0; i <= pieceH; i++)
            {
                int j;
                for (j = 0; j < (pieceW / 2) - 1; j += 2)
                {
                    int posX = cutWidth * j;
                    int posY = cutHeight * i;
                    //copy left
                    screenShot.ReadPixels(new Rect(posX, posY, cutWidth, cutHeight), posX, posY);
                    //copy right
                    screenShot.ReadPixels(new Rect(posX + resWidth / 2, posY, cutWidth, cutHeight), posX + cutWidth, posY);
                }

                for (; j < pieceW; j += 2)
                {
                    int posX = cutWidth * j;
                    int posY = cutHeight * i;
                    //copy left
                    screenShot.ReadPixels(new Rect(posX - (resWidth / 2) + cutWidth, posY, cutWidth, cutHeight), posX, posY);
                    //copy right
                    screenShot.ReadPixels(new Rect(posX + cutWidth, posY, cutWidth, cutHeight), posX + cutWidth, posY);
                }
                //Debug.Log(j);
                //Debug.Log(pieceW);
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
