using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginSupport
{
    public class PluginManager
    {
        public List<IPlugin> Plugins = [];

        public void ScanPlugins(string directory)
        {
            if (Directory.Exists(directory))
            {
                //перебирвем все файлы dll
                foreach (var file in Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories))
                    try
                    {
                        //загружаем ассемблю
                        var assmb = Assembly.LoadFile(file);
                        //перебираем все типы из ассембли
                        foreach (var type in assmb.GetTypes())
                        {
                            //проверяем наличие интерфейса IPlugin
                            var iface = type.GetInterface("IPlugin");
                            if (iface != null && !string.IsNullOrWhiteSpace(type.FullName))
                            {
                                //создаем экземпляр плагина
                                if (assmb.CreateInstance(type.FullName) is IPlugin plugin)
                                    Plugins.Add(plugin);
                            }
                        }
                    }
                    catch {/*is not .NET assembly*/}
            }
        }
    }
}
