using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SleddingGameModFramework.Utils
{
    /// <summary>
    /// Helper utilities for reflection operations
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly Dictionary<string, Type> _typeCache = new();
        
        /// <summary>
        /// Finds a type by its full name
        /// </summary>
        /// <param name="fullName">Full name of the type including namespace</param>
        /// <returns>The type, or null if not found</returns>
        public static Type FindType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;
            
            if (_typeCache.TryGetValue(fullName, out Type cachedType))
                return cachedType;
            
            // Search in all loaded assemblies
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .FirstOrDefault(t => t.FullName == fullName);
            
            if (type != null)
            {
                _typeCache[fullName] = type;
            }
            
            return type;
        }
        
        /// <summary>
        /// Finds a method in the specified type
        /// </summary>
        /// <param name="type">Type containing the method</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="parameterTypes">Optional array of parameter types</param>
        /// <param name="bindingFlags">Binding flags for the method search</param>
        /// <returns>The method info, or null if not found</returns>
        public static MethodInfo FindMethod(
            Type type, 
            string methodName, 
            Type[] parameterTypes = null, 
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            if (type == null || string.IsNullOrEmpty(methodName))
                return null;
            
            if (parameterTypes != null)
            {
                return type.GetMethod(methodName, bindingFlags, null, parameterTypes, null);
            }
            else
            {
                return type.GetMethod(methodName, bindingFlags);
            }
        }
        
        /// <summary>
        /// Gets the value of a field or property from an object
        /// </summary>
        /// <param name="obj">The object instance (or null for static members)</param>
        /// <param name="memberName">Name of the field or property</param>
        /// <param name="bindingFlags">Binding flags for the member search</param>
        /// <returns>The value, or null if not found</returns>
        public static object GetValue(
            object obj, 
            string memberName, 
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            if (string.IsNullOrEmpty(memberName))
                return null;
            
            Type type = obj?.GetType() ?? typeof(object);
            
            // Try as a field first
            FieldInfo field = type.GetField(memberName, bindingFlags);
            if (field != null)
                return field.GetValue(obj);
            
            // Then try as a property
            PropertyInfo property = type.GetProperty(memberName, bindingFlags);
            if (property != null)
                return property.GetValue(obj);
            
            return null;
        }
        
        /// <summary>
        /// Sets the value of a field or property on an object
        /// </summary>
        /// <param name="obj">The object instance (or null for static members)</param>
        /// <param name="memberName">Name of the field or property</param>
        /// <param name="value">Value to set</param>
        /// <param name="bindingFlags">Binding flags for the member search</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetValue(
            object obj, 
            string memberName, 
            object value, 
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            if (string.IsNullOrEmpty(memberName))
                return false;
            
            Type type = obj?.GetType() ?? typeof(object);
            
            // Try as a field first
            FieldInfo field = type.GetField(memberName, bindingFlags);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }
            
            // Then try as a property
            PropertyInfo property = type.GetProperty(memberName, bindingFlags);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Invokes a method on an object
        /// </summary>
        /// <param name="obj">The object instance (or null for static methods)</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="parameters">Method parameters</param>
        /// <param name="bindingFlags">Binding flags for the method search</param>
        /// <returns>The method return value, or null if an error occurred</returns>
        public static object InvokeMethod(
            object obj, 
            string methodName, 
            object[] parameters = null, 
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            if (string.IsNullOrEmpty(methodName))
                return null;
            
            Type type = obj?.GetType() ?? typeof(object);
            parameters ??= Array.Empty<object>();
            
            try
            {
                MethodInfo method = type.GetMethod(methodName, bindingFlags);
                if (method != null)
                {
                    return method.Invoke(obj, parameters);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to invoke method {methodName}: {ex.Message}");
            }
            
            return null;
        }
    }
}