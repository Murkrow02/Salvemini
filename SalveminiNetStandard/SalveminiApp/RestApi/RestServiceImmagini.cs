using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
    public class RestServiceImmagini: ImageService
    {
        HttpClient client;

        public RestServiceImmagini()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
            client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));
        }


        public async Task<bool> UploadImageAsync(Stream image, string fileName, string percorso)
        {
            try
            {
                var url = Costants.Uri("images/upload/" + percorso + "/" + fileName);
                HttpContent fileStreamContent = new StreamContent(image);
                fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = fileName };
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
             
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent);
                    var response = await client.PostAsync(url, formData);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
           
        }

        public async Task<string[]> DeleteImage()
        {
            var uri = Costants.Uri("images/deletepic");

            try
            {
                var response = await client.GetAsync(uri);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new string[] { "Perfetto", "La tua immagine è stata rimossa" };
                    case HttpStatusCode.InternalServerError:
                        return new string[] { "Errore", "Si è verificato un errore, riprova più tardi o contattaci se il problema persiste" };
                    default:
                        return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Rimuovi immagine", ex.Message);
                return new string[] { "Errore", "Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste" };
            }
        }

        public async Task<bool> UploadGiornalinoAsync(Stream file)
        {
            try
            {
                var url = Costants.Uri("giornalino/upload");
                HttpContent fileStreamContent = new StreamContent(file);
                fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "pdf", FileName = "giornalino" };
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent);
                    var response = await client.PostAsync(url, formData);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

        }
    }

    public interface ImageService
    {
        Task<bool> UploadImageAsync(Stream image, string fileName, string percorso);
        Task<string[]> DeleteImage();
        Task<bool> UploadGiornalinoAsync(Stream file);
    }

    public class ImageManager
    {
        ImageService apiService;


        public ImageManager(ImageService service)
        {
            apiService = service;
        }

        public Task<bool> uploadImages(Stream image, string fileName, string percorso)
        {
            return apiService.UploadImageAsync(image, fileName, percorso);
        }

        public Task<bool> UploadGiornalinoAsync(Stream file)
        {
            return apiService.UploadGiornalinoAsync(file);
        }

        public Task<string[]> DeleteImage()
        {
            return apiService.DeleteImage();
        }

    }

}
