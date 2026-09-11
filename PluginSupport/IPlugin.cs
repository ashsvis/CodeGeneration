using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSupport
{
    public interface IPlugin
    {
        string Name { get; }
        void Run(IHost host);
    }
}
