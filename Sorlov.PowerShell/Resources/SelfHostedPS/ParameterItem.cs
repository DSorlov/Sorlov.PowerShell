using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.SelfHostedPS
{
    public class ParameterItem
    {
        private string name;
        private string helpText;
        private bool mandatory;
        private string type;

        public ParameterItem(string name, string helpText, bool mandatory, string type)
        {
            this.name = name;
            this.helpText = helpText;
            this.mandatory = mandatory;
            this.type = type;
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }


        public bool Mandatory
        {
            get { return mandatory; }
            set { mandatory = value; }
        }

        public string HelpText
        {
            get { return helpText; }
            set { helpText = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
