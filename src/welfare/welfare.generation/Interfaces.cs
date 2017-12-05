using welfare.core;

namespace welfare.generation
{
    public interface ICompiledWelfareService{
        IWelfareService GetNewInstance();
        void SaveToFile(string filename);
    }

    public interface IWelfareServiceBuilder
    {
        IWelfareServiceBuilder AddNamespace(string name);
        IWelfareServiceBuilder AddUsing(string name);
        IWelfareServiceBuilder AddWelfareRule(WelfareRule rule);
        ICompiledWelfareService Build();
    }
}