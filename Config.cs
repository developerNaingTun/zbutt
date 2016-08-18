using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace zbutt
{
    class Config
    {
        private RegistryKey reg;
        public Config(string path)
        {
            reg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Technomation\\" + path);
        }

        public string Read(string key)
        {
            return reg.GetValue(key).ToString();
        }

        public string Read(string key, string def)
        {
            return reg.GetValue(key,def).ToString();
        }

        public void Write(string key, string value)
        {
            reg.SetValue(key, value);
        }
    }
}
