function CreateQuestionTimeGraph()
{
  var dataTable = CreateQuestionTimeDataTable(m_JSONObjects);
  DrawQuestionTimeDataTable(dataTable);
}

function CreateQuestionTimeDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var questionDictionary = {};

  //Filter all level_start events
  var questionAnsweredEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "question_answered" &&

            data.debug_device == false && 
            data.custom_params.debug == "false" &&

            data.custom_params.question.includes(m_Filter) == true &&

            (m_FirstTimeOnly == false || data.custom_params.times_answered == "0"));
  });

  console.log(questionAnsweredEvents);

  for (i = 0; i < questionAnsweredEvents.length; ++i)
  {
    var question = questionAnsweredEvents[i].custom_params.question;

    //If the key doesn't yet, add a new object         
    if ((question in questionDictionary) == false)                                
    {
      questionDictionary[question] = {totalEvents:0, totalCorrectEvents:0, totalIncorrectEvents:0, cummTime:0, cummCorrectTime:0, cummIncorrectTime:0};
    }

    questionDictionary[question].totalEvents += 1;
    questionDictionary[question].cummTime += parseInt(questionAnsweredEvents[i].custom_params.time);

    if (questionAnsweredEvents[i].custom_params.correct == "true")
    {
      questionDictionary[question].totalCorrectEvents += 1;
      questionDictionary[question].cummCorrectTime += parseInt(questionAnsweredEvents[i].custom_params.time);
    }
    else
    {
      questionDictionary[question].totalIncorrectEvents += 1;
      questionDictionary[question].cummIncorrectTime += parseInt(questionAnsweredEvents[i].custom_params.time);
    }
  }

  //Order the dictionary alfabetically
  var questionDictionaryOrdered = {};
  Object.keys(questionDictionary).sort().forEach(function(key)
  {
    questionDictionaryOrdered[key] = questionDictionary[key];
  });

  console.log(questionDictionaryOrdered);

  //Turn dictionary into an array
  var dataTableRows = [];
  var dataTableKeys = Object.keys(questionDictionaryOrdered);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key,
                        questionDictionaryOrdered[key].cummTime / questionDictionaryOrdered[key].totalEvents,
                        questionDictionaryOrdered[key].cummCorrectTime / questionDictionaryOrdered[key].totalCorrectEvents,
                        //questionDictionaryOrdered[key].cummIncorrectTime / questionDictionaryOrdered[key].totalIncorrectEvents
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', 'Average time to answer');
  dataTable.addColumn('number', 'Average time to answer correctly');
  //dataTable.addColumn('number', 'Average time to answer incorrectly'); //Useless data
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawQuestionTimeDataTable(dataTable)
{
  // Set chart options
  var options = {'title': 'Question Times (average)',
                 'width': m_Width,
                 'height': m_Height,
                 'hAxis': {
                    title: 'Question ID',
                  },
                  'vAxis': {
                    title: 'Time (sec)',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_questionstime_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.LineChart(document.getElementById('chart_questionstime_div'));
  chart.draw(dataTable, options);
}