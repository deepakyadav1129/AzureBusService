using System.Diagnostics.Eventing.Reader;

namespace EMS.Helders
{
    public static class FileValidationHelper
    {
        public static bool IsValidExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return EMS.Constants.FileConstant.AllowedExtensions.Contains(extension);
        }

        public static bool IsValidFileSize(long fileSize)
        {
            return fileSize <= EMS.Constants.FileConstant.MazFileSize;
        }

    }
}
