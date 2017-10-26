using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Automation
{
    public class DynamicTypeManager
    {
        private TypeBuilder tb;


        public DynamicTypeManager(string assemblyName, string typeName)
        {
            tb = GetTypeBuilder(assemblyName, typeName);
        }

        public object Create()
        {
            Type t = tb.CreateType();
            return Activator.CreateInstance(t);
        }

        private TypeBuilder GetTypeBuilder(string assemblyName, string typeName)
        {
            AssemblyName an = new AssemblyName("Dynamic."+assemblyName);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            TypeBuilder tb = moduleBuilder.DefineType(typeName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(object));
            return tb;
        }

        public FieldBuilder CreateProperty(string propertyName, Type propertyType, bool toString = false)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getPropertyBuiler = CreatePropertyGetter(tb, fieldBuilder, toString);
            MethodBuilder setPropertyBuiler = CreatePropertySetter(tb, fieldBuilder, toString);

            propertyBuilder.SetGetMethod(getPropertyBuiler);
            propertyBuilder.SetSetMethod(setPropertyBuiler);
            if (toString) SetToStringField(fieldBuilder);

            return fieldBuilder;
        }

        public void SetToStringField(FieldBuilder fieldBuilder)
        {

            MethodBuilder getMethodBuilder =
                tb.DefineMethod("ToString",
                    MethodAttributes.Public,
                    fieldBuilder.FieldType, Type.EmptyTypes);

            ILGenerator getIL = getMethodBuilder.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
        }

        private MethodBuilder CreatePropertyGetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, bool nonPublic = false)
        {
            MethodBuilder getMethodBuilder =
                typeBuilder.DefineMethod("get_" + fieldBuilder.Name,
                    nonPublic ? MethodAttributes.Private : MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    fieldBuilder.FieldType, Type.EmptyTypes);

            ILGenerator getIL = getMethodBuilder.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }


        private MethodBuilder CreatePropertySetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, bool nonPublic = false)
        {
            MethodBuilder setMethodBuilder =
                typeBuilder.DefineMethod("set_" + fieldBuilder.Name,
                  nonPublic ? MethodAttributes.Private : MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new Type[] { fieldBuilder.FieldType });

            ILGenerator setIL = setMethodBuilder.GetILGenerator();

            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        public static void SetProperty(object Target, string Name, object value, bool nonPublic = false)
        {

            PropertyInfo targetProperty = Target.GetType().GetProperty(Name, nonPublic ? BindingFlags.NonPublic : BindingFlags.Public);

            if (targetProperty == null)
            {
                throw new Exception("Object " + Target.ToString() + "   does not have Target Property " + Name);

            }

            targetProperty.GetSetMethod(nonPublic).Invoke(Target, new object[] { value });
        }


        public static object GetProperty(object Target, string Name, bool throwError, bool nonPublic = false)
        {

            PropertyInfo targetProperty = Target.GetType().GetProperty(Name, nonPublic ? BindingFlags.NonPublic : BindingFlags.Public);

            if (targetProperty == null)
            {
                if (throwError)
                {
                    throw new Exception("Object " + Target.ToString() + "   does not have Target Property " + Name);
                }
                else
                {
                    return null;
                }
            }
            else
            {

                return targetProperty.GetGetMethod(nonPublic).Invoke(Target, null);
            }

        }

    }
}
