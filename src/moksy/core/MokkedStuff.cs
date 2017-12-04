using System;
using System.Collections.Generic;
using System.Text;

namespace moksy.core
{
    public class MokkedActions : Dictionary<string, Action>
    {

    }

    public class MokkedActionsOfT<T> : Dictionary<string, Action<T>>
    {
    }

    public class MokkedActionsOfT<T1, T2> : Dictionary<string, Action<T1, T2>>
    {
    }

    public class MokkedFuncs<T> : Dictionary<string, Func<T>>
    {

    }

    public class MokkedFuncsWithReturn<T, R> : Dictionary<string, Func<T,R>>
    {

    }
}
