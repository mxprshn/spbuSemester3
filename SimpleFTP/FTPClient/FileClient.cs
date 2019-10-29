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
        private TcpClient client;

        public FileClient(int port, string hostname = "localhost")
        {
            client = new TcpClient(hostname, port);
        }

        public async Task ListFiles(string path)
        {
            var stream = client.GetStream();
            // using?
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(BuildListQuery(path));
            writer.Flush();

            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            Console.WriteLine(data);
        }

        public async Task GetFile(string path)
        {
            var stream = client.GetStream();
            // using?
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(BuildGetQuery(path));
            writer.Flush();

            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            Console.WriteLine(data);
        }

        //private IList<FileInformation> ParseListResponse(string response)
        //{
        //    var match = Regex.Match(response, "(?<size>\\d)(?<path> .+ (?<isDir>false|true))+");
        //}
        //public void GetFile(string path)

        ////public async Task GetFile()
        ////{
        ////    return null;
        ////}

        private string BuildListQuery(string path) => $"1 {path}";
        private string BuildGetQuery(string path) => $"2 {path}";
    }
}
