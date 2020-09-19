using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Utilities;

namespace Tipper
{
    public partial class TwitterHelper
    {
        private string APIKey;
        private string APISecretKey;
        private string BearerToken;
        private string AccessToken;
        private string AccessTokenSecret;
        string oAuthUrl = "https://api.twitter.com/1.1/statuses/update.json";

        public TwitterHelper()
        {
            //TODO: should pull this from a Keystore
            var file = Filey.Load(@"C:\Users\Jack\Documents\Visual Studio 2012\Projects\AFLTipper\keys.keys").Split('\n');
            APIKey = file [0].Replace("API key: ", "").Replace("\r", "");
            APISecretKey = file[1].Replace("API secret key: ","").Replace("\r", "");
            BearerToken = file[2].Replace("Bearer token: ", "").Replace("\r", "");
            AccessToken = file[3].Replace("Access token: ", "").Replace("\r", "");
            AccessTokenSecret = file[4].Replace("Access token secret: ", "").Replace("\r", "");
        }

        public void SendTweet(string message)
        {
            string authHeader = GenerateAuthorizationHeader(message);
            string postBody = "status=" + Uri.EscapeDataString(message);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            authRequest.Headers.Add("Authorization", authHeader);
            authRequest.Method = "POST";
            authRequest.UserAgent = "OAuth gem v0.4.4";
            authRequest.Host = "api.twitter.com";
            authRequest.ContentType = "application/x-www-form-urlencoded";
            authRequest.ServicePoint.Expect100Continue = false;
            authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (Stream stream = authRequest.GetRequestStream())
            {
                byte[] content = Encoding.UTF8.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            WebResponse authResponse = authRequest.GetResponse();
            authResponse.Close();
        }

        private string GenerateAuthorizationHeader(string status)
        {
            string signatureMethod = "HMAC-SHA1";
            string version = "1.0";
            string nonce = GenerateNonce();
            double timestamp = ConvertToUnixTimestamp(DateTime.Now);
            string dst = string.Empty;

            dst = string.Empty;
            dst += "OAuth ";
            dst += string.Format("oauth_consumer_key=\"{0}\", ", Uri.EscapeDataString(APIKey));
            dst += string.Format("oauth_nonce=\"{0}\", ", Uri.EscapeDataString(nonce));
            dst += string.Format("oauth_signature=\"{0}\", ", Uri.EscapeDataString(GenerateOauthSignature(status, nonce, timestamp.ToString())));
            dst += string.Format("oauth_signature_method=\"{0}\", ", Uri.EscapeDataString(signatureMethod));
            dst += string.Format("oauth_timestamp=\"{0}\", ", timestamp);
            dst += string.Format("oauth_token=\"{0}\", ", Uri.EscapeDataString(AccessToken));
            dst += string.Format("oauth_version=\"{0}\"", Uri.EscapeDataString(version));
            return dst;
        }

        private string GenerateOauthSignature(string status, string nonce, string timestamp)
        {
            string signatureMethod = "HMAC-SHA1";
            string version = "1.0";
            string result = string.Empty;
            string dst = string.Empty;

            dst += string.Format("oauth_consumer_key={0}&", Uri.EscapeDataString(APIKey));
            dst += string.Format("oauth_nonce={0}&", Uri.EscapeDataString(nonce));
            dst += string.Format("oauth_signature_method={0}&", Uri.EscapeDataString(signatureMethod));
            dst += string.Format("oauth_timestamp={0}&", timestamp);
            dst += string.Format("oauth_token={0}&", Uri.EscapeDataString(AccessToken));
            dst += string.Format("oauth_version={0}&", Uri.EscapeDataString(version));
            dst += string.Format("status={0}", Uri.EscapeDataString(status));

            string signingKey = string.Empty;
            signingKey = string.Format("{0}&{1}", Uri.EscapeDataString(APISecretKey), Uri.EscapeDataString(AccessTokenSecret));

            result += "POST&";
            result += Uri.EscapeDataString(oAuthUrl);
            result += "&";
            result += Uri.EscapeDataString(dst);

            HMACSHA1 hmac = new HMACSHA1();
            hmac.Key = Encoding.UTF8.GetBytes(signingKey);

            byte[] databuff = System.Text.Encoding.UTF8.GetBytes(result);
            byte[] hashbytes = hmac.ComputeHash(databuff);

            return Convert.ToBase64String(hashbytes);
        }

        private string GenerateNonce()
        {
            string nonce = string.Empty;
            var rand = new Random();
            int next = 0;
            for (var i = 0; i < 32; i++)
            {
                next = rand.Next(65, 90);
                char c = Convert.ToChar(next);
                nonce += c;
            }

            return nonce;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
