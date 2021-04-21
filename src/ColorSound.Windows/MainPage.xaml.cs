using ColorSound.Process;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorSound.Windows
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
            var processor = new ColorSoundProcessor();
            _ = Task.Run(async () =>
              {
                  while (true)
                  {
                      await processor.RunProcessAsync(true);
                      Thread.Sleep(10);
                  }
              });
        }
    }
}
