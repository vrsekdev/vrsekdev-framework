using Havit.Blazor.Mobx.Tests;
using System;

namespace TestProfiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new StoreAccessorTests();
            tests.TestInitialize();

            for (int i =0; i < 1; i++)
            {
                tests.Store_Action_BatchMutations();
            }
        }
    }
}
