using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSTEKParser
{
    public class Downloader
    {
        public static void Download(string url, string localFileName)
        {
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(url, localFileName);
            }
            catch (WebException)
            {
                MessageBox.Show("Ошибка сети", "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
