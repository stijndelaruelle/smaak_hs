function CreateLevelTriesGraph(div)
{
  var dataTable = CreateLevelTriesDataTable(m_JSONObjects);
  DrawLevelTriesDataTable(dataTable);
}

function CreateLevelTriesDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var levelCompleteDictionary = {};

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
    if ((levelName in levelCompleteDictionary) == false)
    {
      levelCompleteDictionary[levelName] = {totalCompleteEvents:0, totalTries:0, lowestTries:0, highestTries:0};
    } 

    //Add to the object
    var numTries = parseInt(levelCompleteEvents[i].custom_params.tries) + 1; //Starts counting from 0
    levelCompleteDictionary[levelName].totalCompleteEvents += 1;
    levelCompleteDictionary[levelName].totalTries += numTries; 

    //'Highscores'
    if (i == 0 || numTries < levelCompleteDictionary[levelName].lowestTries) { levelCompleteDictionary[levelName].lowestTries = numTries; }
    if (i == 0 || numTries > levelCompleteDictionary[levelName].highestTries) { levelCompleteDictionary[levelName].highestTries = numTries; }
  }

  //Order the dictionary alfabetically
  var levelCompleteDictionaryOrdered = {};
  Object.keys(levelCompleteDictionary).sort().forEach(function(key)
  {
    levelCompleteDictionaryOrdered[key] = levelCompleteDictionary[key];
  });

  console.log(levelCompleteDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(levelCompleteDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        levelCompleteDictionaryOrdered[key].totalTries / levelCompleteDictionaryOrdered[key].totalCompleteEvents,
                        //levelCompleteDictionaryOrdered[key].lowestTries,
                        levelCompleteDictionaryOrdered[key].highestTries
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', 'Average');
  //dataTable.addColumn('number', 'Lowest'); //Useless info
  dataTable.addColumn('number', 'Highest');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawLevelTriesDataTable(dataTable)
{
  // Set chart options
  var options = {'title': 'Level tries to complete',
                 'width': m_Width,
                 'height': m_Height,
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

  document.getElementById("js_p").innerHTML = '<div id="chart_leveltries_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.LineChart(document.getElementById('chart_leveltries_div'));
  chart.draw(dataTable, options);
}