using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace Scream
{
    class ExtUtils
    {

        /// <summary>
        /// get Clipboard
        /// </summary>
        /// <returns></returns>
        public static string GetClipboardData()
        {
            string strData = string.Empty;
            try
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data.GetDataPresent(DataFormats.Text))
                {
                    strData = data.GetData(DataFormats.Text).ToString();
                }
                return strData;
            }
            catch
            {
            }
            return strData;
        }

        /// <summary>
        /// set Clipboard
        /// </summary>
        /// <returns></returns>
        public static void SetClipboardData(string strData)
        {
            try
            {
                Clipboard.SetText(strData);
            }
            catch
            {
            }
        }

        /// <summary>
        /// HttpWebRequest
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrl(string url)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                req.Timeout = 5000;
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                stream.Close();
                resp.Close();
                req.Abort();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// speed test via http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHttpStatusTime(string url)
        {
            int responseTime = 0;
            int testcount = 0;
            do
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Timeout = 5000;
                    Stopwatch responsetimer = new Stopwatch();
                    responsetimer.Start();
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    responsetimer.Stop();
                    TimeSpan ts = responsetimer.Elapsed;
                    responseTime = (int)ts.TotalMilliseconds;
                    resp.Close();
                    req.Abort();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                testcount++;
            } while (testcount < 2);
            return responseTime.ToString();
        }

        /// <summary>
        /// speed test via http proxy request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetHttpStatusTime(string url, int port)
        {
            int responseTime = 0;
            int testcount = 0;
            do
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Timeout = 5000;
                    WebProxy proxy = new WebProxy("127.0.0.1", port);
                    req.Proxy = proxy;
                    Stopwatch responsetimer = new Stopwatch();
                    responsetimer.Start();
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    responsetimer.Stop();
                    TimeSpan ts = responsetimer.Elapsed;
                    responseTime = (int)ts.TotalMilliseconds;
                    resp.Close();
                    req.Abort();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                testcount++;
            } while (testcount < 2);
            return responseTime.ToString();
        }

        /// <summary>
        /// Base64 encode
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            try
            {
                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Base64 decode
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Decode(string plainText)
        {
            try
            {
                plainText = plainText.Trim()
                  .Replace("\n", "")
                  .Replace("\r\n", "")
                  .Replace("\r", "")
                  .Replace(" ", "");

                if (plainText.Length % 4 > 0)
                {
                    plainText = plainText.PadRight(plainText.Length + 4 - plainText.Length % 4, '=');
                }

                byte[] data = Convert.FromBase64String(plainText);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// autostart ,set
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static bool AutoStartSet(bool enabled)
        {
            try
            {
                string applicationPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + System.AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                RegistryKey runKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (enabled)
                {
                    runKey.SetValue("Scream", applicationPath);
                }
                else
                {
                    runKey.DeleteValue("Scream");
                }
                runKey.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// autostart ,check
        /// </summary>
        /// <returns></returns>
        public static bool AutoStartCheck()
        {
            try
            {
                RegistryKey runKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                string[] runList = runKey.GetValueNames();
                runKey.Close();
                foreach (string item in runList)
                {
                    if (item.Equals("Scream"))
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// json key to lower
        /// </summary>
        /// <param name="jsonObject"></param>
        public static void ChangePropertiesToLowerCase(JObject jsonObject)
        {
            foreach (JProperty property in jsonObject.Properties().ToList())
            {
                if (property.Value.Type == JTokenType.Object)// replace property names in child object
                    ChangePropertiesToLowerCase((JObject)property.Value);

                property.Replace(new JProperty(property.Name.ToLower(), property.Value));// properties are read-only, so we have to replace them
            }
        }

        /// <summary>
        /// gc collect
        /// </summary>
        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
