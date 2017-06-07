using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGameOfLife.Models
{
    public class Counter
    {
        public int count { get; set; }

        public void increment()
        {
            this.count++;
        }
    }
}