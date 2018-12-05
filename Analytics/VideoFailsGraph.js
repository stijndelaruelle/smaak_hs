function CreateVideoFailsGraph()
{
  var dataTable = CreateVideoFailsDataTable(m_JSONObjects);
  DrawVideoFailsDataTable(dataTable);
}

function CreateVideoFailsDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var videoDictionary = {};

  //Filter all video_started events
  var videoStartEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "video_started" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.video_name != "" && //Apparantly there is an unnamed video?
            data.custom_params.video_name.includes(m_Filter) == true);
  });

  console.log(videoStartEvents);

  //Analyse all the video_started events
  for (i = 0; i < videoStartEvents.length; ++i)
  {
    var videoName = videoStartEvents[i].custom_params.video_name;

    //If the key doesn't yet, add a new object
    if ((videoName in videoDictionary) == false)
    {
      videoDictionary[videoName] = {started:0, completed:0, skipped:0};
    } 

    //Add to the object
    videoDictionary[videoName].started += 1;
  }
  
  //Filter all video_stopped events
  var videoStopEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "video_stopped" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.video_name != "" && //Apparantly there is an unnamed video?
            data.custom_params.video_name.includes(m_Filter) == true);
  });

  console.log(videoStopEvents);

  //Analyse all the video_stopped events
  for (i = 0; i < videoStopEvents.length; ++i)
  {
    var videoName = videoStopEvents[i].custom_params.video_name;
    var hasFinished = (videoStopEvents[i].custom_params.finished == "true");

    //If the key doesn't yet, add a new object (very unlikely to happen)
    if ((videoName in videoDictionary) == false)
    {
      videoDictionary[videoName] = {started:0, completed:0, skipped:0};
    } 

    //Add to the object
    if (hasFinished) { videoDictionary[videoName].completed += 1; }
    else             { videoDictionary[videoName].skipped += 1; }
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
                        videoDictionaryOrdered[key].completed,
                        videoDictionaryOrdered[key].skipped,
                        videoDictionaryOrdered[key].started - videoDictionaryOrdered[key].completed - videoDictionaryOrdered[key].skipped
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Times Finished');
  dataTable.addColumn('number', '# Times Skipped');
  dataTable.addColumn('number', '# Times Disappeared(?)');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawVideoFailsDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Video Fails',
                 'width': m_Width,
                 'height': m_Height,
                 'isStacked': stackedType,
                 'hAxis': {
                    title: 'Video ID',
                  },
                  'vAxis': {
                    title: '# Times'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_videofails_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_videofails_div'));
  chart.draw(dataTable, options);
}