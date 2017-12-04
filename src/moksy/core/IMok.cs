using System;
using System.Collections.Generic;
using System.Text;

namespace moksy.core
{
    public interface IMok
    {
        IMok AddMokkedStuff(string name, Action stuff);
        IMok AddMokkedStuff<T1>(string name, Action<T1> stuff);
        IMok AddMokkedStuff<T1, T2>(string name, Action<T1, T2> stuff);
        IMok AddMokkedStuff<T1, T2, T3>(string name, Action<T1, T2, T3> stuff);
        IMok AddMokkedStuff<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> stuff);
        IMok AddMokkedStuff<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> stuff);

        IMok AddMokkedStuff<TResult>(string name, Func<TResult> stuff);
        IMok AddMokkedStuff<T1, TResult>(string name, Func<T1, TResult> stuff);
        IMok AddMokkedStuff<T1, T2, TResult>(string name, Func<T1, T2, TResult> stuff);
        IMok AddMokkedStuff<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> stuff);
        IMok AddMokkedStuff<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> stuff);
        IMok AddMokkedStuff<T1, T2, T3, T4, T5, TResult>(string name, Func<T1, T2, T3, T4, T5, TResult> stuff);

        IMok SetCallBase(bool callBase);
        MokkedActions MokkedActions { get; }
        MokkedActionsOfT<object> MokkedActionsOfT { get; }
        MokkedActionsOfT<object, object> MokkedActionsOfT2 { get; }
    }
}
