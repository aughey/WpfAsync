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
    public class MessageReceiver
    {
        public event Action? TestCompleted;

        public MessageReceiver()
        {
            Task.Run(() => ListenForMessages());
        }

        private void ListenForMessages()
        {
            // Simulate listening for messages
            while (true)
            {
                // Simulate message reception
                Thread.Sleep(1000); // Wait for 1 second to simulate receiving messages

                // Simulate detecting a specific message indicating test completion
                OnTestCompleted();
            }
        }

        protected virtual void OnTestCompleted()
        {
            TestCompleted?.Invoke();
        }
    }

    public class ExternalHardwareAsync
    {
        private MessageReceiver receiver;
        private TaskCompletionSource<bool>? tcs;

        public ExternalHardwareAsync(MessageReceiver receiver)
        {
            this.receiver = receiver;
        }

        public Task<bool> StartTestAsync(CancellationToken cancellationToken)
        {
            tcs = new TaskCompletionSource<bool>();

            cancellationToken.Register(() =>
            {
                receiver.TestCompleted -= OnTestCompleted; // Unsubscribe to prevent memory leaks
                tcs.TrySetCanceled(); // Set the task as canceled
            });

            receiver.TestCompleted += OnTestCompleted; // Subscribe to completion event
            SendTestCommand(); // Mock sending a command to the hardware

            return tcs.Task;
        }

        private void OnTestCompleted()
        {
            receiver.TestCompleted -= OnTestCompleted; // Unsubscribe from event
            tcs?.SetResult(true); // Signal that the task is complete
        }

        private void SendTestCommand()
        {
            // Simulate sending a command to the hardware
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        private ExternalHardwareAsync externalHardwareAsync;

        public SecondaryWindow()
        {
            InitializeComponent();
            externalHardwareAsync = new ExternalHardwareAsync(new MessageReceiver());
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayLabel.Content = "Starting test...";
            UpdateButton.IsEnabled = false;

            using var cts = new CancellationTokenSource();
            Task delayTask = Task.Delay(500, cts.Token);
            Task<bool> testTask = externalHardwareAsync.StartTestAsync(cts.Token);

            var completedTask = await Task.WhenAny(testTask, delayTask);

            if (completedTask == testTask && testTask.Status == TaskStatus.RanToCompletion)
            {
                // Test completed successfully
                DisplayLabel.Content = "Test completed successfully";
            }
            else
            {
                // Timeout occurred or task was otherwise canceled
                DisplayLabel.Content = "Test failed or was canceled";
            }
            cts.Cancel(); // Ensure the test task is canceled if still running

            UpdateButton.IsEnabled = true;
        }
    }
}