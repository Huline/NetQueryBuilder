using System.Linq;

namespace NetQueryBuilder.Properties
{
    /// <summary>
    ///     Represents a descriptor for a hierarchical property path, enabling navigation through its components.
    /// </summary>
    internal class PathDescriptor
    {
        private readonly string[] _parts;

        public PathDescriptor(string[] parts)
        {
            _parts = parts;
        }

        /// <summary>
        ///     Indicates whether the current path descriptor refers to a property
        ///     with additional nested child components.
        /// </summary>
        /// <remarks>
        ///     This property is generally used to determine if the path
        ///     requires further traversal to navigate through a hierarchical
        ///     property structure.
        /// </remarks>
        public bool HasChild => _parts.Length > 1;

        /// <summary>
        ///     Retrieves the current top-level property name from the hierarchical property path.
        /// </summary>
        /// <returns>
        ///     A string representing the current property name in the path. Returns null if the path is empty.
        /// </returns>
        public string GetCurrentPath()
        {
            return _parts.FirstOrDefault();
        }

        /// <summary>
        ///     Retrieves a descriptor representing the path to the next hierarchical property in the sequence.
        /// </summary>
        /// <returns>
        ///     A PathDescriptor instance containing the remaining components of the hierarchical property path.
        ///     If there are no child paths, the resulting PathDescriptor will represent an empty path.
        /// </returns>
        public PathDescriptor GetChildPath()
        {
            return new PathDescriptor(_parts.Skip(1).ToArray());
        }
    }
}