using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows;
using XKCD_Browser.Models;

namespace XKCD_Browser.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        private bool _isLoading;

        private bool _shouldAssignLatest = false;

        public bool isLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("isLoading"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        private WebClient _webClient = new WebClient();

        private ComicItem result;

        public ComicItem ComicResult
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ComicResult"));
            }
        }

        public void GetLatestComic()
        {
            if (isLoading)
                return;

            _shouldAssignLatest = true;
            isLoading = true;
            string webAddr = App.Current.LatestComicAPI;
            _webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            _webClient.DownloadStringAsync(new Uri(webAddr));
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            _webClient.DownloadStringCompleted -= webClient_DownloadStringCompleted;
            
            if (e == null || e.Cancelled || e.Error != null)
            {
                MessageBox.Show("Seems like the internet connection is down or the website refused the request.\n:(\nWe are sad too that you couldnt continue.", "Error", MessageBoxButton.OK);
                isLoading = false;
                _shouldAssignLatest = false;
                return;
            }
            string json = e.Result;
            if (!string.IsNullOrEmpty(json))
            {
                ComicItem jsonResult = JsonConvert.DeserializeObject<ComicItem>(json);
                //ComicResult = JsonConvert.DeserializeObject<ComicItem>(json);

                //Converting date format
                var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
                jsonResult.month = dtf.GetAbbreviatedMonthName(Convert.ToInt32(jsonResult.month));
                ComicResult = jsonResult;
                if (_shouldAssignLatest && !(ComicResult.num == null || ComicResult.num == 0))
                {
                    App.Current.LatestComicNum = ComicResult.num;
                }
            }
            else
            {
                MessageBox.Show("Seems like the the servers are not responding as expected.\n:(\nI would be sad too.", "Error", MessageBoxButton.OK);
            }
            isLoading = false;
            _shouldAssignLatest = false;
        }

    }
}
