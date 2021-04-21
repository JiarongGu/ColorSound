using ColorSound.Process.Helper;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Storage.Streams;
using Windows.UI.Core;


namespace ColorSound.Process
{
    public class ImageCapture : IDisposable
    {
        private MediaCapture _mediaCapture;
        private MediaFrameReader _mediaFrameReader;

        public ImagePixelData ImagePixelData { get; set; }

        public void Dispose()
        {
            if (_mediaCapture != null)
                _mediaCapture.Dispose();

            if (_mediaFrameReader != null)
                _mediaFrameReader.Dispose();
        }

        public async Task StartAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var mediaCapture = await InitializeMediaCaptureAsync();

                var defaultFormat = mediaCapture.FrameSources.First().Value.SupportedFormats
                    .OrderByDescending(m => m.FrameRate.Numerator / (decimal)m.FrameRate.Denominator).First();

                var mediaFrameReader = await InitializeFrameReaderAsync(defaultFormat);
                await mediaFrameReader.StartAsync();
            });
        }

        public async Task StopAsync() 
        {
            await _mediaFrameReader.StopAsync();
        }

        private void FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            GetImagePixelDataAsync().Wait();
        }

        public async Task GetImagePixelDataAsync()
        {
            var frame = _mediaFrameReader.TryAcquireLatestFrame();

            SoftwareBitmap frameBitmapTry = null;
            if (frame?.VideoMediaFrame?.Direct3DSurface != null)
            {
                frameBitmapTry = SoftwareBitmap.CreateCopyFromSurfaceAsync(frame.VideoMediaFrame.Direct3DSurface).AsTask().Result;
            }
            else if (frame?.VideoMediaFrame?.SoftwareBitmap != null)
            {
                frameBitmapTry = frame.VideoMediaFrame.SoftwareBitmap;
            }

            if (frameBitmapTry == null)
                return;


            using (var frameBitmap = frameBitmapTry)
            using (var stream = new InMemoryRandomAccessStream())
            using (var bitmap = SoftwareBitmap.Convert(frameBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(bitmap);
                await encoder.FlushAsync();

                var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, stream);
                var pixelData = (await decoder.GetPixelDataAsync()).DetachPixelData();
                var pixels = new byte[decoder.PixelWidth, decoder.PixelHeight][];
                var pixelColors = new Color[decoder.PixelWidth, decoder.PixelHeight];

                for (var h = 0; h < decoder.PixelHeight; h++)
                {
                    for (var w = 0; w < decoder.PixelWidth; w++)
                    {
                        var offset = h * w * 4;
                        var red = pixelData[offset];
                        var green = pixelData[offset + 1];
                        var blue = pixelData[offset + 2];
                        var alpha = pixelData[offset + 3];

                        pixels[w, h] = new byte[] { red, green, blue, alpha };
                        pixelColors[w, h] = Color.FromArgb(alpha, red, green, blue);
                    }
                }

                ImagePixelData = new ImagePixelData(decoder.PixelWidth, decoder.PixelHeight, pixels, pixelColors);
            }
        }


        public async Task<MediaCapture> InitializeMediaCaptureAsync()
        {
            if (_mediaCapture != null)
                return _mediaCapture;

            _mediaCapture = new MediaCapture();

            var settings = new MediaCaptureInitializationSettings()
            {
                // With CPU the results contain always SoftwareBitmaps, otherwise with GPU
                // they preferring D3DSurface
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,

                //Capture only video, no audio
                StreamingCaptureMode = StreamingCaptureMode.Video
            };

            await _mediaCapture.InitializeAsync(settings);

            // Set exposure (auto light adjustment)
            var exposureCapabilities = _mediaCapture.VideoDeviceController.Exposure.Capabilities;
            if (exposureCapabilities.Supported && exposureCapabilities.AutoModeSupported)
            {
                _mediaCapture.VideoDeviceController.Exposure.TrySetAuto(true);
            }

            return _mediaCapture;
        }

        public async Task<MediaFrameReader> InitializeFrameReaderAsync(MediaFrameFormat mediaFrameFormat)
        {
            if (_mediaFrameReader != null)
            {
                return _mediaFrameReader;
            }

            var mediaFrameSource = _mediaCapture.FrameSources.First().Value;
            var videoDeviceController = mediaFrameSource.Controller.VideoDeviceController;

            videoDeviceController.DesiredOptimization = Windows.Media.Devices.MediaCaptureOptimization.Quality;
            videoDeviceController.PrimaryUse = Windows.Media.Devices.CaptureUse.Video;

            //Set resolution, frame rate and video subtyp
            var videoFormat = mediaFrameSource.SupportedFormats
                .Where(sf => sf.VideoFormat.Width == mediaFrameFormat.VideoFormat.Width
                    && sf.VideoFormat.Height == mediaFrameFormat.VideoFormat.Height
                    && sf.Subtype == mediaFrameFormat.Subtype)
                .OrderByDescending(m => m.FrameRate.Numerator / (decimal)m.FrameRate.Denominator)
                .First();

            await mediaFrameSource.SetFormatAsync(videoFormat);

            _mediaFrameReader = await _mediaCapture.CreateFrameReaderAsync(mediaFrameSource);
            _mediaFrameReader.FrameArrived += FrameArrived;
            return _mediaFrameReader;
        }
    }
}