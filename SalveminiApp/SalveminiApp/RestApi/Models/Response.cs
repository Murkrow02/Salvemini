using System;
using System.Net.Http;

namespace SalveminiApp.RestApi.Models
{
    public class ResponseModel
    {
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
