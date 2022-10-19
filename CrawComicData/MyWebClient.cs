using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CrawComicData
{
    internal class MyWebClient : WebClient
    {
        public MyWebClient() { }
        public MyWebClient(string useragent, string cookie, string contentType = "text/html; charset=UTF-8")
        {
            this.Encoding = Encoding.UTF8;
            this.Headers[HttpRequestHeader.UserAgent] = useragent;
            this.Headers[HttpRequestHeader.Cookie] = cookie;
            this.Headers[HttpRequestHeader.ContentType] = contentType;
        }
        public static MyWebClient Create(string useragent, string cookie, string contentType = "text/html; charset=UTF-8")
        {
            MyWebClient client = new MyWebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers[HttpRequestHeader.UserAgent] = useragent;
            client.Headers[HttpRequestHeader.Cookie] = cookie;
            client.Headers[HttpRequestHeader.ContentType] = contentType;
            return client;
        }

        public static MyWebClient Clone(MyWebClient clientOriginal)
        {
            MyWebClient client = new MyWebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers[HttpRequestHeader.UserAgent] = clientOriginal.Headers[HttpRequestHeader.UserAgent];
            client.Headers[HttpRequestHeader.Cookie] = clientOriginal.Headers[HttpRequestHeader.Cookie];
            client.Headers[HttpRequestHeader.ContentType] = client.Headers[HttpRequestHeader.Cookie];
            return client;
        }

        public static void SetHeader(MyWebClient client, string useragent, string cookie, string contentType = "text/html; charset=UTF-8")
        {
            client.Encoding = Encoding.UTF8;
            client.Headers[HttpRequestHeader.UserAgent] = useragent;
            client.Headers[HttpRequestHeader.Cookie] = cookie;
            client.Headers[HttpRequestHeader.ContentType] = contentType;
        }
    }
}
