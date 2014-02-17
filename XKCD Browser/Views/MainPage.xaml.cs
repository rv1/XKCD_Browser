using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows.Input;
using System.Windows.Navigation;
using XKCD_Browser.ViewModels;

namespace XKCD_Browser
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
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

        private void AllComics_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ComicPage.xaml", UriKind.Relative));
        }

        private void RateButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask oRateTask = new MarketplaceReviewTask();
            oRateTask.Show();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).GetLatestComic();
        }

    }
}