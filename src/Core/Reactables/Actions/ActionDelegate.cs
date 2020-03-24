using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Havit.Blazor.Mobx.Reactables.Actions
{
    public delegate Delegate ActionInterceptorFactory(ReaderWriterLockSlim transactionLock, Action dequeue);

    public class ActionDelegate
    {
        public static Delegate CreateDelegateHelper(RuntimeTypeHandle delegateTypeHandle, RuntimeMethodHandle runtimeMethodHandle)
        {
            Type type = Type.GetTypeFromHandle(delegateTypeHandle);
            MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(runtimeMethodHandle);

            return Delegate.CreateDelegate(type, methodInfo);
        }

        public static ActionInterceptorFactory GetFactoryForMethod(MethodInfo targetMethod)
        {
            MethodInfo createDelegateMethod = typeof(ActionDelegate).GetMethod(nameof(CreateDelegateHelper));

            Type[] parameterTypes = targetMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            Type baseMethodDelegateType = Expression.GetActionType(parameterTypes);

            Type[] interceptorParameterTypes = new[] { baseMethodDelegateType }.Concat(parameterTypes).ToArray();
            Type delegateType = Expression.GetActionType(interceptorParameterTypes);

            MethodInfo callBaseDelegate = GetDelegateMethod(parameterTypes);

            DynamicMethod dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), typeof(Delegate), new[] { typeof(ReaderWriterLockSlim), typeof(Action) });
            ILGenerator methodGenerator = dynamicMethod.GetILGenerator();
            //methodGenerator.
            methodGenerator.Emit(OpCodes.Ldtoken, delegateType);
            methodGenerator.Emit(OpCodes.Ldtoken, callBaseDelegate);
            methodGenerator.Emit(OpCodes.Call, createDelegateMethod);
            methodGenerator.Emit(OpCodes.Ret);

            return (ActionInterceptorFactory)dynamicMethod.CreateDelegate(typeof(ActionInterceptorFactory));
        }

        private static MethodInfo GetDelegateMethod(Type[] parameterTypes)
        {
            var methods = typeof(ActionDelegate).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == "Action");
            return methods.Single(x => x.GetGenericArguments().Length == parameterTypes.Length).GetGenericMethodDefinition().MakeGenericMethod(parameterTypes);
        }

        public static void Action(Action action)
        {
            action();
        }

        public static void Action<T>(Action<T> action, T v1)
        {
            action(v1);
        }

        public static void Action<T, T2>(Action<T, T2> action, T v1, T2 v2)
        {
            action(v1, v2);
        }

        public static void Action<T, T2, T3>(Action<T, T2, T3> action, T v1, T2 v2, T3 v3)
        {
            action(v1, v2, v3);
        }

        public static void Action<T, T2, T3, T4>(Action<T, T2, T3, T4> action, T v1, T2 v2, T3 v3, T4 v4)
        {
            action(v1, v2, v3, v4);
        }

        public static void Action<T, T2, T3, T4, T5>(Action<T, T2, T3, T4, T5> action, T v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            action(v1, v2, v3, v4, v5);
        }
    }
}
