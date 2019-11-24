using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FTPClient
{
    /// <summary>
    /// Class implementing SimpleFTP network client.
    /// </summary>
    public class FileClient : IDisposable
    {
        private IClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">IClient implementation used to send and receive queries.</param>
        public FileClient(IClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Gets content of a specific directory on server as a list of FileInformation objects.
        /// </summary>
        /// <param name="dirPath">Path to directory to list.</param>
        /// <returns>List of FileInformation objects for the directory.</returns>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="ArgumentException"/>
        public async Task<IList<FileInformation>> List(string dirPath)
        {
            await client.Send(BuildListQuery(dirPath));
            var response = await client.Receive();
            var responseString = Encoding.UTF8.GetString(response);

            var match = Regex.Match(responseString, "-?\\d+");

            switch (match.Value)
            {
                case "-1":
                    {
                        throw new DirectoryNotFoundException("Directory on server not found.");
                    }
                case "-2":
                    {
                        throw new ArgumentException("Invalid directory name or access denied.");
                    }
            }

            return ParseListResponse(responseString);
        }

        /// <summary>
        /// Downloads a file from server.
        /// </summary>
        /// <param name="sourcePath">Path to file on server.</param>
        /// <param name="targetPath">Downloaded file path.</param>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="ArgumentException"/>
        public async Task Get(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                throw new ArgumentException("Target file already exists.");
            }

            try
            {
                Path.GetFullPath(Path.GetDirectoryName(targetPath));
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid target file path.");
            }

            await client.Send(BuildGetQuery(sourcePath));
            var response = await client.Receive();
            var responseString = Encoding.UTF8.GetString(response);

            var match = Regex.Match(responseString, "-?\\d+ ?");

            switch (match.Value)
            {
                case "-1":
                    {
                        throw new FileNotFoundException("File on server not found.");
                    }
                case "-2":
                    {
                        throw new ArgumentException("Invalid file name or access denied.");
                    }
            }

            var header = Encoding.UTF8.GetBytes(match.Value);

            var content = new byte[response.Length - header.Length];

            Array.Copy(response, header.Length, content, 0, content.Length);

            using (var fileStream = new FileStream(targetPath, FileMode.Create))
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
        private string BuildDisconnectQuery() => "$bye";

        public async void Dispose()
        {
            if (client.IsConnected)
            {
                await client.Send(BuildDisconnectQuery());
            }
            
            client.Dispose();
        }
    }
}