using System.Threading.Tasks;

namespace CloudMall.Services.Storage.Services
{
    public interface IStorageProvider
    {
        Task<string> SaveBytes(byte[] bytes, string filePath);
    }
}