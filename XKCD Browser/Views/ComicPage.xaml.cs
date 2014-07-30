using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows.Navigation;
using XKCD_Browser.ViewModels;

namespace XKCD_Browser.Views
{
    public partial class ComicPage : PhoneApplicationPage
    {
        public ComicPage()
        {
            InitializeComponent();
            Loaded += ComicPage_Loaded;
            Unloaded += ComicPage_Unloaded;
        }

        void ComicPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_vm == null)
                return;

            _vm.PropertyChanged -= _vm_PropertyChanged;
        }

        private MainPageViewModel _vm;

        void ComicPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm = DataContext as MainPageViewModel;
            if(_vm == null)
                return;

            _vm.PropertyChanged += _vm_PropertyChanged;
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ComicResult")
            {
                ResetImageTransformation();
                ImageTransformation.ScaleX = 1;
                ImageTransformation.ScaleY = 1;
            }
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
            if (ComicImage.Opacity != 1)
            {
                ComicImage.IsHitTestVisible = true;
                TextGrid.Opacity = 0;
                ComicImage.Opacity = 1;
            }
            else
            {
                ComicImage.IsHitTestVisible = false;
                TextGrid.Opacity = 1;
                ComicImage.Opacity = 0.4;
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getPreviousComic();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getRandomComic();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getNextComic();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).saveCurrentComic();
        }

        private void LatestButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getLatestComic();
        }

        private void OldestButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getOldestComic();
        }

        private void gotoButton_Click(object sender, EventArgs e)
        {
            (this.DataContext as MainPageViewModel).getNumberedComic();
        }

        private void RateButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask oRateTask = new MarketplaceReviewTask();
            oRateTask.Show();
        }

        //source for pinch zoom http://codecopy.wordpress.com/2011/12/15/wp7-pinch-and-pan-zoom-an-image/
        private System.Windows.Point Center;
        private double InitialScale;

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            // Store the initial rotation angle and scaling
            InitialScale = ImageTransformation.ScaleX;
            // Calculate the center for the zooming
            System.Windows.Point firstTouch = e.GetPosition(ComicImage, 0);
            System.Windows.Point secondTouch = e.GetPosition(ComicImage, 1);

            Center = new System.Windows.Point(firstTouch.X + (secondTouch.X - firstTouch.X) / 2.0, firstTouch.Y + (secondTouch.Y - firstTouch.Y) / 2.0);
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            // If its less that the original  size or more than 4x then don’t apply
            if (InitialScale * e.DistanceRatio > 4 || (InitialScale != 1 && e.DistanceRatio == 1) || InitialScale * e.DistanceRatio < 1)
                return;

            // If its original size then center it back
            if (e.DistanceRatio <= 1.08)
            {
                ResetImageTransformation();
            }

            ImageTransformation.CenterX = Center.X;
            ImageTransformation.CenterY = Center.Y;

            // Update the rotation and scaling
            if (this.Orientation == PageOrientation.Landscape)
            {
                // When in landscape we need to zoom faster, if not it looks choppy
                ImageTransformation.ScaleX = InitialScale * (1 + (e.DistanceRatio - 1) * 2);
            }
            else
            {
                ImageTransformation.ScaleX = InitialScale * e.DistanceRatio;
            }
            ImageTransformation.ScaleY = ImageTransformation.ScaleX;
        }

        private void ResetImageTransformation()
        {
            ImageTransformation.CenterY = 0;
            ImageTransformation.CenterY = 0;
            ImageTransformation.TranslateX = 0;
            ImageTransformation.TranslateY = 0;
        }

        private void Image_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            // if is not touch enabled or the scale is different than 1 then don’t allow moving
            if (ImageTransformation.ScaleX <= 1.1)
                return;

            double centerX = ImageTransformation.CenterX;
            double centerY = ImageTransformation.CenterY;
            double translateX = ImageTransformation.TranslateX;
            double translateY = ImageTransformation.TranslateY;
            double scale = ImageTransformation.ScaleX;
            double width = ComicImage.ActualWidth;
            double height = ComicImage.ActualHeight;

            // Verify limits to not allow the image to get out of area
            if (centerX - scale * centerX + translateX + e.HorizontalChange < 0 && centerX + scale * (width - centerX) + translateX + e.HorizontalChange > width)
            {
                ImageTransformation.TranslateX += e.HorizontalChange;
            }

            if (centerY - scale * centerY + translateY + e.VerticalChange < 0 && centerY + scale * (height - centerY) + translateY + e.VerticalChange > height)
            {
                ImageTransformation.TranslateY += e.VerticalChange;
            }

            return;
        }

        //private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        //{
        //    var vm = DataContext as MainPageViewModel;
        //    if (vm != null)
        //    {
        //        if (e.HorizontalVelocity < 0)
        //        {
        //            (this.DataContext as MainPageViewModel).getNextComic();
        //        }
        //        else if (e.HorizontalVelocity > 0)
        //        {
        //            (this.DataContext as MainPageViewModel).getPreviousComic();
        //        }
        //    }
        //}
    }
}