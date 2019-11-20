using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FTPServer
{
    public class FileQueryParser : IQueryParser
    {
        public FileServer Server { get; set; }

        public IServerCommand ParseQuery(string source, TcpClient client)
        {
            if (source == "$bye")
            {
                return new DisconnectCommand(Server, client);
            }

            var match = Regex.Match(source, "(?<code>\\d) (?<path>.+)");

            switch (int.Parse(match.Groups["code"].Value))
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

            return null;
        }
    }
}
