using System.Windows;
using System.Windows.Controls;

namespace Aris.Services
{
    public interface IPageSizing
    {
        double PreferredWidth { get; }
        double PreferredHeight { get; }
    }
}