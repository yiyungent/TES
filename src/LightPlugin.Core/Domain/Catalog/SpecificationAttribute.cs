using System.Collections.Generic;
using LightPlugin.Core.Domain.Localization;

namespace LightPlugin.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a specification attribute
    /// </summary>
    public partial class SpecificationAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<SpecificationAttributeOption> _specificationAttributeOptions;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute options
        /// </summary>
        public virtual ICollection<SpecificationAttributeOption> SpecificationAttributeOptions
        {
            get { return _specificationAttributeOptions ?? (_specificationAttributeOptions = new List<SpecificationAttributeOption>()); }
            protected set { _specificationAttributeOptions = value; }
        }
    }
}
