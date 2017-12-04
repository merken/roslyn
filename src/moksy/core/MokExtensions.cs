using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace moksy.core
{
    public static class MokExtensions
    {
        public static M CallBase<M>(this M mok, bool callBase) where M : class
        {
            var iMok = mok as IMok;

            if (iMok == null)
                throw new NotSupportedException("The provided object is not an IMok");

            iMok.SetCallBase(callBase);

            return iMok as M;
        }

        public static M MokSomething<M>(this M mok, Expression<Action<M>> expression, Action acts) where M : class
        {
            var iMok = mok as IMok;

            if (iMok == null)
                throw new NotSupportedException("The provided object is not an IMok");

            var methodName = GetMethodName(expression);
            iMok.AddMokkedStuff(methodName, acts);

            return iMok as M;
        }

        public static M MokSomething<T1, M>(this M mok, Expression<Action<M>> expression, Action<T1> acts) where M : class
        {
            var iMok = mok as IMok;

            if (iMok == null)
                throw new NotSupportedException("The provided object is not an IMok");

            var methodName = GetMethodName(expression);
            iMok.AddMokkedStuff<T1>(methodName, acts);

            return iMok as M;
        }

        public static M MokSomething<T1, T2, M>(this M mok, Expression<Action<M>> expression, Action<T1, T2> acts) where M : class
        {
            var iMok = mok as IMok;

            if (iMok == null)
                throw new NotSupportedException("The provided object is not an IMok");

            var methodName = GetMethodName(expression);
            iMok.AddMokkedStuff(methodName, acts);

            return iMok as M;
        }

        //public static T MokSomething<T>(this T mok, Expression<Action<T>> expression, Action<T> acts) where T : class
        //{
        //    var iMok = mok as IMok;

        //    if (iMok == null)
        //        throw new NotSupportedException("The provided object is not an IMok");

        //    var methodName = GetMethodName(expression);
        //    iMok.AddMokkedStuff(methodName, acts);

        //    return iMok as T;
        //}

        public static T MokSomething<T, TResult>(this T mok, Expression<Func<T, TResult>> expression, Func<TResult> returns) where T : class
        {
            var iMok = mok as IMok;

            if (iMok == null)
                throw new NotSupportedException("The provided object is not an IMok");

            var methodName = GetMethodName(expression);
            iMok.AddMokkedStuff(methodName, returns as Func<object>);

            return iMok as T;
        }

        //public static T MokSomething<T, TResult>(this T mok, Expression<Func<T, TResult>> expression, Func<T, TResult> returns) where T : class
        //{
        //    var iMok = mok as IMok;

        //    if (iMok == null)
        //        throw new NotSupportedException("The provided object is not an IMok");

        //    var methodName = GetMethodName(expression);
        //    iMok.AddMokkedStuff(methodName, returns);

        //    return iMok as T;
        //}

        //public static T MokSomething<T, TResult>(this T mok, Expression<Func<T, TResult>> expression, Action<TResult> returns) where T : class
        //{
        //    var iMok = mok as IMok;

        //    if(iMok == null)
        //        throw new NotSupportedException("The provided object is not an IMok");

        //    var methodName = GetMethodName(expression);
        //    iMok.AddMokkedStuff(methodName, returns);

        //    return iMok as T;
        //}

        public static string GetMethodName(LambdaExpression expression)
        {
            var methodCall = expression.Body as MethodCallExpression;

            if (methodCall == null)
                throw new NotSupportedException($"The provided method call was not supported {expression.ToString()}");

            var method = methodCall.Method;
            var methodName = GetFullMethodName(method);

            return methodName;
        }

        public static string GetFullMethodName(MethodInfo method)
        {
            var methodName = method.Name;
            var parameters = method.GetParameters();

            var parameterBuilder = new StringBuilder();
            foreach (var p in parameters)
                parameterBuilder.Append($":{p}");

            return $"{methodName}{parameterBuilder}";
        }
    }
}
