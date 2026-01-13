using System.Windows;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs EventArgs)
        {
            base.OnStartup(EventArgs);
            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (RequestSender, EventArgs) =>
            {
                LogUnhandledException((Exception)EventArgs.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            DispatcherUnhandledException += (RequestSender, EventArgs) =>
            {
                LogUnhandledException(EventArgs.Exception, "Application.Current.DispatcherUnhandledException");
                EventArgs.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (RequestSender, EventArgs) =>
            {
                LogUnhandledException(EventArgs.Exception, "TaskScheduler.UnobservedTaskException");
                EventArgs.SetObserved();
            };
        }
        private void LogUnhandledException(Exception Exception, string HandlingSource)
        {
            rin(Exception.ToString());
        }
    }
}