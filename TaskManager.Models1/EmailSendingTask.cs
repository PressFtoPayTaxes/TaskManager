using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models1
{
    public class EmailSendingTask : Entity
    {
        public virtual Task Task { get; set; }
        public string MailReceiver { get; set; }
        public string MailTheme { get; set; }
        public string MailText { get; set; }
    }
}
