
using System.Windows;
using System.Windows.Controls;


namespace Aris.Services
{   
    public static class NavService
    {
        public static event Action<string> ?On_OpenViewer;
        public static event Action ?On_GoHome;

        public static IPageSizeStore SizeStore { get; set; } = new PageSizeFile();

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
            var window = Window.GetWindow(host);            

            if (window != null && host.Content is IPageSizing && host.Content != newContent)
            {
                var IsMaximized = window.WindowState == WindowState.Maximized;
                var (width, height) = IsMaximized ? (window.RestoreBounds.Width, window.RestoreBounds.Height) : (window.Width, window.Height);

                SizeStore.Save(host.Content.GetType().Name, window.Width, window.Height, IsMaximized);
            }

            if (host.Content is IDisposable disposable) disposable.Dispose();

            host.Content = newContent;

            if (window != null && newContent is IPageSizing sizing)
            {
                var saved = SizeStore.Get(newContent.GetType().Name);

                window.WindowState = WindowState.Normal;
                window.Width = saved?.Width ?? sizing.PreferredWidth;
                window.Height = saved?.Height ?? sizing.PreferredHeight;
                window.Left = (SystemParameters.PrimaryScreenWidth - window.Width) / 2;
                window.Top = (SystemParameters.PrimaryScreenHeight - window.Height) / 2;

                if (saved?.IsMaximized == true) window.WindowState = WindowState.Maximized;
            }
        }
    }
}