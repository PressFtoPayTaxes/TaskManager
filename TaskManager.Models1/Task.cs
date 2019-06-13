using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models1
{
    public class Task : Entity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Period Period { get; set; }
    }
}
