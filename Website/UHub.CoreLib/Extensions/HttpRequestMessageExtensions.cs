using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Util.APIControllers.FileUpload;

namespace UHub.CoreLib.Extensions
{
    /// <summary>
    /// HttpRequestMessage extensions
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Check if a request message is part of a chunked upload
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsChunkUpload(this HttpRequestMessage request)
        {
            return request.Content.Headers.ContentRange != null;
        }

        /// <summary>
        /// Get chunk metadata from a request message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static FileUploadChunkMetaData GetChunkMetaData(this HttpRequestMessage request)
        {
            return new FileUploadChunkMetaData()
            {
                ChunkIdentifier = request.Headers.Contains("X-File-Identifier") ? request.Headers.GetValues("X-File-Identifier").FirstOrDefault() : null,
                ChunkStart = request.Content.Headers.ContentRange.From,
                ChunkEnd = request.Content.Headers.ContentRange.To,
                TotalLength = request.Content.Headers.ContentRange.Length
            };
        }
    }
}
