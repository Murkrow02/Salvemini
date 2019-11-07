using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using SalveminiApi.Models;

namespace SalveminiApi.Helpers
{
    public class Utility
    {
        DatabaseString db = new DatabaseString();

        public static DateTime italianTime()
        {
            var todayDate = DateTime.UtcNow;
            var italianDate = todayDate.AddHours(2);
            return italianDate;
        }

       public static void addToAnalytics(string valore)
        {
            DatabaseString db2 = new DatabaseString();

            try
            {
                var data = Helpers.Utility.italianTime();
                var esiste = db2.Analytics.Where(x => x.Giorno.Year == data.Year && x.Giorno.Month == data.Month && x.Giorno.Day == data.Day && x.Tipo == valore).ToList();
                if (esiste.Count < 1)
                {
                    var accesso = new Analytics { Giorno = data, Tipo = valore, Valore = 1 };
                    db2.Analytics.Add(accesso);
                }
                else
                {
                    esiste[0].Valore = esiste[0].Valore + 1;
                }
                db2.SaveChanges();
            }
            catch (Exception ex)
            {
                //Fa niente
            }
        }

        public bool authorized(HttpRequestMessage re, int minStatus = 0)
        {
            string token;
            string id;
            var headers = re.Headers;
            if (headers.Contains("x-auth-token"))
            {
                token = headers.GetValues("x-auth-token").First();

                //Null Token
                if (string.IsNullOrEmpty(token))
                    return false;

                if (headers.Contains("x-user-id")){
                    id = headers.GetValues("x-user-id").First();

                    //Null id
                    if (string.IsNullOrEmpty(id))
                        return false;

                    //Check token with user
                    var utente = db.Utenti.Find(Convert.ToInt32(id));
                    if (utente.ArgoToken != token)
                        return false;
                    else
                    {
                        //Check user status
                        if (utente.Stato >= minStatus)
                            return true;
                        else
                            return false;
                    }
                }
                else
                {
                    //No id in headers
                    return false;
                }
            }
            else
            {
                //No token in headers
                return false;
            }
        }

         public static void ReduceImageSize(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbnailImg.Save(targetPath, image.RawFormat);
            }
        }

        public static void CropImage(Stream stream, int width, int height, string path)
        {
            Image source = System.Drawing.Image.FromStream(stream);

            Image result = null;

            try
            {
                if (source.Width != width || source.Height != height)
                {
                    // Resize image
                    float sourceRatio = (float)source.Width / source.Height;

                    using (var target = new Bitmap(width, height))
                    {
                        using (var g = System.Drawing.Graphics.FromImage(target))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            // Scaling
                            float scaling;
                            float scalingY = (float)source.Height / height;
                            float scalingX = (float)source.Width / width;
                            if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                            int newWidth = (int)(source.Width / scaling);
                            int newHeight = (int)(source.Height / scaling);

                            // Correct float to int rounding
                            if (newWidth < width) newWidth = width;
                            if (newHeight < height) newHeight = height;

                            // See if image needs to be cropped
                            int shiftX = 0;
                            int shiftY = 0;

                            if (newWidth > width)
                            {
                                shiftX = (newWidth - width) / 2;
                            }

                            if (newHeight > height)
                            {
                                shiftY = (newHeight - height) / 2;
                            }

                            // Draw image
                            g.DrawImage(source, -shiftX, -shiftY, newWidth, newHeight);
                        }

                        result = (Image)target.Clone();
                    }
                }
                else
                {
                    // Image size matched the given size
                    result = (Image)source.Clone();
                }
            }
            catch (Exception)
            {
                result = null;
            }

            result.Save(path, result.RawFormat);
        }

    }

    public static class Extensions {

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }

}