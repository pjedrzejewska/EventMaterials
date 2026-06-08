/* Generate measures from columns */

var measuresTable = Model.Tables["_Measures"];
var kpiSourceTable = Model.Tables["Sales"];

foreach(var measure in measuresTable.Measures.ToList())
{
    measure.Delete();
}

foreach (var column in kpiSourceTable.Columns)
{
    if (column.Name.Contains("KPI "))
    {
        var measure = measuresTable.AddMeasure(
        column.Name + " BASE",                                    // Name
        "SUM(" + column.DaxObjectFullName + ")",        // DAX expression
        "BASE"                                          // Display Folder
        );

        foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
        {
        // add measure
        measuresTable.AddMeasure(
        measure.Name + " " + calcItem.Name,                              // Name
        "CALCULATE(" + measure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
        calcItem.Name                         // Display Folder
        );
        }
    }

}