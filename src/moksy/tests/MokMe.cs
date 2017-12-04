using System.Diagnostics;

namespace tests
{
    public class MokMe
    {
        public bool WasBoolCalled
        {
            get;
            set;
        }

        public virtual void DoStuff()
        {
            WasBoolCalled = true;
            Debug.WriteLine("Please mok me");
        }

        public virtual void DoStuff(int i)
        {
            WasBoolCalled = true;
            Debug.WriteLine("Please mok me");
        }

        public virtual void DoStuff(int i, string a)
        {
            WasBoolCalled = true;
            Debug.WriteLine("Please mok me");
        }
    }
}