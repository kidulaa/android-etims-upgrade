using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EBM2x.Dependency
{
    public interface IDownloadProcessor
    {
        void DownloadFromUrl(string url);
    }
}
