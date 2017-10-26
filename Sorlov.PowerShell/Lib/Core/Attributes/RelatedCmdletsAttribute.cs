using System;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RelatedCmdletsAttribute : Attribute
    {
        public List<Type> RelatedCmdlets { get; set; }

        public string[] ExternalCmdlets { get; set; }

        public RelatedCmdletsAttribute(params Type[] type)
        {
            List<Type> cmdlets = new List<Type>();
            foreach (Type t in type)
            {
                cmdlets.Add(t);
            }
            RelatedCmdlets = cmdlets;
        }
    }
}
