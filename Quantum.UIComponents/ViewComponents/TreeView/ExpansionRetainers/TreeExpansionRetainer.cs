namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic contract for any mechanism used to cache tree expansion states.
    /// </summary>
    public interface ITreeExpansionRetainer
    {
        /// <summary>
        /// Loads the expansion state value from the internal cache and sets it on the specified tree view model item.
        /// </summary>
        /// <param name="item"></param>
        void LoadExpansionState(ITreeViewModelItem item);


        /// <summary>
        /// Saves the specified tree view model item's expansion state using an view model item reference-independant key.
        /// </summary>
        /// <param name="item"></param>
        void SaveExpansionState(ITreeViewModelItem item);


        /// <summary>
        /// Cleans up any out-dated cached values.
        /// </summary>
        void CleanUp();
    }
    

    /// <summary>
    /// Represents all the possible strategies a tree view model can use to retain it's 
    /// expansion states when invalidating all or part of it's content.
    /// </summary>
    public enum TreeExpansionRetainingStrategy
    {
        /// <summary>
        /// The tree view model will not retain any expansion states. Whenever content is invalidated, the expansion values will reset.
        /// </summary>
        None,
        

        /// <summary>
        /// The tree view model will use the item path provided by the tree view model item's headers as an id to cache expansion states.
        /// </summary>
        ItemPath,


        /// <summary>
        /// The tree view model will use a "reference path" created from an item's ancestor values togather will the item's value to cache expansion states.
        /// NOTE : Selecting this option will not keep any strong references to the values.
        /// </summary>
        ReferencePath,


        /// <summary>
        /// The tree view model will use a user defined strategy to store expansion states.
        /// </summary>
        CustomStrategy
    }

}
