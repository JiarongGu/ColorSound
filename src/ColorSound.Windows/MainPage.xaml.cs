using ColorSound.Process;
using ColorSound.Process.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ColorSound.Windows
{
    public sealed partial class MainPage : Page
    {
        SoftwareBitmapSource softwareBitmapSource;

        ColorSoundProcessor processor;

        bool play = false;

        double amplitude = 0.05;

        BitmapImage noPictureImage;

        static string ROOT = Package.Current.InstalledLocation.Path;
        static string NO_PICTURE_IMAGE = ROOT + @"\Assets\no_picture.jpg";


        public MainPage()
        {
            InitializeComponent();

            Window.Current.CoreWindow.PointerCursor = null;
            Window.Current.CoreWindow.PointerPosition = new Point(42, 42);

            softwareBitmapSource = new SoftwareBitmapSource();
            processor = new ColorSoundProcessor(amplitude);
            noPictureImage = new BitmapImage(new Uri(NO_PICTURE_IMAGE));

            Loaded += MainPage_Loaded;
            processor.FrameUpdated += ColorProcessor_FrameUpdated;
            processor.ColorUpdated += ColorProcessor_ColorUpdated;
            btnStart.Click += BtnStart_Click;
            silderVolume.ValueChanged += silderVolume_ValueChanged;

            imgVideo.Source = softwareBitmapSource;
            imgNoPicture.Source = noPictureImage;

            imgNoPicture.Visibility = Visibility.Visible;
            imgVideo.Visibility = Visibility.Collapsed;

            silderVolume.Value = amplitude * 500;
        }


        private void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            _ = Task.Run(async () =>
              {
                  while (true)
                  {
                      await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                      {
                          await processor.RunProcessAsync(play);
                      });

                      Thread.Sleep(20);
                  }
              });
        }

        private void ColorProcessor_FrameUpdated(object sender, SoftwareBitmap bitmap)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await softwareBitmapSource.SetBitmapAsync(bitmap);
            }).AsTask().ConfigureAwait(false);
        }

        private void ColorProcessor_ColorUpdated(object sender, System.Drawing.Color[] color)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                color1.Fill = new SolidColorBrush(color[0].GetUIColor());
                color1.Stroke = new SolidColorBrush(color[0].GetUIColor());

                color2.Fill = new SolidColorBrush(color[1].GetUIColor());
                color2.Stroke = new SolidColorBrush(color[1].GetUIColor());
            }).AsTask().ConfigureAwait(false);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            play = !play;
            if (play)
            {
                btnStart.Content = "PAUSE";
                imgVideo.Visibility = Visibility.Visible;
                imgNoPicture.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnStart.Content = "START";
                imgNoPicture.Visibility = Visibility.Visible;
                imgVideo.Visibility = Visibility.Collapsed;
            }
        }

        private void silderVolume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            processor.Amplitude = e.NewValue / 500;
        }
    }
}
