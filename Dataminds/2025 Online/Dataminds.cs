/* Generate measures from columns */

var measuresTable = Model.Tables["_Measures"];
var kpiSourceTable = Model.Tables["Sales"];

foreach(var measure in measuresTable.Measures.ToList())
{
    measure.Delete();
}

foreach(var column in kpiSourceTable.Columns)
{
    if (column.Name.Contains("KPI ")) 
    {
        var daxFunction = "SUM(";

        if (column.Name.Contains("Unit ")) 
        {
            daxFunction = "AVERAGE(";
        }

        var baseMeasure = measuresTable.AddMeasure(
        column.Name + " BASE",                                    // Name
        daxFunction + column.DaxObjectFullName + ")",        // DAX expression
        "BASE"                                          // Display Folder
        );
        baseMeasure.IsHidden = true;

    	// get correct measure name
        var indexOfBase = baseMeasure.Name.IndexOf("BASE");
        var measureNameWithoutBase = baseMeasure.Name.Substring(0, indexOfBase -5);
        var correctMeasureName = measureNameWithoutBase.Remove(0,3);


        foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
        {
             // add measure
            var newCalcMeasure = measuresTable.AddMeasure(
            correctMeasureName + " " + calcItem.Name,                              // Name
            "CALCULATE(" + baseMeasure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
            calcItem.Name                         // Display Folder
        );

            if(calcItem.Name.Contains("%")) 
                {
                    newCalcMeasure.FormatString = "#.##%";
                }
            else if(column.Name.Contains("USD"))
                {
                    newCalcMeasure.FormatString = "#.##$";
                }
            
            newCalcMeasure.Description = "BASE: " + baseMeasure.Expression + @"
        
CALC: " + calcItem.Expression;

        }
    }
}

