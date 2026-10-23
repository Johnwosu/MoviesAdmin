using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesAdmin.Services;

public class AzureBlobService : IAzureBlobService
{
    // Used to connect to Azure Blob Storage / Azurite.
    private readonly BlobServiceClient _blobServiceClient;

    // Stores the name of the Blob container, for example "movieapp".
    private readonly string _containerName;

    // Constructor receives application configuration.
    public AzureBlobService(IConfiguration configuration)
    {
        // Read the Blob container name from configuration / User Secrets.
        _containerName =
            configuration["BlobContainerName"]
            ?? throw new InvalidOperationException(
                "Storage container name is missing.");

        // Read the Azure Storage connection string from configuration / User Secrets.
        string connectionString =
            configuration.GetConnectionString("AzureStorage")
            ?? throw new InvalidOperationException(
                "Storage connection string is missing.");

        // Create the Blob Storage client using the connection string.
        _blobServiceClient =
            new BlobServiceClient(connectionString);
    }

    // Uploads a file to Blob Storage and returns its URL.
    public async Task<string> UploadFileAsync(
        IFormFile file,
        string filename)
    {
        // Get the Blob container where movie images will be stored.
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(
                _containerName);

        // Get a reference to the Blob using the supplied filename.
        var blobClient =
            containerClient.GetBlobClient(filename);

        // Set the uploaded file's content type.
        // This helps the browser recognize it as an image.
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        };

        // Open the uploaded file as a stream and upload it to Blob Storage.
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(
                stream,
                options);
        }

        // Return the URL of the uploaded image.
        // This URL can be saved in Movie.ImageUrl.
        return blobClient.Uri.ToString();
    }
}