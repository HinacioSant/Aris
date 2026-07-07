using Aris.Models;
using Aris.Services;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Aris.Services
{
    public class PageSizeFile : IPageSizeStore
    {
        private readonly string _filePath;
        private readonly Dictionary<string, PageSize> _sizes;
        public PageSizeFile(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "Aris", "settings.Json");
            _sizes = Load();
        }

        private Dictionary<string, PageSize> Load()
        {
            if (!File.Exists(_filePath)) return [];

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<Dictionary<string, PageSize>>(json) ?? [];
            }
            catch (Exception ex)
            {
            var result = ErrorHandler.Handler(ex, nameof(PageSizeFile));
            Debug.WriteLine(result.Error);
            return [];
                
            }
        }

        public void Save(string pageKey, double Width, double height, bool IsMaximized)
        {
            _sizes[pageKey] = new PageSize(Width, height, IsMaximized);

            try
            {
                var dir = Path.GetDirectoryName(_filePath)!;
                Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_sizes);
                var tempPath = Path.Combine(dir, $"temp_{Guid.NewGuid()}.temp");
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, _filePath, overwrite: true);
            }

            catch (Exception ex)
            {
                var result = ErrorHandler.Handler(ex, nameof(PageSizeFile));
                Debug.WriteLine(result.Error);
            }
        }

        public PageSize? Get(string pageKey) => _sizes.TryGetValue(pageKey, out var size) ? size : null;
    }


    public interface IPageSizeStore
    {
        void Save(string pageKey, double Width, double Height, bool IsMaximized);
        PageSize? Get(string pageKey);
    }
}