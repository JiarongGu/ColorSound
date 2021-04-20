﻿using ColorSound.Configuration;
using ColorSound.Devices;
using ColorSound.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorSound
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var camera = new Camera();
            var mediaFrameFormats = await camera.GetMediaFrameFormatsAsync();
            ConfigurationFile.SetSupportedVideoFrameFormats(mediaFrameFormats);
            var videoSetting = await ConfigurationFile.Read(mediaFrameFormats);

            await camera.Initialize(videoSetting);
            camera.Start();
            
            var httpServer = new HttpServer(camera);
            httpServer.Start();
        }
    }
}
