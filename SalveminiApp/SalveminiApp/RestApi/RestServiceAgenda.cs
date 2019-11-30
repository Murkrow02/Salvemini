using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace SalveminiApp.RestApi
{
	public class RestServiceAgenda : IRestServiceAgenda
	{
		HttpClient client;
		public List<Models.Compiti> Compiti = new List<Models.Compiti>();

		public RestServiceAgenda()
		{
			client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(15);
			client.DefaultRequestHeaders.Add("x-user-id", Preferences.Get("UserId", 0).ToString());
			client.DefaultRequestHeaders.Add("x-auth-token", Preferences.Get("Token", ""));

		}

		public async Task<List<Models.Compiti>> GetCompitiAgenda(int idGiorno)
		{
			Compiti = new List<Models.Compiti>();
			var uri = Costants.Uri("agenda/compiti/" + idGiorno);

			try
			{
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
                    Compiti = JsonConvert.DeserializeObject<List<Models.Compiti>>(content);
                }
                else
                {
                    return null;
                }

			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"Error GET agenda compiti", ex.Message);
                return null;
			}
			return Compiti;
		}
	}

	public interface IRestServiceAgenda
	{
		Task<List<Models.Compiti>> GetCompitiAgenda(int idGiorno);

	}

	public class AgendaManager
	{
        IRestServiceAgenda restServiceAgenda;

		public AgendaManager(IRestServiceAgenda serviceAgenda)
		{
            restServiceAgenda = serviceAgenda;
		}

		public Task<List<Models.Compiti>> GetCompitiAgenda(int idGiorno)
		{
			return restServiceAgenda.GetCompitiAgenda(idGiorno);
		}
	}
}
