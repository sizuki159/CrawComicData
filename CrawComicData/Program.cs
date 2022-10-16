using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace CrawComicData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WebClient client = new WebClient())
            {

                string cookie = "visit-read=63381157ab16c-63381157ab16e; _ga=GA1.2.1766211325.1664618840; VinaHost-Shield=efe5487042804a144922f824a20f40dc; QiQiSession=mp9un231906mmlt5208qpfrq24; _gid=GA1.2.646586248.1665886188; _gat_gtag_UA_55970084_1=1";
                string useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";
                client.Encoding = Encoding.UTF8;

                client.Headers[HttpRequestHeader.UserAgent] = useragent;
                client.Headers[HttpRequestHeader.Cookie] = cookie;
                client.Headers[HttpRequestHeader.ContentType] = "text/html; charset=UTF-8";

                string raw = client.DownloadString("https://truyenqqpro.com/truyen-tranh/ajin-chan-wa-kataritai-11988");
                File.WriteAllText("data.html", raw);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(raw);
                HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//a[contains(@target, '_self')]");
                if (htmlNodes == null) throw new Exception("Null");
                if (htmlNodes.Count > 0)
                {
                    foreach (HtmlNode node in htmlNodes)
                    {
                        string urlChapter = node.GetAttributes("href").First().Value;

                        client.Headers[HttpRequestHeader.Cookie] = cookie;
                        client.Headers[HttpRequestHeader.UserAgent] = useragent;
                        client.Headers[HttpRequestHeader.ContentType] = "text/html; charset=UTF-8";

                        string content = client.DownloadString(urlChapter);
                        htmlDocument.LoadHtml(content);
                        HtmlNodeCollection htmlNodesContent = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'page-chapter')]");
                        if (htmlNodesContent.Count > 0)
                        {
                            List<string> filenames = new List<string>();
                            foreach (HtmlNode nodeContent in htmlNodesContent)
                            {
                                string matchString = Regex.Match(nodeContent.InnerHtml, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
                                if (string.IsNullOrEmpty(matchString))
                                    break;
                                matchString = matchString.Replace("?gf=hdfgdfg", string.Empty);
                                if (!matchString.Contains("http"))
                                    matchString = matchString.Replace("//", "https://");
                                client.Headers[HttpRequestHeader.UserAgent] = useragent;
                                client.Headers[HttpRequestHeader.ContentType] = "text/html; charset=UTF-8";
                                client.Headers[HttpRequestHeader.Cookie] = cookie;
                                client.Headers[HttpRequestHeader.Referer] = "https://truyenqqpro.com/";


                                string fileName = Path.GetFileName(matchString);
                                if (filenames.Contains(fileName))
                                    fileName = fileName + "";
                                filenames.Add(fileName);
                                client.DownloadFile(matchString, @"images\" + fileName);
                            }
                        }
                        return;
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
