namespace welfare.generation
{
    public class WelfareRule{
        public int Id { get; set; }
        public int Month { get; set; }
        public int IdMaritalStatus { get; set; }
        public int IdHandicap { get; set; }
        public int NumberOfChildren { get; set; }
        public int BrutoStart { get; set; }
        public int BrutoEnd { get; set; }
        public int Age { get; set; }
        public decimal Value { get; set; }
    }
}