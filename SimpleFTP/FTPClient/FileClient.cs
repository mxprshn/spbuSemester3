using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FTPClient
{
    public class FileClient
    {
        private IClient client;

        public FileClient(IClient client)
        {
            this.client = client;
        }

        public async Task ListFiles(string path)
        {
            await client.Send(BuildListQuery(path));
            Console.WriteLine(await client.Receive());
        }

        public async Task GetFile(string path)
        {
            await client.Send(BuildGetQuery(path));
            Console.WriteLine(await client.Receive());
        }

        //private IList<FileInformation> ParseListResponse(string response)
        //{
        //    var match = Regex.Match(response, "(?<size>\\d)(?<path> .+ (?<isDir>false|true))+");
        //}

        ////public async Task GetFile()
        ////{
        ////    return null;
        ////}

        private string BuildListQuery(string path) => $"1 {path}";
        private string BuildGetQuery(string path) => $"2 {path}";
    }
}
