using System;
using System.Reflection;
using System.Reflection.Emit;
using NSEipix.Base;
using NSEipix.Repository;

namespace GoingMedievalModLauncher.Engine
{
	public static class DynamicRepositoryBuilder<T, M> 
        where T : JsonRepository<T, M>
        where M : Model
	{
        
        public static Type CompileResultType(string className)
        {
            TypeBuilder tb = GetTypeBuilder(className);
            Type objectType = tb.CreateType();
            return objectType;
        }

        public static Type CompileResultType(TypeBuilder tb)
        {
            Type objectType = tb.CreateType();
            return objectType;
        }

        public static TypeBuilder GetTypeBuilder(string className)
        {
            var typeSignature = className;
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    typeof(T));
            return tb;
        }


        public static void OverrideDeserialize(TypeBuilder tb)
        {
            if ( tb == null || tb.BaseType == null )
            {
                Logger.Instance.info("Type Builder was not specified or was imvalid!");
                return;
            }
            
            Logger.Instance.info(typeof(T).Name);
            var orig = tb.BaseType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            if ( orig == null )
            {
                Logger.Instance.info("The original method was null! Trying with public member.");
                orig = tb.BaseType.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Instance);
            }

            if ( orig == null )
            {
                Logger.Instance.info("The original method is still null! Nothing we can do.");
                return;
            }
            var ev = typeof(RepositoryPatch<T, M>).GetMethod(
                "CallEvent", BindingFlags.Public | BindingFlags.Static);
            if ( ev == null )
            {
                Logger.Instance.info("The event caller was null! How?");
                return;
            }

            var attr = MethodAttributes.Family;
            attr |= MethodAttributes.HideBySig | MethodAttributes.ReuseSlot | MethodAttributes.Virtual;
            
            var overriden = tb.DefineMethod("Deserialize",  attr);

            var ilGen= overriden.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.EmitCall(OpCodes.Call, orig, null);
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, ev);
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Ret);

        }

    }
}