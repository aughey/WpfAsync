namespace WpfAsync
{
    using System.Threading.Tasks;

    public class ExternalHardware
    {
        private bool isTestComplete;

        public ExternalHardware()
        {
            isTestComplete = false;
        }

        public void StartTest()
        {
            isTestComplete = false;
            Task.Delay(1000).ContinueWith(_ => isTestComplete = true);  // Simulates test process
        }

        public bool TestComplete
        {
            get { return isTestComplete; }
        }
    }
}