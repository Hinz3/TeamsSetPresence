using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TeamsSetPresence.Core.Configurations;
using TeamsSetPresence.Core.DTOs;
using TeamsSetPresence.Core.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace TeamsSetPresence.Core.Services
{
    public class TeamsService : ITeamsService
    {
        private readonly HttpClient httpClient;
        private readonly AuthenticationConfig authenticationConfig;
        private readonly ILogger<TeamsService> logger;

        public TeamsService(HttpClient httpClient, AuthenticationConfig authenticationConfig, ILogger<TeamsService> logger)
        {
            this.httpClient = httpClient;
            this.authenticationConfig = authenticationConfig;
            this.logger = logger;
        }

        public async Task<string> GetAccessToken()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(authenticationConfig.ClientId)
                    .WithClientSecret(authenticationConfig.ClientSecret)
                    .WithAuthority(new Uri(authenticationConfig.Authority))
                    .Build();

            string[] scopes = new string[] { $"{authenticationConfig.ApiUrl}.default" };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token acquired");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
                logger.LogWarning("Scope provided is not supported");
            }

            return result.AccessToken;
        }

        public async Task<bool> ChangeStatus(string accessToken, string userId, string availability, string activity)
        {
            var busyActivities = new List<string>() { "inacall", "inaconferencecall" };
            if (availability.ToLower() == "available" && activity.ToLower() != "available")
            {
                throw new ArgumentException("Availability: Available can only be set with activity: available");
            }
            else if (availability.ToLower() == "away" && activity.ToLower() != "away")
            {
                throw new ArgumentException("Availability: away can only be set with activity: away");
            }
            else if (availability.ToLower() == "busy" && !busyActivities.Contains(activity.ToLower()))
            {
                throw new ArgumentException("Availability: busy can only be set with activity: inacall or inaconferencecall");
            }

            var uri = $"{authenticationConfig.ApiUrl}beta/users/{userId}/presence/setPresence";
            var body = new GraphTeamsStatusDTO()
            {
                SessionId = authenticationConfig.ClientId,
                Availability = availability,
                Activity = activity
            };

            var data = JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                //var errorMessage = await response.Content.ReadFromJsonAsync<GraphErrorDTO>();
                throw new ArgumentException(errorMessage);
            }

            return true;
        }

        public async Task<List<UserDTO>> GetUsers(string accessToken)
        {
            var uri = $"{authenticationConfig.ApiUrl}v1.0/users";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<GraphResponseDTO<UserDTO>>();
            return data.Value;

        }
    }
}
