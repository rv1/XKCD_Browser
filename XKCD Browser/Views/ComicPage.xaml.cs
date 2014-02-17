using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using XKCD_Browser.ViewModels;

namespace XKCD_Browser.Views
{
    public partial class ComicPage : PhoneApplicationPage
    {
        public ComicPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.New)
            {
                MainPageViewModel mainPageViewModel = new MainPageViewModel();
                mainPageViewModel.GetLatestComic();
                this.DataContext = mainPageViewModel;

            }
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ComicImage.Opacity == 0.5)
            {
                TextGrid.Opacity = 0;
                ComicImage.Opacity = 1;
            }
            else
            {
                TextGrid.Opacity = 1;
                ComicImage.Opacity = 0.5;
            }
        }
    }
}