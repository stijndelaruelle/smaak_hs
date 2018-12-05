function CreateLevelTimeGraph(div)
{
  var dataTable = CreateLevelTimeDataTable(m_JSONObjects);
  DrawLevelTimeDataTable(dataTable);
}

function CreateLevelTimeDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var levelDictionary = {};

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

    //If the key doesn't yet, add a new object
    if ((levelName in levelDictionary) == false)
    {
      levelDictionary[levelName] = {totalEvents:0, totalCompleteEvents:0, cummCurrentTime:0, cummCurrentCompleteTime:0, cummTotalCompleteTime:0};
    } 

    //Add to the object
    levelDictionary[levelName].totalEvents += 1;
    levelDictionary[levelName].totalCompleteEvents += 1;
    levelDictionary[levelName].cummCurrentTime += parseInt(levelCompleteEvents[i].custom_params.current_time);
    levelDictionary[levelName].cummCurrentCompleteTime += parseInt(levelCompleteEvents[i].custom_params.current_time);
    levelDictionary[levelName].cummTotalCompleteTime += parseInt(levelCompleteEvents[i].custom_params.total_time);
  }

  //Filter all level_fail events
  var levelFailEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "level_fail" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" && //NOTE THE CAPITAL "D", probably a typo in C# :(

            data.custom_params.level_name.includes(m_Filter) == true);
  });

  console.log(levelFailEvents);

  //Analyse all the level_fail events
  for (i = 0; i < levelFailEvents.length; ++i)
  {
    var levelName = levelFailEvents[i].custom_params.level_name;

    //If the key doesn't yet, add a new object (should only happen if a level has never been completed)
    if ((levelName in levelDictionary) == false)
    {
      levelDictionary[levelName] = {totalEvents:0, totalCompleteEvents:0, cummCurrentTime:0, cummCurrentCompleteTime:0, cummTotalCompleteTime:0};
    } 

    //Add to the object
    levelDictionary[levelName].totalEvents += 1;
    levelDictionary[levelName].cummCurrentTime += parseInt(levelFailEvents[i].custom_params.current_time);
  }

  //Order the dictionary alfabetically
  var levelDictionaryOrdered = {};
  Object.keys(levelDictionary).sort().forEach(function(key)
  {
    levelDictionaryOrdered[key] = levelDictionary[key];
  });

  console.log(levelDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(levelDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        levelDictionaryOrdered[key].cummTotalCompleteTime / levelDictionaryOrdered[key].totalCompleteEvents,
                        levelDictionaryOrdered[key].cummCurrentCompleteTime / levelDictionaryOrdered[key].totalCompleteEvents,
                        levelDictionaryOrdered[key].cummCurrentTime / levelDictionaryOrdered[key].totalEvents,
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', 'Average total time to complete');
  dataTable.addColumn('number', 'Average run time to complete');
  dataTable.addColumn('number', 'Average run time');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawLevelTimeDataTable(dataTable)
{
  // Set chart options
  var options = {'title': 'Level Times (average)',
                 'width': m_Width,
                 'height': m_Height,
                 'hAxis': {
                    title: 'Level ID'
                  },
                  'vAxis': {
                    title: 'Time (sec)',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_leveltime_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.LineChart(document.getElementById('chart_leveltime_div'));
  chart.draw(dataTable, options);
}