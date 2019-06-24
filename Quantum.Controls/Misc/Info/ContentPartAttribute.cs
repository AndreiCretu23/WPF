using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Controls
{
    /// <summary>
    /// Represents an attribute that is applied to the class definition of a control which supports content
    /// to identify the types of the named parts of the content that are used for templating.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ContentPartAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the content template part.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the type of the content template part.
        /// </summary>
        public Type Type { get; set; }


        /// <summary>
        /// Creates a new instance of the ContentPartAttribute class.
        /// </summary>
        public ContentPartAttribute()
        {
        }


        /// <summary>
        /// Returns a value indicating if the value of this attribute is the default value.
        /// </summary>
        /// <returns></returns>
        public override bool IsDefaultAttribute()
        {
            return Name == null && Type == null;
        }


        /// <summary>
        /// Returns a value indicating if the specified instance is equal by value to this ContentTemplateAttribute instance.
        /// </summary>
        /// <param name="obj">The instance to compare with.</param>
        /// <returns></returns>
        public override bool Match(object obj)
        {
            return obj is ContentPartAttribute optionalTemplatePart &&
                   optionalTemplatePart.Name == Name &&
                   optionalTemplatePart.Type == Type;
        }
    }
}
