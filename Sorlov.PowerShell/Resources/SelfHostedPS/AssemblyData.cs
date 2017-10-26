using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.SelfHostedPS
{
    public class AssemblyData
    {
        public AssemblyData(System.Reflection.Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            this.assembly = assembly;
        }

        private readonly System.Reflection.Assembly assembly;

        /// <summary>
        /// Gets the title property
        /// </summary>
        public string ProductTitle
        {
            get
            {
                return GetAttributeValue<System.Reflection.AssemblyTitleAttribute>(a => a.Title,
                       System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase));
            }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version
        {
            get
            {
                string result = string.Empty;
                Version version = assembly.GetName().Version;
                if (version != null)
                    return version.ToString();
                else
                    return "1.0.0.0";
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<System.Reflection.AssemblyDescriptionAttribute>(a => a.Description); }
        }


        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return GetAttributeValue<System.Reflection.AssemblyProductAttribute>(a => a.Product); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return GetAttributeValue<System.Reflection.AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company
        {
            get { return GetAttributeValue<System.Reflection.AssemblyCompanyAttribute>(a => a.Company); }
        }

        protected string GetAttributeValue<TAttr>(Func<TAttr,
          string> resolveFunc, string defaultResult = null) where TAttr : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
                return resolveFunc((TAttr)attributes[0]);
            else
                return defaultResult;
        }
    }

}
