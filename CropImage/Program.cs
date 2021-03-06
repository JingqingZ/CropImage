﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CropImage
{
    internal class CropImage
    {
        public class Combo
        {
            public int xwc; // x-value or width or cols
            public int yhr; // y-value or height or rows

            public Combo()
            {
                this.xwc = 0;
                this.yhr = 0;
            }

            public Combo(int x, int y)
            {
                this.xwc = x;
                this.yhr = y;
            }
        }

        /// <summary>
        /// crop one images into cols*rows pieces
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="destDir">destination directory to save small images after division</param>
        /// <param name="numBlocks">cols and rows of blocks</param>
        /// <param name="blockSize">width and height of each block</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Combo CutImage(String imagePath, String destDir, Combo numBlocks, Combo blockSize, int mode)
        {
            // if parameters are invalid, return -1
            if (numBlocks.xwc <= 0 || numBlocks.yhr <= 0 ||
                blockSize.xwc <= 0 || blockSize.yhr <= 0)
            {
                return new Combo(-1, -1);
            }

            // read the origin image
            Image img = Image.FromFile(imagePath);

            // compute scale ratio
            int screenWidth = numBlocks.xwc*blockSize.xwc;
            int screenHeight = numBlocks.yhr*blockSize.yhr;
            double scaleWidth = (screenWidth + 0.000) / img.Width;
            double scaleHeight = (screenHeight + 0.000)/img.Height;

            int imgScaledWidth, imgScaledHeight;
            Combo smallImgNum = new Combo(); // cols and rows of small images after division
            
            //mode 1: scale to fill the whole screen, though may only display part of the origin image
            //mode 0: scale to fit the whole screen, and display the whole image

            if (scaleWidth > scaleHeight && mode == 1 ||
                scaleWidth < scaleHeight && mode == 0)
            {
                imgScaledWidth = (img.Width - 1)/numBlocks.xwc + 1; // ceiling
                imgScaledHeight = (imgScaledWidth*blockSize.yhr - 1)/blockSize.xwc + 1;
                smallImgNum.xwc = numBlocks.xwc;
                smallImgNum.yhr = Math.Max(numBlocks.yhr, (img.Height - 1)/imgScaledHeight + 1); // ceiling
            }
            else
            {
                imgScaledHeight = (img.Height - 1)/numBlocks.yhr + 1; // ceiling
                imgScaledWidth = (imgScaledHeight*blockSize.xwc - 1)/blockSize.yhr + 1; // ceiling and keep ratio
                smallImgNum.xwc = Math.Max(numBlocks.xwc, (img.Width - 1)/imgScaledWidth + 1); // ceiling
                smallImgNum.yhr = numBlocks.yhr;
            }

            // declaration of small images that will be generated by the origin image
            Image[] smallimg = new Image[smallImgNum.xwc*smallImgNum.yhr];

            //create & clean destination directory
            Directory.CreateDirectory(destDir);
            String[] filePaths = Directory.GetFiles(destDir);
            foreach (String filePath in filePaths)
                File.Delete(filePath);
            
            // crop
            for (int i = 0; i < smallImgNum.yhr; i++)
            {
                for (int j = 0; j < smallImgNum.xwc; j++)
                {
                    int rank = i*smallImgNum.xwc + j;
                    smallimg[rank] = new Bitmap(blockSize.xwc, blockSize.yhr);
                    Graphics graphics = Graphics.FromImage(smallimg[rank]);
                    graphics.DrawImage(img,
                        new Rectangle(0, 0, blockSize.xwc, blockSize.yhr),
                        new Rectangle(j*imgScaledWidth, i*imgScaledHeight, imgScaledWidth, imgScaledHeight),
                        GraphicsUnit.Pixel);
                    graphics.Dispose();
                    // save
                    smallimg[rank].Save(destDir + "\\" + rank + ".bmp", ImageFormat.Bmp);
                }
            }
            return smallImgNum; //if success
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            CropImage ci = new CropImage();
            Directory.SetCurrentDirectory("..\\..");
            Console.WriteLine(Directory.GetCurrentDirectory());
            ci.CutImage("Images\\ICL3.jpeg", "Images\\ICL3\\0", new Combo(5, 3), new Combo(819, 432), 0);
            ci.CutImage("Images\\ICL3.jpeg", "Images\\ICL3\\1", new Combo(5, 3), new Combo(819, 432), 1);
        }
    }
}
