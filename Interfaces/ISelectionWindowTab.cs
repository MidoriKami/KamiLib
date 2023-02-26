using System.Collections.Generic;

namespace KamiLib.Interfaces;

public interface ISelectionWindowTab
{
    string Id { get; }
    
    string TabName { get; }
    
    ISelectable? LastSelection { get; set; }
    
    IEnumerable<ISelectable> GetTabSelectables();

    void DrawTabExtras();

}
