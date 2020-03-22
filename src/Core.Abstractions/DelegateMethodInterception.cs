using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class DelegateMethodInterception : MethodInterception
    {
		private readonly ClassMethodInterception interception;

		public DelegateMethodInterception()
		{
			interception = new ClassMethodInterception();
		}

		public MethodInfo InterceptedMethod 
		{ 
			get
			{
				return interception.InterceptedMethod;
			}
			set
			{
				interception.InterceptedMethod = value;
			}
		}

		public MulticastDelegate Delegate
		{
			set 
			{
				interception.InterceptorMethod = value.GetType().GetMethod("Invoke") ?? throw new NotSupportedException();
				interception.InterceptorTarget = value;
			}
		}

		public bool ProvideInterceptedTarget
		{
			get
			{
				return interception.ProvideInterceptedTarget;
			}
			set
			{
				interception.ProvideInterceptedTarget = value;
			}
		}

		public override MethodInfo GetInterceptedMethod()
		{
			return interception.GetInterceptedMethod();
		}

		public override MethodInfo GetInterceptorMethod()
		{
			return interception.GetInterceptorMethod();
		}

		public override object GetInterceptorTarget()
		{
			return interception.GetInterceptorTarget();
		}

		public override bool ShouldProvideInterceptedTarget()
		{
			return interception.ShouldProvideInterceptedTarget();
		}
	}
}
