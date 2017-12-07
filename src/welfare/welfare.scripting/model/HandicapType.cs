using System;
using System.Collections.Generic;

namespace welfare.scripting.model
{
    public partial class HandicapType
    {
        public HandicapType()
        {
            WelfareLogic = new HashSet<WelfareLogic>();
        }

        public int Id { get; set; }
        public int HandicapPct { get; set; }

        public ICollection<WelfareLogic> WelfareLogic { get; set; }
    }
}
