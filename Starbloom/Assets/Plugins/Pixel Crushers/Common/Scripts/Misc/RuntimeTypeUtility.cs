// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Utility methods to work with types.
    /// </summary>
    public static class RuntimeTypeUtility
    {

        /// <summary>
        /// Searches all assemblies for a type with a specified name.
        /// </summary>
        /// <param name="typeName">Fully-qualified type name.</param>
        /// <returns>A type, or null if none matches.</returns>
        public static System.Type GetTypeFromName(string typeName)
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var type = assembly.GetType(typeName);
                if (type != null) return type;
            }
            return null;
        }

    }
}
