using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.events
{
    public partial class Event
    {
        public bool cancelled = false;

        public void cancel() { 
            cancelled = true;
        }
    }
}
