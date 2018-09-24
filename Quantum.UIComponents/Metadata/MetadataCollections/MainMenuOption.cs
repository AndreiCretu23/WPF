namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used for requesting an entry into the main menu of the application. 
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// The appearence / location in the main menu of the application depends of the MainMenuOption's own metadata : <para/>
    /// 1) MenuPath -> This metadata type indicates the location in the main menu of the parent entry. WARNING : Any MainMenuOption metadata collection MUST have exactly one MenuPath.
    ///                Parameters : 1.1) AbstractMenuPath parentPath -> indicates the path of the parent menu entry of the requested menu entry.
    ///                             1.2) Int32 categoryIndex -> Menu entries which have the same parentPath can get separated into categories(visually split by separators).
    ///                                                         This parameter indicates in which category should the new menu entry be located.
    ///                             1.3) Int32 OrderIndex -> Menu entries inside a category get order by an order index. This parameter specifies where in the 
    ///                                                      parent category will the new menu entry be located).<para/>
    /// 2) Description -> This metadata type indicates the new menu entry's header. WARNING : Any MainMenuOption metadata collection MUST have exactly one Description. <para/>
    /// 3) Icon -> This metadata type indicates the new menu entry's icon. A MainMenuOption can have maximum one metadata of this type. <para/>
    /// 4) ToolTip -> This metadata type indicates the new menu entry's tooltip. A MainMenuOption can have maximum one metadata of this type. <para/>
    /// 5) Checkable -> This metadata type indicates if the new menu entry is checkable or not. If the parent MainMenuOption collection does not contain this metadata type, the default value will be set to false. <para/>
    /// 6) CheckChanged -> This metadata type provides an action that is to be executed when the check state of the menu entry changes. Only makes sense if the parent MainMenuOption contains a Checkable(true) metadata. <para/>
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class MainMenuOption : MetadataCollection<IMainMenuMetadata>, ICommandMetadata
    {
    }
}
