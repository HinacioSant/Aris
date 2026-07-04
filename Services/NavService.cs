
using System.Windows.Controls;

namespace Aris.Services
{   
    public static class NavService
    {
        public static event Action<string> ?On_OpenViewer;
        public static event Action ?On_GoHome;

        public static void OpenViewer(string filepath)
        {
            On_OpenViewer?.Invoke(filepath);
        }

        public static void GoHome()
        {
            On_GoHome?.Invoke();
        }

        public static void SetContent(ContentControl host, object newContent)
        {
            if (host.Content is IDisposable disposable) disposable.Dispose();

            host.Content = newContent;
        }
    }
}