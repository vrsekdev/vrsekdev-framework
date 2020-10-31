using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VrsekDevBlazor.BlazorCommunicationFoundation
{
    [Generator]
    public class ClientImplementationSourceGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            throw new NotImplementedException();
        }

        public void Execute(SourceGeneratorContext context)
        {
            //context.Compilation..SyntaxTrees.Where(x => x.)

            Console.WriteLine(Assembly.GetCallingAssembly().FullName);
            //throw new NotImplementedException();
        }
    }
}
