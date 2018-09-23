using System;

namespace Quantum.Common
{
    /// <summary>
    /// Defines the object as unique, idenfiable by a custom id.
    /// </summary>
    public interface IIdentifiable
    {
        string Guid { get; }
    }
}
