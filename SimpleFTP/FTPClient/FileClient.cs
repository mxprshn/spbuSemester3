using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FTPClient
{
    public class FileClient : IDisposable
    {
        private IClient client;

        public FileClient(IClient client)
        {
            this.client = client;
        }

        public async Task<IList<FileInformation>> List(string dirPath)
        {
            await client.Send(BuildListQuery(dirPath));
            return ParseListResponse(Encoding.UTF8.GetString(await client.Receive()));
        }

        public async Task Get(string sourcePath, string targetPath)
        {
            // проверить пути на корректность

            await client.Send(BuildGetQuery(sourcePath));
            var response = await client.Receive();
            var responseString = Encoding.UTF8.GetString(response);
            var match = Regex.Match(responseString, "\\d+ ");
            var header = Encoding.UTF8.GetBytes(match.Value);

            var content = new byte[response.Length - header.Length];

            Array.Copy(response, header.Length, content, 0, content.Length);

            using (var fileStream = new FileStream(targetPath, FileMode.OpenOrCreate))
            {
                await fileStream.WriteAsync(content);
            }
        }

        private IList<FileInformation> ParseListResponse(string response)
        {
            var result = new List<FileInformation>();

            var matches = Regex.Matches(response, " (?<name>.*?) (?<isDir>false|true)");

            foreach (Match match in matches)
            {
                result.Add(new FileInformation(match.Groups["name"].Value, bool.Parse(match.Groups["isDir"].Value)));
            }

            return result;
        }

        private string BuildListQuery(string path) => $"1 {path}";
        private string BuildGetQuery(string path) => $"2 {path}";

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
