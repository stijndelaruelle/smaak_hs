function CreateVideoTimeGraph()
{
  var dataTable = CreateVideoTimeDataTable(m_JSONObjects);
  DrawVideoTimeDataTable(dataTable);
}

function CreateVideoTimeDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var videoDictionary = {};

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
    var timeWatched = parseInt(videoStopEvents[i].custom_params.time_played);

    //If the key doesn't yet, add a new object (very unlikely to happen)
    if ((videoName in videoDictionary) == false)
    {
      videoDictionary[videoName] = {totalEvents:0, totalSkippedEvents:0, cummTimeWatched:0, cummSkippedTimeWatched:0, videoLength:0};
    } 

    //Add to the object
    videoDictionary[videoName].totalEvents += 1;
    videoDictionary[videoName].cummTimeWatched += timeWatched;

    if (hasFinished == false)
    {
      videoDictionary[videoName].totalSkippedEvents += 1;
      videoDictionary[videoName].cummSkippedTimeWatched += timeWatched;
    }
    else
    {
      //If we finished watching we can fetch the max video length (hopefull at least 1 person has completely watched it, or this won't work.)
      if (videoDictionary[videoName].videoLength == 0)
        videoDictionary[videoName].videoLength = timeWatched;
    }
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
                        videoDictionaryOrdered[key].cummTimeWatched / videoDictionaryOrdered[key].totalEvents,
                        videoDictionaryOrdered[key].cummSkippedTimeWatched / videoDictionaryOrdered[key].totalSkippedEvents
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', 'Average time watched');
  dataTable.addColumn('number', 'Average time watched when skipped');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawVideoTimeDataTable(dataTable)
{
  // Set chart options
  var options = {'title': 'Video Times',
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

  document.getElementById("js_p").innerHTML = '<div id="chart_videotime_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.LineChart(document.getElementById('chart_videotime_div'));
  chart.draw(dataTable, options);
}