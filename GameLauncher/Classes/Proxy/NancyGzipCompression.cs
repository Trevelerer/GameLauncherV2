using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace GameLauncher.Classes.Proxy
{
    public class NancyGzipCompression : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += CheckForCompression;
        }

        private static void CheckForCompression(NancyContext context)
        {
            if (!RequestIsGzipCompatible(context.Request))
            {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response))
            {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response))
            {
                return;
            }

            CompressResponse(context.Response);
        }

        private static void CompressResponse(Response response)
        {
            response.Headers["Content-Encoding"] = "gzip";
            response.Headers["connection"] = "close";

            var content = new MemoryStream();

            response.Contents(content);

            content.Position = 0;

            response.Contents = stream =>
            {
                using (var gzip = new GZipStream(stream, CompressionLevel.Fastest, true))
                {
                    gzip.Write(content.ToArray(), 0, (int) content.Length);
                }
            };
        }

        private static bool ContentLengthIsTooSmall(Response response)
        {
            //if (response.Headers.TryGetValue("Content-Length", out var contentLength))
            //{
            //    var length = long.Parse(contentLength);
            //    if (length < 4096)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        private static readonly List<string> ValidMimes = new List<string>
                                                {
                                                    "application/json",
                                                    "application/json; charset=utf-8"
                                                };

        private static bool ResponseIsCompatibleMimeType(Response response)
        {
            return true;
            //return ValidMimes.Any(x => x == response.ContentType);
        }

        private static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}
