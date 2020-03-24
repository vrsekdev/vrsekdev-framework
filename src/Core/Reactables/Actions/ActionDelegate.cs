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
        public static Delegate CreateDelegateHelper(object target, RuntimeTypeHandle delegateTypeHandle, RuntimeMethodHandle runtimeMethodHandle)
        {
            Type type = Type.GetTypeFromHandle(delegateTypeHandle);
            MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(runtimeMethodHandle);

            return Delegate.CreateDelegate(type, target, methodInfo);
        }

        public static ActionInterceptorFactory GetFactoryForMethod(MethodInfo targetMethod)
        {
            MethodInfo enterWriteLockMethod = typeof(ReaderWriterLockSlim).GetMethod(nameof(ReaderWriterLockSlim.EnterWriteLock));
            MethodInfo exitWriteLockMethod = typeof(ReaderWriterLockSlim).GetMethod(nameof(ReaderWriterLockSlim.ExitWriteLock));

            MethodInfo createDelegateMethod = typeof(ActionDelegate).GetMethod(nameof(CreateDelegateHelper));

            Type[] parameterTypes = targetMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            Type baseMethodDelegateType = Expression.GetActionType(parameterTypes);

            Type[] interceptorParameterTypes = new[] { baseMethodDelegateType }.Concat(parameterTypes).ToArray();
            Type delegateType = Expression.GetActionType(interceptorParameterTypes);

            MethodInfo callBaseMethod = GetCallBaseMethod(parameterTypes);

            DynamicMethod dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), typeof(Delegate), new[] { typeof(ReaderWriterLockSlim), typeof(Action) });
            ILGenerator methodGenerator = dynamicMethod.GetILGenerator();
            methodGenerator.Emit(OpCodes.Ldarg_0);
            methodGenerator.Emit(OpCodes.Ldarg_1);
            methodGenerator.Emit(OpCodes.Newobj, typeof(ActionDelegateWrapper).GetConstructors()[0]);
            methodGenerator.Emit(OpCodes.Ldtoken, delegateType);
            methodGenerator.Emit(OpCodes.Ldtoken, callBaseMethod);
            methodGenerator.Emit(OpCodes.Call, createDelegateMethod);
            methodGenerator.Emit(OpCodes.Ret);

            return (ActionInterceptorFactory)dynamicMethod.CreateDelegate(typeof(ActionInterceptorFactory));
        }

        private static MethodInfo GetCallBaseMethod(Type[] parameterTypes)
        {
            var methods = typeof(ActionDelegateWrapper).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "Action");
            return methods.Single(x => x.GetGenericArguments().Length == parameterTypes.Length).GetGenericMethodDefinition().MakeGenericMethod(parameterTypes);
        }

        public class ActionDelegateWrapper
        {
            private readonly ReaderWriterLockSlim transactionLock;
            private readonly Action dequeue;

            public ActionDelegateWrapper(
                ReaderWriterLockSlim transactionLock,
                Action dequeue)
            {
                this.transactionLock = transactionLock;
                this.dequeue = dequeue;
            }

            private void ExecuteWithLock(Action action)
            {
                transactionLock.EnterWriteLock();
                try
                {
                    action();
                }
                finally
                {
                    transactionLock.ExitWriteLock();
                }

                dequeue();
            }

            public void Action(Action action)
            {
                ExecuteWithLock(action);
            }

            public void Action<T>(Action<T> action, T v1)
            {
                ExecuteWithLock(() => action(v1));
            }

            public void Action<T, T2>(Action<T, T2> action, T v1, T2 v2)
            {
                ExecuteWithLock(() => action(v1, v2));
            }

            public void Action<T, T2, T3>(Action<T, T2, T3> action, T v1, T2 v2, T3 v3)
            {
                ExecuteWithLock(() => action(v1, v2, v3));
            }

            public void Action<T, T2, T3, T4>(Action<T, T2, T3, T4> action, T v1, T2 v2, T3 v3, T4 v4)
            {
                ExecuteWithLock(() => action(v1, v2, v3, v4));
            }

            public void Action<T, T2, T3, T4, T5>(Action<T, T2, T3, T4, T5> action, T v1, T2 v2, T3 v3, T4 v4, T5 v5)
            {
                ExecuteWithLock(() => action(v1, v2, v3, v4, v5));
            }
        }
    }
}
