using Coding4Fun.Toolkit.Controls;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Input;
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

        private int currentComic;

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

        private Random rnd = new Random();

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

        private bool getComicAtIndex(int index)
        {
            if (index > App.Current.LatestComicNum || index <= 0)
                return false;

            if (isLoading)
                return false;

            _shouldAssignLatest = false;
            isLoading = true;
            currentComic = index;
            string webAddr = "http://xkcd.com/" + (currentComic).ToString() + "/info.0.json";
            _webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            _webClient.DownloadStringAsync(new Uri(webAddr));
            return true;
        }

        public void getPreviousComic()
        {
            getComicAtIndex(currentComic - 1);
        }

        public void getNextComic()
        {
            getComicAtIndex(currentComic + 1);
        }

        public void getRandomComic()
        {
            getComicAtIndex(rnd.Next(App.Current.LatestComicNum));
        }

        public void getLatestComic()
        {
            getComicAtIndex(App.Current.LatestComicNum);
        }

        public void getOldestComic()
        {
            getComicAtIndex(1);
        }

        public void getNumberedComic()
        {
            InputPrompt input = new InputPrompt();
            input.Title = "Enter Comic Number";
            input.Message = "Range: 0 to " + App.Current.LatestComicNum;
            input.BorderThickness = new Thickness(1);
            input.IsCancelVisible = true;
            input.Completed += input_Completed;
            input.InputScope = new InputScope { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number} } };
            input.Show();

            //getComicAtIndex(66);

        }

        private void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult.ToString() == "Ok")
            {
                if (!string.IsNullOrWhiteSpace(e.Result))
                {
                    if (!getComicAtIndex(Convert.ToInt32(e.Result)))
                    {
                        MessageBox.Show("I wish I could go and fetch that comic, but seems like it does not exist.\nNice try though.", "Error", MessageBoxButton.OK);
                    }
                }
            }

            //getComicAtIndex(66);
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
                if (_shouldAssignLatest && !(ComicResult.num == 0))
                {
                    App.Current.LatestComicNum = ComicResult.num;
                    currentComic = ComicResult.num;
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
