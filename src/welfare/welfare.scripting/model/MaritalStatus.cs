using System;
using System.Collections.Generic;

namespace welfare.scripting.model
{
    public partial class MaritalStatus
    {
        public MaritalStatus()
        {
            WelfareLogic = new HashSet<WelfareLogic>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<WelfareLogic> WelfareLogic { get; set; }
    }
}
