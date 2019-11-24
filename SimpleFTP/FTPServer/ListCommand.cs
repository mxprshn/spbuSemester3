using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPServer
{
    /// <summary>
    /// Lists content of a directory.
    /// </summary>
    public class ListCommand : IServerCommand
    {
        private string path;
        private TcpClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Directory to list.</param>
        /// <param name="client">Client to send response.</param>
        public ListCommand(string path, TcpClient client)
        {
            this.path = path;
            this.client = client;
        }

        /// <summary>
        /// Sends query with directory contents to client.
        /// </summary>
        public async Task Execute()
        {
            string responseString = "";

            try
            {
                var fileNames = Directory.GetFiles(path);
                var dirNames = Directory.GetDirectories(path);
                var fileCount = fileNames.Length + dirNames.Length;

                for (var i = 0; i < fileNames.Length; ++i)
                {
                    fileNames[i] += " false";
                }

                for (var i = 0; i < dirNames.Length; ++i)
                {
                    dirNames[i] += " true";
                }

                var filesString = $"{(fileNames.Length == 0 ? "" : " ")}{String.Join(" ", fileNames)}";
                var dirsString = $"{(dirNames.Length == 0 ? "" : " ")}{String.Join(" ", dirNames)}";
                responseString = $"{fileCount}{filesString}{dirsString}";

            }
            catch (DirectoryNotFoundException)
            {
                responseString = "-1";
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException
                    || e is ArgumentException || e is PathTooLongException)
            {
                responseString = "-2"; 
            }

            var writer = new StreamWriter(client.GetStream());
            await writer.WriteLineAsync(responseString);
            await writer.FlushAsync();
        }
    }
}