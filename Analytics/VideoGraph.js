function CreateVideoGraph()
{
  var dataTable = CreateVideoDataTable(m_JSONObjects);
  DrawVideoDataTable(dataTable);
}

function CreateVideoDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var videoDictionary = {};

  //Filter all level_start events
  var videoStartEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "video_started" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.video_name != "" && //Apparantly there is an unnamed video?
            data.custom_params.video_name.includes(m_Filter) == true);
  });

  console.log(videoStartEvents);

  //Analyse all the level_start events
  for (i = 0; i < videoStartEvents.length; ++i)
  {
    var videoName = videoStartEvents[i].custom_params.video_name;

    //If the key doesn't yet, add a new object
    if ((videoName in videoDictionary) == false)
    {
      videoDictionary[videoName] = {watchInGame:0, watchInMenu:0};
    } 

    //Add to the object
    if (videoStartEvents[i].custom_params.watch_in_game == "true") { videoDictionary[videoName].watchInGame += 1; }
    if (videoStartEvents[i].custom_params.watch_in_menu == "true") { videoDictionary[videoName].watchInMenu += 1; }
  }
  
  //Order the dictionary alfabetically
  var videoDictionaryOrdered = {};
  Object.keys(videoDictionary).sort().forEach(function(key)
  {
    videoDictionaryOrdered[key] = videoDictionary[key];
  });

  console.log(videoDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(videoDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        videoDictionaryOrdered[key].watchInGame,
                        videoDictionaryOrdered[key].watchInMenu
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Times Watched In Game');
  dataTable.addColumn('number', '# Times Watched In Menu');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawVideoDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Video Overview',
                 'width': m_Width,
                 'height': m_Height,
                 'isStacked': stackedType,
                 'hAxis': {
                    title: 'Video ID'
                  },
                  'vAxis': {
                    title: '# Times',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_video_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_video_div'));
  chart.draw(dataTable, options);
}