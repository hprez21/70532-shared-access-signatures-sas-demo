using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = GetContainer("images");
            //ListBlobs(container);

            CreateCORSSetting(GetClient(GetAccount()));

            Console.WriteLine("Operación exitosa");
            Console.ReadLine();

        }

        static CloudStorageAccount GetAccount()
        {
            return CloudStorageAccount.Parse(
                CloudConfigurationManager
                .GetSetting("StorageData"));
        }

        static CloudBlobClient GetClient(CloudStorageAccount account)
        {
            return account.CreateCloudBlobClient();
        }

        static CloudBlobContainer
            GetContainer(string containerName)
        {
            var storageAccount = GetAccount();

            var client = GetClient(storageAccount);

            var container = client
                .GetContainerReference(containerName);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            return container;
        }

        static void ListBlobs(CloudBlobContainer container)
        {
            var blobs = container.ListBlobs();

            //var sas = GetSASToken(GetAccount());
            var sas = container.GetSharedAccessSignature(null, "directiva");

            foreach (var blob in blobs)
            {
                Console.WriteLine(blob.Uri + sas);
                Process.Start(blob.Uri.AbsoluteUri + sas);
            }
        }

        static string GetSASToken(CloudStorageAccount account)
        {
            SharedAccessAccountPolicy policy =
                new SharedAccessAccountPolicy()
                {
                    Permissions = SharedAccessAccountPermissions.Read
                    | SharedAccessAccountPermissions.List,

                    Services = SharedAccessAccountServices.Blob,
                    ResourceTypes = SharedAccessAccountResourceTypes.Object,
                    SharedAccessExpiryTime = DateTime.Now.AddMinutes(30),
                    Protocols = SharedAccessProtocol.HttpsOnly
                };
            return account.GetSharedAccessSignature(policy);
        }

        static void CreateCORSSetting(CloudBlobClient blobClient)
        {
            ServiceProperties sp =
                new ServiceProperties();
            sp.Cors = new CorsProperties();

            sp.Cors.CorsRules.Add(new CorsRule()
            {
                AllowedMethods = CorsHttpMethods.Get,
                AllowedOrigins = new List<string>()
                {
                    "https://hprez21.com"
                },
                MaxAgeInSeconds = 3600
            });
            blobClient.SetServiceProperties(sp);

        }

    }
}
