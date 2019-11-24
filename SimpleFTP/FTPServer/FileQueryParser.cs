using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace FTPServer
{
    /// <summary>
    /// Class implementing protocol query parser.
    /// </summary>
    public class FileQueryParser : IQueryParser
    {
        /// <summary>
        /// Server the parser is associated with.
        /// </summary>
        public Server Server { get; set; }

        /// <summary>
        /// Returns an appropriate command by query.
        /// </summary>
        /// <param name="source">Query string.</param>
        /// <param name="client">Client returned command is associated with.</param>
        /// <returns>An appropriate command.</returns>
        public IServerCommand ParseQuery(string source, TcpClient client)
        {
            if (source == "$bye")
            {
                return new DisconnectCommand(Server, client);
            }

            var match = Regex.Match(source, "(?<code>\\d) (?<path>.+)");

            if (int.TryParse(match.Groups["code"].Value, out var result))
            {
                switch (result)
                {
                    case 1:
                        {
                            return new ListCommand(match.Groups["path"].Value, client);
                        }
                    case 2:
                        {
                            return new GetCommand(match.Groups["path"].Value, client);
                        }
                }
            }

            return null;
        }
    }
}