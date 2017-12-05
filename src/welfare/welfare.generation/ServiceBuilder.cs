namespace welfare.generation
{
    public static class ServiceBuilder
    {
        public static IWelfareServiceBuilder CreateWelfareServiceBuilder(){
            return new WelfareServiceBuilder();
        }
    }
}