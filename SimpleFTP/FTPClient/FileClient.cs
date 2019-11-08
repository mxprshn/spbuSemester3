using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        public async Task List(string path)
        {
            await client.Send(BuildListQuery(path));
            var fileInfo = ParseListResponse(Encoding.UTF8.GetString(await client.Receive()));

            foreach (var current in fileInfo)
            {
                Console.WriteLine($"{current.Name} {current.IsDirectory}");
            }
        }

        public async Task Get(string path)
        {
            await client.Send(BuildGetQuery(path));
            var response = await client.Receive();
            //Console.WriteLine(await client.Receive());

            using (var fileStream = new FileStream($"C:\\Users\\mxprshn\\Downloads\\olollo.jpg", FileMode.OpenOrCreate))
            {
                await fileStream.WriteAsync(response);
            }
        }

        private IList<FileInformation> ParseListResponse(string response)
        {
            var matches = Regex.Matches(response, " (?<name>.*?) (?<isDir>false|true)");
            var result = new List<FileInformation>();

            foreach (Match match in matches)
            {
                result.Add(new FileInformation(match.Groups["name"].Value, bool.Parse(match.Groups["isDir"].Value)));
            }

            return result;
        }

        //public async Task GetFile()
        //{
        //    return null;
        //}

        private string BuildListQuery(string path) => $"1 {path}";
        private string BuildGetQuery(string path) => $"2 {path}";
    }
}
