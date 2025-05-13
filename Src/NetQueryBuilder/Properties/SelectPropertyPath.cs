namespace NetQueryBuilder.Properties
{
    /// <summary>
    ///     Represents the selection of a property path with an associated selection state.
    /// </summary>
    public class SelectPropertyPath
    {
        public SelectPropertyPath(PropertyPath propertyPath, bool isSelected = true)
        {
            IsSelected = isSelected;
            Property = propertyPath;
        }

        /// <summary>
        ///     Indicates whether the property path is selected or not.
        /// </summary>
        /// <remarks>
        ///     This property is primarily used to determine if a given property path
        ///     should be included in operations such as queries or displayed in UI components.
        /// </remarks>
        public bool IsSelected { get; set; }

        /// <summary>
        ///     Represents the associated property path in the selection process.
        /// </summary>
        /// <remarks>
        ///     This property provides access to the full details of the property path,
        ///     including its metadata such as name, type, and parent type, and allows
        ///     operations related to querying and expression building.
        /// </remarks>
        public PropertyPath Property { get; }
    }
}