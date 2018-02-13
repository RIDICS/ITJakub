using System;
using System.Linq;
using Microsoft.Net.Http.Headers;

namespace Vokabular.Shared.AspNetCore.Helpers
{
    public static class UploadHelper
    {
        public const int MultipartReaderBufferSize = 80 * 1024;

        public static string GetBoundary(string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            var elements = contentType.Split(' ');
            var element = elements.First(entry => entry.StartsWith("boundary="));
            var boundary = element.Substring("boundary=".Length);

            boundary = HeaderUtilities.RemoveQuotes(boundary).Value;

            return boundary;
        }
    }
}
