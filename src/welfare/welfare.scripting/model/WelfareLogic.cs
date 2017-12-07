using System;
using System.Collections.Generic;

namespace welfare.scripting.model
{
    public partial class WelfareLogic
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int IdMaritalStatus { get; set; }
        public int IdHandicap { get; set; }
        public int NumberOfChildren { get; set; }
        public int BrutoStart { get; set; }
        public int BrutoEnd { get; set; }
        public int Age { get; set; }
        public decimal Value { get; set; }

        public HandicapType IdHandicapNavigation { get; set; }
        public MaritalStatus IdMaritalStatusNavigation { get; set; }
    }
}
