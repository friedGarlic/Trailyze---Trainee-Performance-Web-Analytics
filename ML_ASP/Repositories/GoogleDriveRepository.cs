using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Accord.IO;
using Microsoft.AspNetCore.Hosting;

namespace ML_ASP.Repositories
{
    public static class GoogleDriveService
    {
        private static readonly string[] Scopes = { DriveService.Scope.Drive };

        public static DriveService GetDriveService(string apiKey = null)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                // Use OAuth 2.0 for user-specific data
                UserCredential credential;

                using (var stream = new FileStream(@"D:\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    String folderPath = @"D:\";
                    String filePath = Path.Combine(folderPath, "DriveServiceCredentials.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(filePath)).Result;

                    Console.WriteLine("Authentication succeeded.");
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Trailyze",
                });

                Console.WriteLine("DriveService initialized successfully.");
                return service;
            }
            else
            {
                // Use API key for public data
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "Trailyze",
                });

                Console.WriteLine("DriveService initialized with API key successfully.");
                return service;
            }
        }

    }

}
