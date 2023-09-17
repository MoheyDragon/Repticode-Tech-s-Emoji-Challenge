using System.Collections.Generic;
public struct LocalizedDialogueGroup
{
    
    public List<Row> rows;
    public LocalizedDialogueGroup(List<Row> _rows)
    {
        rows = _rows;
    }
}