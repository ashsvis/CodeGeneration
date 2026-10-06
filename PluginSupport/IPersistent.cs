
using System.Xml.Linq;

namespace PluginSupport
{
    public interface IPersistent<T>
    {
        XElement WriteContent();
        void ReadContent(XElement element);
        bool NoDataToWrite();
    }
}
