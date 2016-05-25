﻿using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System;
using Windows.UI;

namespace MyDevoxx.Utils
{
    public class RoundedTransformation : TransformationBase
    {
        private double _radius;
        private double _cropWidthRatio;
        private double _cropHeightRatio;

        private double _borderSize;
        private string _borderHexColor;

        public RoundedTransformation(double radius) : this(radius, 1d, 1d)
        {
        }

        public RoundedTransformation(double radius, double cropWidthRatio, double cropHeightRatio) : this(radius, cropWidthRatio, cropHeightRatio, 0d, null)
        {
        }

        public RoundedTransformation(double radius, double cropWidthRatio, double cropHeightRatio, double borderSize, string borderHexColor)
        {
            _radius = radius;
            _cropWidthRatio = cropWidthRatio;
            _cropHeightRatio = cropHeightRatio;
            _borderSize = borderSize;
            _borderHexColor = borderHexColor;
        }

        public override string Key
        {
            get
            {
                return string.Format("RoundedTransformation,radius={0},cropWidthRatio={1},cropHeightRatio={2},borderSize={3},borderHexColor={4}",
              _radius, _cropWidthRatio, _cropHeightRatio, _borderSize, _borderHexColor);
            }
        }

        protected override BitmapHolder Transform(BitmapHolder source)
        {
            return ToRounded(source, (int)_radius, _cropWidthRatio, _cropHeightRatio, _borderSize, _borderHexColor);
        }

