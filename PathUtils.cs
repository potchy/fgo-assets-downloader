namespace FgoAssetsDownloader
{
    public static class PathUtils
    {
        public static string SanitizeFileName(string fileName)
        {
            return string.Join("", fileName.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
