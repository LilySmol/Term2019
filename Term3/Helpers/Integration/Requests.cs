using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TErm.Helpers.Integration
{
    public abstract class Requests
    {
        /// <summary>
        /// Выполняет GET запрос по privateToken и url.
        /// </summary>
        protected string get(string privateToken, string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers["Private-Token"] = privateToken;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseToString = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseToString = sr.ReadToEnd();
                    }
                }
                return responseToString;
            }
            catch (WebException e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error(e.ToString());
                return "";
            }
        }
    }
}