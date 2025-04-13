using System;

namespace SleddingGameModFramework.Patches
{
    /// <summary>
    /// Attribute for marking methods that should be patched
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PatchAttribute : Attribute
    {
        /// <summary>
        /// Full name of the class containing the method to patch
        /// </summary>
        public string TargetType { get; }
        
        /// <summary>
        /// Name of the method to patch
        /// </summary>
        public string TargetMethod { get; }
        
        /// <summary>
        /// Types of the method parameters (for overload resolution)
        /// </summary>
        public Type[] ParameterTypes { get; }
        
        /// <summary>
        /// Creates a new patch attribute
        /// </summary>
        /// <param name="targetType">Full name of the class containing the method to patch</param>
        /// <param name="targetMethod">Name of the method to patch</param>
        /// <param name="parameterTypes">Types of the method parameters (for overload resolution)</param>
        public PatchAttribute(string targetType, string targetMethod, Type[] parameterTypes = null)
        {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            ParameterTypes = parameterTypes ?? Type.EmptyTypes;
        }
    }
}