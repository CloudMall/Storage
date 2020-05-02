using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CloudMall.Services.Storage.Services
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly LocalStorageProviderOptions _options;

        public LocalStorageProvider(IOptions<LocalStorageProviderOptions> options)
        {
            _options = options.Value;
        }

        public Task<string> SaveBytes(byte[] bytes, string filePath)
        {
            var fullPath = $"{_options.BaseDir}/{filePath}";
            System.IO.File.WriteAllBytes(fullPath, bytes);
            return Task.FromResult(fullPath);
        }
    }

    public class LocalStorageProviderOptions
    {
        public string BaseDir { get; set; }
    }
}