        public static BitmapHolder ToRounded(BitmapHolder source, int rad, double cropWidthRatio, double cropHeightRatio, double borderSize, string borderHexColor)
        {
            double sourceWidth = source.Width;
            double sourceHeight = source.Height;

            double desiredWidth = sourceWidth;
            double desiredHeight = sourceHeight;

            double desiredRatio = cropWidthRatio / cropHeightRatio;
            double currentRatio = sourceWidth / sourceHeight;

            if (currentRatio > desiredRatio)
                desiredWidth = (cropWidthRatio * sourceHeight / cropHeightRatio);
            else if (currentRatio < desiredRatio)
                desiredHeight = (cropHeightRatio * sourceWidth / cropWidthRatio);

            double cropX = ((sourceWidth - desiredWidth) / 2);
            double cropY = ((sourceHeight - desiredHeight) / 2);

            BitmapHolder bitmap = null;

            if (cropX != 0 || cropY != 0)
            {
                bitmap = CropTransformation.ToCropped(source, (int)cropX, (int)cropY, (int)(desiredWidth), (int)(desiredHeight));
            }
            else
            {
                bitmap = new BitmapHolder(source.Pixels, source.Width, source.Height);
            }

            if (rad == 0)
                rad = (int)(Math.Min(desiredWidth, desiredHeight) / 2);
            else rad = (int)(rad * (desiredWidth + desiredHeight) / 2 / 500);

            int w = (int)desiredWidth;
            int h = (int)desiredHeight;

            int transparentColor = Colors.Transparent.ToInt();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x <= rad && y <= rad)
                    { //top left corner
                        if (!CheckRoundedCorner(rad, rad, rad, Corner.TopLeftCorner, x, y))
                            bitmap.Pixels[y * w + x] = transparentColor;
                    }
                    else if (x >= w - rad && y <= rad)
                    { // top right corner
                        if (!CheckRoundedCorner(w - rad, rad, rad, Corner.TopRightCorner, x, y))
                            bitmap.Pixels[y * w + x] = transparentColor;
                    }
                    else if (x >= w - rad && y >= h - rad)
                    { // bottom right corner
                        if (!CheckRoundedCorner(w - rad, h - rad, rad, Corner.BottomRightCorner, x, y))
                            bitmap.Pixels[y * w + x] = transparentColor;
                    }
                    else if (x <= rad && y >= h - rad)
                    { // bottom left corner
                        if (!CheckRoundedCorner(rad, h - rad, rad, Corner.BottomLeftCorner, x, y))
                            bitmap.Pixels[y * w + x] = transparentColor;
                    }
                }
            }

            //TODO draws a border - we should optimize that and add some anti-aliasing
            if (borderSize > 0d)
            {
                borderSize = (borderSize * (desiredWidth + desiredHeight) / 2d / 500d);
                int borderColor = Colors.Transparent.ToInt();

                try
                {
                    if (!borderHexColor.StartsWith("#", StringComparison.Ordinal))
                        borderHexColor.Insert(0, "#");
                    borderColor = borderHexColor.ToColorFromHex().ToInt();
                }
                catch (Exception)
                {
                }

                int intBorderSize = (int)Math.Ceiling(borderSize);

                for (int i = 2; i < intBorderSize; i++)
                {
                    CircleAA(bitmap, i, borderColor);
                }
            }

            return bitmap;
        }

        private enum Corner
        {
            TopLeftCorner,
            TopRightCorner,
            BottomRightCorner,
            BottomLeftCorner,
        }

        private static bool CheckRoundedCorner(int h, int k, int r, Corner which, int xC, int yC)
        {
            int x = 0;
            int y = r;
            int p = (3 - (2 * r));

            do
            {
                switch (which)
                {
                    case Corner.TopLeftCorner:
                        {   //Testing if its outside the top left corner
                            if (xC <= h - x && yC <= k - y) return false;
                            else if (xC <= h - y && yC <= k - x) return false;
                            break;
                        }
                    case Corner.TopRightCorner:
                        {   //Testing if its outside the top right corner
                            if (xC >= h + y && yC <= k - x) return false;
                            else if (xC >= h + x && yC <= k - y) return false;
                            break;
                        }
                    case Corner.BottomRightCorner:
                        {   //Testing if its outside the bottom right corner
                            if (xC >= h + x && yC >= k + y) return false;
                            else if (xC >= h + y && yC >= k + x) return false;
                            break;
                        }
                    case Corner.BottomLeftCorner:
                        {   //Testing if its outside the bottom left corner
                            if (xC <= h - y && yC >= k + x) return false;
                            else if (xC <= h - x && yC >= k + y) return false;
                            break;
                        }
                }

                x++;

                if (p < 0)
                {
                    p += ((4 * x) + 6);
                }
                else
                {
                    y--;
                    p += ((4 * (x - y)) + 10);
                }
            } while (x <= y);

            return true;
        }

        // helper function, draws pixel and mirrors it
        static void SetPixel4(BitmapHolder bitmap, int centerX, int centerY, int deltaX, int deltaY, int color)
        {
            bitmap.SetPixel(centerX + deltaX, centerY + deltaY, color);
            bitmap.SetPixel(centerX - deltaX, centerY + deltaY, color);
            bitmap.SetPixel(centerX + deltaX, centerY - deltaY, color);
            bitmap.SetPixel(centerX - deltaX, centerY - deltaY, color);
        }

        static void CircleAA(BitmapHolder bitmap, int size, int color)
        {
            if (size % 2 != 0)
                size++;

            int centerX = bitmap.Width / 2;
            double radiusX = (bitmap.Width - size) / 2;
            int centerY = bitmap.Height / 2;
            double radiusY = (bitmap.Height - size) / 2;

            const int maxTransparency = 127; // default: 127
            double radiusX2 = radiusX * radiusX;
            double radiusY2 = radiusY * radiusY;

            // upper and lower halves
            int quarter = (int)Math.Round(radiusX2 / Math.Sqrt(radiusX2 + radiusY2));

            for (int x = 0; x <= quarter; x++)
            {
                double y = Math.Floor(radiusY * Math.Sqrt(1 - x * x / radiusX2));
                double error = y - Math.Floor(y);
                int transparency = (int)Math.Round(error * maxTransparency);
                int alpha = color | (transparency << 24);
                int alpha2 = color | ((maxTransparency - transparency) << 24);
                SetPixel4(bitmap, centerX, centerY, x, (int)Math.Floor(y), alpha);
                SetPixel4(bitmap, centerX, centerY, x, (int)Math.Floor(y) + 1, alpha2);
            }

            // right and left halves
            quarter = (int)Math.Round(radiusY2 / Math.Sqrt(radiusX2 + radiusY2));

            for (int y = 0; y <= quarter; y++)
            {
                double x = Math.Floor(radiusX * Math.Sqrt(1 - y * y / radiusY2));
                double error = x - Math.Floor(x);
                int transparency = (int)Math.Round(error * maxTransparency);
                int alpha = color | (transparency << 24);
                int alpha2 = color | ((maxTransparency - transparency) << 24);
                SetPixel4(bitmap, centerX, centerY, (int)Math.Floor(x), y, alpha);
                SetPixel4(bitmap, centerX, centerY, (int)Math.Floor(x) + 1, y, alpha2);
            }
        }
    }
}
