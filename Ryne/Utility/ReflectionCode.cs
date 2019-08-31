using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ryne.Entities;

namespace Ryne.Utility
{
    /// <summary>
    /// Class that contains code that is powerful but is expected run quite slowly and shouldn't be used often
    /// </summary>
    static class ReflectionCode
    {
        private static List<Type> EntityTypesStorage;

        public static List<Type> EntityTypes => EntityTypesStorage ?? (EntityTypesStorage = GetEditableEntities().ToList());

        public static IEnumerable<Type> GetClassesWithBaseClass<T>()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsSubclassOf(typeof(T))))
            {
                yield return type;
            }
        }

        // TODO: get entities from all assemblies?
        public static IEnumerable<Type> GetEditableEntities()
        {
            var entityTypes = GetClassesWithBaseClass<Entity>().ToList();
            foreach (var entityType in entityTypes)
            {
                var e = (Entity)Activator.CreateInstance(entityType);
                if (!e.ContainsFlag(EntityFlag.EditorNotEditable))
                {
                    yield return entityType;
                }
            }
        }
    }
}
