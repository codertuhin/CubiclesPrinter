using System;
using System.Drawing;
using Cubicles.Utility;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// This class contains different methods for work with images
    /// </summary>
    public class ImageHelper
    {
        /// HasColor
        #region HasColor

        /// <summary>
        /// Indicates whether the image has color or not
        /// </summary>
        /// <param name="image">image to be inspected</param>
        /// <returns>true if has color; otherwise false</returns>
        public static bool HasColor(Image image)
        {
            try
            {
                return !IsGrayScale(new Bitmap(image));
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return false;
            }
        }

        /// <summary>
        /// Scans image and determines if its' grayscaled
        /// </summary>
        /// <param name="img">image to be inspected</param>
        /// <returns>true if grayscaled; otherwise false</returns>
        private static bool IsGrayScale(Bitmap img)
        {
            for (Int32 h = 0; h < img.Height; h++)
                for (Int32 w = 0; w < img.Width; w++)
                {
                    Color color = img.GetPixel(w, h);
                    if ((color.R != color.G || color.G != color.B || color.R != color.B) && color.A != 0)
                    {
                        return false;
                    }
                }

            return true;
        }
        
        #endregion
    }
}
