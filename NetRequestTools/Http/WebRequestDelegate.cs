using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NetRequestTools.Http
{
    public class WebRequestDelegate
    {
        private static readonly Dictionary<string, Func<string, string, WebRequest>> webFactoryMethods =
            new Dictionary<string, Func<string, string, WebRequest>>
            {
                {WebRequestMethods.Http.Get, CreateGetRequest},
                {WebRequestMethods.Http.Post, CreatePostRequest},
            };

        public string RequestType { get; set; }

        public string Payload { get; set; }

        public string BaseUrl { get; set; }

        public Dictionary<string, string> ResponseHeaders { get; private set; }

        public void MakeRequest( HttpRequestFormat format )
        {
            RequestType = format.RequestType;
            Payload = format.Payload;
            BaseUrl = format.BaseUrl;
            MakeRequest();
        }

        public void MakeRequest()
        {
            WebRequest request = webFactoryMethods[ RequestType ]( BaseUrl, Payload );
            WebResponse response = request.GetResponse();

            ResponseHeaders = new Dictionary<string, string>();
            WebHeaderCollection headers = response.Headers;
            headers.AllKeys.ToList().ForEach( key => ResponseHeaders.Add( key, headers[ key ] ) );
        }

        private static WebRequest CreatePostRequest( string baseUrl, string payload )
        {
            byte[] encodedPayload = Encoding.ASCII.GetBytes( payload );
            WebRequest request = WebRequest.Create( baseUrl );
            request.Method = WebRequestMethods.Http.Post;
            request.ContentLength = encodedPayload.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write( encodedPayload, 0, encodedPayload.Length );
            requestStream.Close();

            return request;
        }

        private static WebRequest CreateGetRequest( string baseUrl, string payload )
        {
            payload = payload.TrimStart( '?' );
            WebRequest request = WebRequest.Create( baseUrl + "?" + payload );
            request.Method = WebRequestMethods.Http.Get;
            return request;
        }
    }
}
