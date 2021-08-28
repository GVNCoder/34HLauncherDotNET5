using Microsoft.Win32;

namespace Launcher.Utilities
{
    public static class StandardFileDialogUtility
    {
        public static OpenFileDialog BuildSingleselectOpenFileDialog(string dialogDirectory, string dialogTitle, string filesFilter)
        {
            return new OpenFileDialog
            {
                InitialDirectory = dialogDirectory,
                Title = dialogTitle,
                Filter = filesFilter,
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                FileName = string.Empty
            };
        }
    }
}