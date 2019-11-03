using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FTPServer
{
    public class FileQueryParser : IQueryParser
    {
        public IServerCommand ParseQuery(string data)
        {
            var match = Regex.Match(data, "(?<code>\\d) (?<path>.+)");

            switch (int.Parse(match.Groups["code"].Value))
            {
                case 1:
                    {
                        return new ListCommand(match.Groups["path"].Value);
                    }
                case 2:
                    {
                        return new GetCommand(match.Groups["path"].Value);
                    }
            }

            return null;
        }
    }
}
