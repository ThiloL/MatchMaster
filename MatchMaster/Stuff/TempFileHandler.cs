using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    public class TFH : IDisposable
    {
        private List<string> temporaryFiles;

        public TFH()
        {
            temporaryFiles = new List<string>();
        }

        public string get(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) throw new Exception("'extension' missing");

            string t = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + "." + extension;
            temporaryFiles.Add(t);

            return t;
        }

        public void Dispose()
        {
            if (temporaryFiles.Count > 0)
                foreach (var t in temporaryFiles)
                {
                    try
                    {
                        File.Delete(t);
                    }
                    catch { }
                }
        }
    }
}
