using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace ColorSound
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static MediaCapture mediaCapture = new MediaCapture();

        private static bool initalized = false;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if (!initalized) {
                initalized = true;

                mediaCapture.Failed += new MediaCaptureFailedEventHandler(OnMediaCaptureFailed);

                try
                {
                    await mediaCapture.InitializeAsync();
                }
                catch (Exception e) 
                {
                    
                }
            }

            
            var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreatePng(), stream);

        }

        private void OnMediaCaptureFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) { 
            
        }
    }
}
