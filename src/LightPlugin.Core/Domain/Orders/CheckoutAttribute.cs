using System.Collections.Generic;
using LightPlugin.Core.Domain.Catalog;
using LightPlugin.Core.Domain.Localization;
using LightPlugin.Core.Domain.Stores;

namespace LightPlugin.Core.Domain.Orders
{
    /// <summary>
    /// Represents a checkout attribute
    /// </summary>
    public partial class CheckoutAttribute : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        private ICollection<CheckoutAttributeValue> _checkoutAttributeValues;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable products are required in order to display this attribute
        /// </summary>
        public bool ShippableProductRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is marked as tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }



        //validation fields

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        public string ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        public int? ValidationFileMaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        public string DefaultValue { get; set; }





        
        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value;
            }
        }
        /// <summary>
        /// Gets the checkout attribute values
        /// </summary>
        public virtual ICollection<CheckoutAttributeValue> CheckoutAttributeValues
        {
            get { return _checkoutAttributeValues ?? (_checkoutAttributeValues = new List<CheckoutAttributeValue>()); }
            protected set { _checkoutAttributeValues = value; }
        }
    }

}
