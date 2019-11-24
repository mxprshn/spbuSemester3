using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    public class FileInformation
    {
        public string Name { get; }
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
