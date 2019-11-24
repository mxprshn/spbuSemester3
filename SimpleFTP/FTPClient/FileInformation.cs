
namespace FTPClient
{
    /// <summary>
    /// Class containing information about a file or directory.
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        /// Gets file name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Determines whether object is directory or file.
        /// </summary>
        public bool IsDirectory { get; }

        public FileInformation(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }

        public override bool Equals(object obj)
        {
            var item = obj as FileInformation;

            if (item == null)
            {
                return false;
            }

            return (Name == item.Name) && (IsDirectory == item.IsDirectory);
        }
    }
}