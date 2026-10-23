using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MoviesAdmin.Services;

// Defines the upload method that the service must implement.
public interface IAzureBlobService
{
    // Service to upload file to zure Blob Storage
    // return: the URL of the uploaded file as a string
    public Task<string> UploadFileAsync(IFormFile file, string filename);


    //
    // TO-DO: add a new function to delete a file to handle when a trail record is deleted
    //
}