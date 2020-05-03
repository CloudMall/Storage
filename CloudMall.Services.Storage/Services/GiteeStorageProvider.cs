using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WeihanLi.Extensions;

namespace CloudMall.Services.Storage.Services
{
    public class GiteeStorageProvider : IStorageProvider
    {
        private const string PostFileApiUrlFormat = "https://gitee.com/api/v5/repos/{0}/{1}/contents{2}";
        private const string RawFileUrlFormat = "https://gitee.com/{0}/{1}/raw/master{2}";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly GiteeStorageOptions _options;

        public GiteeStorageProvider(HttpClient httpClient, ILogger<GiteeStorageProvider> logger, IOptions<GiteeStorageOptions> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> SaveBytes(byte[] bytes, string filePath)
        {
            var base64Str = Convert.ToBase64String(bytes);
            var requestUrl = PostFileApiUrlFormat.FormatWith(_options.UserName, _options.RepositoryName, filePath);
            using (var response = await _httpClient.PostAsFormAsync(requestUrl,
                new Dictionary<string, string>
                {
                    { "access_token", _options.AccessToken },
                    { "content", base64Str },
                    { "message" , $"add file" }
                }))
            {
                if (response.IsSuccessStatusCode)
                {
                    return RawFileUrlFormat
                        .FormatWith(_options.UserName, _options.RepositoryName, filePath);
                }

                var result = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"post file error, response: {result}");

                return null;
            }
        }
    }

    public class GiteeStorageOptions
    {
        public string UserName { get; set; }

        public string RepositoryName { get; set; }

        public string AccessToken { get; set; }
    }
}