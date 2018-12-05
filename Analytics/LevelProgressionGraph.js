function CreateLevelProgressionGraph()
{
  var dataTable = CreateLevelProgressionDataTable(m_JSONObjects);
  DrawLevelProgressionDataTable(dataTable);
}

function CreateLevelProgressionDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var levelProgressionDictionary = {};

  //Filter all level_start events
  var levelStartEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "level_start" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.level_name.includes(m_Filter) == true);
  });

  console.log(levelStartEvents);

  //Analyse all the level_start events
  for (i = 0; i < levelStartEvents.length; ++i)
  {
    var levelName = levelStartEvents[i].custom_params.level_name;
    var firstTime = levelStartEvents[i].custom_params.first_time; //Not doing anything yet

    //If the key doesn't yet, add a new object
    if ((levelName in levelProgressionDictionary) == false)
    {
      levelProgressionDictionary[levelName] = {started:0, completed:0};
    } 

    //Add to the object
    levelProgressionDictionary[levelName].started += 1;
  }

  //Filter all level_complete events
  var levelCompleteEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "level_complete" &&

            data.debug_device == false && 
            data.custom_params.Debug == "false" && //NOTE THE CAPITAL "D", probably a typo in C# :(

            data.custom_params.level_name.includes(m_Filter) == true);
  });

  console.log(levelCompleteEvents);

  //Analyse all the level_complete events
  for (i = 0; i < levelCompleteEvents.length; ++i)
  {
    var levelName = levelCompleteEvents[i].custom_params.level_name;
    var firstTime = levelCompleteEvents[i].custom_params.first_time; //Not doing anything yet

    //If the key doesn't yet, add a new object (very unlikely to happen)
    if ((levelName in levelProgressionDictionary) == false)
    {
      levelProgressionDictionary[levelName] = {started:0, completed:0};
    } 

    //Add to the object
    levelProgressionDictionary[levelName].completed += 1;
  }

  //Order the dictionary alfabetically
  var levelProgressionDictionaryOrdered = {};
  Object.keys(levelProgressionDictionary).sort().forEach(function(key)
  {
    levelProgressionDictionaryOrdered[key] = levelProgressionDictionary[key];
  });

  console.log(levelProgressionDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(levelProgressionDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        levelProgressionDictionaryOrdered[key].completed,
                        levelProgressionDictionaryOrdered[key].started - levelProgressionDictionaryOrdered[key].completed
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Times Finished');
  dataTable.addColumn('number', '# Times Not Finished');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawLevelProgressionDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Level Progression',
                 'width': m_Width,
                 'height': m_Height,
                 'isStacked': stackedType,
                 'hAxis': {
                    title: 'Level ID'
                  },
                  'vAxis': {
                    title: '# Times',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_levelprogression_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_levelprogression_div'));
  chart.draw(dataTable, options);
}