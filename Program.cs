using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Minio;
using Minio.Exceptions;

namespace minio_dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Minio upload example");
            
            // Create MinioClient
            // (with environment variables)
            // var minioClient = MinioHelper.GetClient();
            
            // Create MinioClient
            // (without environment variables)
            var minioClient = MinioHelper.GetClient(
                endpoint: "127.0.0.1:9000",
                accessKey: "1YHNYC7P460FO1LEXMU4",
                secretKey: "izVV+LrqxZb2GLctgBNKhGjLQaXyxcJad8tQGtdF",
                useSSL: false
            );

            // Upload file
            try
            {
                MinioHelper.Upload(
                    minioClient: minioClient,
                    bucketName: "animals",
                    objectName: "fox.jpg",
                    filePath: "sample/fox.jpg",
                    location: "us-east-1",

                    // List of MIME types
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types
                    contentType: "image/jpeg"
                ).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // End
            Environment.Exit(0);
        }

    }


    class MinioHelper {
        public static MinioClient GetClient(string endpoint="", string accessKey="", string secretKey="", bool useSSL=false)
        {
            Dictionary<string, string> minioConfig = new Dictionary<string, string>();

            // Use Environment Variables
            if (
                Environment.GetEnvironmentVariable("MINIO_ENDPOINT") != null &&
                endpoint != ""
            )
            {
                endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
                accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
                secretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");
            }
            
            if (Environment.GetEnvironmentVariable("MINIO_USESSL") != null)
            {
                if (Environment.GetEnvironmentVariable("MINIO_USESSL") == "true")
                    useSSL = true;
                else
                    useSSL = false;
            }

            // Create Minio Client
            MinioClient minio = null;
            try
            {
                if (useSSL)
                    minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();
                else
                    minio = new MinioClient(endpoint, accessKey, secretKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return minio;
        }

         // File uploader task.
        public async static Task Upload(
            MinioClient minioClient,
            string bucketName,
            string objectName,
            string filePath,
            string location,
            string contentType
        )
        {
            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minioClient.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minioClient.MakeBucketAsync(bucketName, location);
                }

                // Upload a file to bucket.
                await minioClient.PutObjectAsync(bucketName, objectName, filePath, contentType);
                Console.WriteLine("Successfully uploaded " + objectName );
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }
    }
}
