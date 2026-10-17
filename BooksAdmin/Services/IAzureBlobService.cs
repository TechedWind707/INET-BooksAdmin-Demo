namespace BooksAdmin.Services
{
    public interface IAzureBlobService
    {
        //Service to upload a file to Azure Blob Storage and return the URL of the uploaded file as a string.

        public Task<string> UploadFileAsync(IFormFile file, string fileName);
    }

}