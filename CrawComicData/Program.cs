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
            const string URL_COMIC = "https://truyenqqpro.com/truyen-tranh/toan-chuc-phap-su-4140";
            const string cookie = "visit-read=63381157ab16c-63381157ab16e; _ga=GA1.2.1766211325.1664618840; QiQiSession=c9u91jrhtue2560pf22169o149; _gid=GA1.2.1636582465.1666315837; VinaHost-Shield=9426c53cda69d4661ae853c6bc1b88ea";
            const string useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";

            // Init
            if(!Directory.Exists("comic"))
                Directory.CreateDirectory("comic");


            using (MyWebClient client = new MyWebClient(useragent, cookie))
            {
                string rawComic = client.DownloadString(URL_COMIC);
                HtmlDocument htmlDocumentComic = new HtmlDocument();
                htmlDocumentComic.LoadHtml(rawComic);

                // Get Infor Comic
                HtmlNode avatarNode = htmlDocumentComic.DocumentNode.SelectSingleNode("//div[contains(@class, 'book_avatar')]");
                HtmlNode nameNode = htmlDocumentComic.DocumentNode.SelectSingleNode("//h1[contains(@itemprop, 'name')]");
                HtmlNode descriptionNode = htmlDocumentComic.DocumentNode.SelectSingleNode("//div[contains(@class, 'story-detail-info detail-content')]");
                string avatarURL = avatarNode.ChildNodes.FirstOrDefault().GetAttributes("src").First().Value.Replace("?gf=hdfgdfg&mobile=2", string.Empty);
                string name = nameNode.InnerText.Trim();
                string desc = descriptionNode.InnerText.Trim();
                string nameSlug = name.GenerateSlug();
                string pathComic = Path.Combine("comic", nameSlug);
                if (!Directory.Exists(pathComic))
                    Directory.CreateDirectory(pathComic);
                // End Get Infor

                Console.WriteLine("Start Craw " + name);

                MyWebClient.SetHeader(client, useragent, cookie);
                //client.Headers[HttpRequestHeader.Referer] = "https://truyenqqpro.com/";
                client.DownloadFile(avatarURL, Path.Combine(pathComic, Path.GetFileName(avatarURL)));

                using (StreamWriter sw = File.AppendText(Path.Combine(pathComic, "info.txt")))
                {
                    sw.WriteLine(name);
                    sw.WriteLine(desc);
                }

                HtmlNodeCollection htmlNodes = htmlDocumentComic.DocumentNode.SelectNodes("//a[contains(@target, '_self')]");

                if (htmlNodes.Count == 0) throw new Exception("Empty");

                if (htmlNodes.Count > 0)
                {
                    foreach (HtmlNode node in htmlNodes)
                    {
                        // Get Infor Chapter
                        string chapterName = node.InnerText;
                        string urlChapter = node.GetAttributes("href").First().Value;

                        string pathChapter = Path.Combine(pathComic, chapterName);
                        if (!Directory.Exists(pathChapter))
                            Directory.CreateDirectory(pathChapter);
                        // End Get Infor

                        Console.WriteLine("Crawling " + chapterName);

                        MyWebClient.SetHeader(client, useragent, cookie);

                        string content = client.DownloadString(urlChapter);

                        HtmlDocument htmlDocumentChapter = new HtmlDocument();
                        htmlDocumentChapter.LoadHtml(content);
                        HtmlNodeCollection htmlNodesContent = htmlDocumentChapter.DocumentNode.SelectNodes("//div[contains(@class, 'page-chapter')]");
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

                                MyWebClient.SetHeader(client, useragent, cookie);
                                client.Headers[HttpRequestHeader.Referer] = "https://truyenqqpro.com/";


                                string fileName = Path.GetFileName(matchString);
                                if (filenames.Contains(fileName))
                                    fileName = fileName + "";
                                filenames.Add(fileName);
                                client.DownloadFile(matchString, Path.Combine(pathChapter, fileName));
                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
