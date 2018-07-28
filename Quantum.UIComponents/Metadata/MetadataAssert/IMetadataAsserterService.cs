namespace Quantum.Metadata
{
    public interface IMetadataAsserterService
    {
        void AssertMetadataCollections(object obj, string objName = null);
    }
}
