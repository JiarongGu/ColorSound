using ColorSound.Process;
using GrovePi;
using GrovePi.Sensors;
using System.Threading.Tasks;
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


        private void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var processor = new ColorSoundProcessor(0.3);
            var button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin2);
            
            _ = Task.Run(async () =>
            {
                var lastButtonState = SensorStatus.Off;
                var output = false;

                while (true)
                {
                    if (button.CurrentState != lastButtonState)
                    {
                        lastButtonState = button.CurrentState;
                        if (button.CurrentState == SensorStatus.On)
                        {
                            output = !output;
                        }
                    }

                    await processor.RunProcessAsync(output);
                };
            });
        }
    }
}
