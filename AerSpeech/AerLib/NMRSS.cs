﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;


namespace AerSpeech
{
    public class RSSItem
    {
        public string Title;
        public string Description;

        public RSSItem(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }

    /// <summary>
    /// Creates a simple List of RSS entries.
    /// TODO: List -> IEnumerable for encapsulation sake.
    /// </summary>
    public class NMRSS
    {
        private XmlDocument _xmlDoc;
        private WebClient _webClient;
        public List<RSSItem> Entries;
        public bool Loaded;

        public NMRSS(string rssURL)
        {
            _xmlDoc = new XmlDocument();
            _webClient = new WebClient();
            Entries = new List<RSSItem>();
            Loaded = false;
            try
            {
                StreamReader rssFeed = new StreamReader(_webClient.OpenRead(rssURL));
                _xmlDoc.Load(new XmlTextReader(rssFeed));

                XmlNode rootRss = _xmlDoc.SelectSingleNode("rss");
                XmlNodeList channels = rootRss.ChildNodes;

                foreach (XmlNode channelNode in channels)
                {
                    XmlNodeList items = channelNode.SelectNodes("item");

                    foreach (XmlNode itemNode in items)
                    {
                        string title = itemNode.SelectSingleNode("title").InnerText;
                        string description = itemNode.SelectSingleNode("description").InnerText;
                        Entries.Add(new RSSItem(title, description));
                    }
                }
                Loaded = true;
            }
            catch (Exception e)
            {
                NMDebug.LogError("Problems loading RSS. " + e.Message);
            }
        }
    }
}
