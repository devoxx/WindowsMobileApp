using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyDevoxx.Utils
{
    public static class FileUtils
    {
        public static async Task<bool> FileExists(string fileName)
        {
            try { StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileName)); }
            catch { return false; }
            return true;
        }
    }
}
