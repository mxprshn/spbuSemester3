using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    public class Client
    {
        private int port = 0;

        public async Task<IList<FileInformation>> ListFiles()
        {
            return null;
        }

        public async Task<byte[]> GetFile()
        {
            return null;
        }
    }
}
