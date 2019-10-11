using System;
using System.IO;
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
    }

    public interface ImageService
    {
        Task<bool> UploadImageAsync(Stream image, string fileName, string percorso);
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



    }

}
