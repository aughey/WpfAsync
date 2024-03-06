using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer pollTimer;
        private DispatcherTimer timeoutTimer;
        private ExternalHardware external;

        public MainWindow()
        {
            InitializeComponent();

            external = new ExternalHardware();

            // Initialize the polling timer
            pollTimer = new DispatcherTimer();
            pollTimer.Interval = TimeSpan.FromMilliseconds(100); // Poll every tenth of a second
            pollTimer.Tick += PollTimer_Tick;

            // Initialize the timeout timer
            timeoutTimer = new DispatcherTimer();
            timeoutTimer.Interval = TimeSpan.FromSeconds(3); // Set timeout to 3 seconds
            timeoutTimer.Tick += TimeoutTimer_Tick;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayLabel.Content = "Sending test command";
            external.StartTest(); // Start the test
            pollTimer.Start(); // Start polling for test completion
            timeoutTimer.Start(); // Start the timeout timer
        }

        private void PollTimer_Tick(object? sender, EventArgs e)
        {
            if (external.TestComplete)
            {
                DisplayLabel.Content = "Test complete";
                pollTimer.Stop(); // Stop the polling timer
                timeoutTimer.Stop(); // Test completed in time, stop the timeout timer
            }
        }

        private void TimeoutTimer_Tick(object ?sender, EventArgs e)
        {
            // If this timer fires, then the test did not complete in 3 seconds
            DisplayLabel.Content = "Test failed - timed out";
            pollTimer.Stop(); // Stop the polling timer as we know the test has failed
            timeoutTimer.Stop(); // Stop the timeout timer as it has already triggered
        }

    }
}