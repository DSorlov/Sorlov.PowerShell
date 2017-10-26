using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Builder
{
    public class ComboListItem<T>
    {
        public string Name;
        public T Value;

        public ComboListItem(string name, T value)
        {
            Name = name;
            Value = value;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
