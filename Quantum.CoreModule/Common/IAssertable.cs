namespace Quantum.Common
{
    /// <summary>
    /// This interface should be/ is implemented by types which need need to be in a certain state in order to function properly.
    /// </summary>
    public interface IAssertable
    {
        void Assert(string objName = null);
    }
}
