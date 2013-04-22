using System;

namespace ITJakub.Core.Searching.CriteriumCompilers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CriteriumCompilerAttribute:Attribute
    {
        public Type CompilerFor { get; private set; }

        public CriteriumCompilerAttribute(Type compilerFor)
        {
            CompilerFor = compilerFor;
        }
    }
}