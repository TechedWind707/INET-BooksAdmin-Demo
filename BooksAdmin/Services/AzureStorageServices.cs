using Azure.Storage.Blobs;


namespace BooksAdmin.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            containerName = _configuration["AzureStorage:ContainerName"] ?? throw new ArgumentNullException("AzureStorage:ContainerName");
            string connectionString = _configuration["AzureStorage:ConnectionString"] ?? throw new ArgumentNullException("AzureStorage:ConnectionString");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string filename)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(filename);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}