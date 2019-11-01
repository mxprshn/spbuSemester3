using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    public interface IQueryParser
    {
        IServerCommand ParseQuery(string source);
    }
}