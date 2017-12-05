namespace welfare.core
{
    public interface IWelfareService
    {
         decimal GetWelfare(int monthsOfUnemployment, int maritalStatus, int handicap, int numberOfChildren, decimal brutowage, int age);
    }
}