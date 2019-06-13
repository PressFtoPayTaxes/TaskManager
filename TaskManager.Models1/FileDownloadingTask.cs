using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models1
{
    public class FileDownloadingTask : Entity
    {
        public virtual Task Task { get; set; }
        public string FileUrl { get; set; }
    }
}
