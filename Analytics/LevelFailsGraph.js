function CreateLevelFailsGraph(div)
{
  var dataTable = CreateLevelFailsDataTable(m_JSONObjects);
  DrawLevelFailsDataTable(dataTable);
}

function CreateLevelFailsDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var levelFailsDictionary = {};

  //Filter all level_fail events
  var levelFailEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "level_fail" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.level_name.includes(m_Filter) == true);
  });

  console.log(levelFailEvents);

  for (i = 0; i < levelFailEvents.length; ++i)
  {
    var levelName = levelFailEvents[i].custom_params.level_name;

    //If the key doesn't yet, add a new object
    if ((levelName in levelFailsDictionary) == false)
    {
      levelFailsDictionary[levelName] = {deathByEnemy:0, deathByQuiz:0, resetByEndUI:0, resetByUI:0};
    } 

    //Add to the object
    if (levelFailEvents[i].custom_params.death_by_enemy == "true") { levelFailsDictionary[levelName].deathByEnemy  += 1; }
    if (levelFailEvents[i].custom_params.death_by_quiz == "true")  { levelFailsDictionary[levelName].deathByQuiz   += 1; }
    if (levelFailEvents[i].custom_params.reset_by_endui == "true") { levelFailsDictionary[levelName].resetByEndUI  += 1; }
    if (levelFailEvents[i].custom_params.reset_by_ui == "true")    { levelFailsDictionary[levelName].resetByUI     += 1; }
  }

  //Order the dictionary alfabetically
  var levelFailsDictionaryOrdered = {};
  Object.keys(levelFailsDictionary).sort().forEach(function(key)
  {
    levelFailsDictionaryOrdered[key] = levelFailsDictionary[key];
  });

  console.log(levelFailsDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(levelFailsDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        levelFailsDictionaryOrdered[key].deathByEnemy,
                        levelFailsDictionaryOrdered[key].deathByQuiz,
                        levelFailsDictionaryOrdered[key].resetByEndUI,
                        levelFailsDictionaryOrdered[key].resetByUI
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Times died: Enemy');
  dataTable.addColumn('number', '# Times died: Quiz');
  dataTable.addColumn('number', '# Times reset: End UI');
  dataTable.addColumn('number', '# Times reset: General UI');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawLevelFailsDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Level Fails',
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

  document.getElementById("js_p").innerHTML = '<div id="chart_levelfails_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_levelfails_div'));
  chart.draw(dataTable, options);
}