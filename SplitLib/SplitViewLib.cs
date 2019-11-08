using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleToAttribute("Birdyo")]

namespace SplitLib
{

    public class SplitViewLib
    {
        public int pieceW = 1000;
        public int pieceH = 1;
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

        private Color[] ProcessColor(Color[] inputColor, int type)
        {
            Color[] copyPix = (Color[])Array.CreateInstance(typeof(Color), inputColor.Length);
            int resWidth = lastWidth;
            int resHeight = lastHeight;
            int cutWidth = lastWidth / pieceW;
            int cutHeight = lastHeight / pieceH;
            int splitWidth = lastWidth / 2;
            //Debug.Log(pix[0]);
            for (var x = 0; x < lastWidth; x++)
            {
                for (var y = 0; y < resHeight; y++)
                {
                    //copyPix[x + y * resWidth] = Color.black;
                    int nowPart = x % cutWidth;
                    if (type == 1)
                    {
                        if (x < splitWidth)
                        {
                            // put first
                            if (nowPart < cutWidth / 2)
                            {
                                copyPix[x + splitWidth / 2 + y * resWidth] = inputColor[x + y * resWidth];
                            }
                            else
                            {
                                copyPix[x + splitWidth / 2 + y * resWidth] = inputColor[x + splitWidth + y * resWidth];
                            }
                        }
                    }
                    else
                    {
                        // left view
                        if (x < splitWidth)
                        {
                            // put first
                            if (nowPart < cutWidth / 2)
                            {
                                copyPix[x + y * resWidth] = inputColor[x + y * resWidth];
                            }
                            else
                            {
                                copyPix[x + y * resWidth] = inputColor[(x + splitWidth) + y * resWidth];
                            }
                        }
                        else
                        {
                            // right
                            if (nowPart < cutWidth / 2)
                            {
                                copyPix[x + y * resWidth] = inputColor[(x) + y * resWidth];
                            }
                            else
                            {
                                copyPix[x + y * resWidth] = inputColor[(x - splitWidth) + y * resWidth];
                            }
                        }
                    }
                }
            }

            return copyPix;
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
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            Color[] pix = screenShot.GetPixels(0, 0, resWidth, resHeight, 0);

            // 防呆
            if (pieceW > (resWidth / 2))
            {
                pieceW = resWidth / 2;
            }
            if (pieceH > (resHeight / 2))
            {
                pieceH = resHeight / 2;
            }

            Color[] copyPix = ProcessColor(pix, 1);

            screenShot.SetPixels(copyPix);
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
