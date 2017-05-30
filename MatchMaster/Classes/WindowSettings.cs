using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;

namespace MatchMaster
{
    [Serializable]
    public class WindowSetting
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public WindowState State;

        public string GetXml()
        {
            XmlSerializer x = new XmlSerializer(typeof(WindowSetting));

            using (StringWriter sw = new StringWriter())
            {
                x.Serialize(sw, this);
                return sw.ToString();
            }
        }

        public void SetXml(string xml)
        {
            XmlSerializer x = new XmlSerializer(typeof(WindowSetting));

            using (StringReader sr = new StringReader(xml))
            {
                WindowSetting w = (WindowSetting)x.Deserialize(sr);
                Top = w.Top;
                Left = w.Left;
                Width = w.Width;
                Height = w.Height;
                State = w.State;
            }
        }
    }
}
