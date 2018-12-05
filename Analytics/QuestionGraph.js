function CreateQuestionGraph()
{
  var dataTable = CreateQuestionDataTable(m_JSONObjects);
  DrawQuestionDataTable(dataTable);
}

function CreateQuestionDataTable(JSONObjects)
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
            data.custom_params.question.startsWith("ETHICAL") == false &&

            (m_FirstTimeOnly == false || data.custom_params.times_answered == "0"));
  });

  console.log(questionAnsweredEvents);

  for (i = 0; i < questionAnsweredEvents.length; ++i)
  {
    var question = questionAnsweredEvents[i].custom_params.question;
    var questionID = questionAnsweredEvents[i].custom_params.question;
    var correct = questionAnsweredEvents[i].custom_params.correct;

    //If the key doesn't yet, add a new object         
    if ((question in questionDictionary) == false)                                
    {
      questionDictionary[question] = {correct:0, incorrect:0};
    }

    if (correct == "true") { questionDictionary[questionID].correct += 1; }
    else                   { questionDictionary[questionID].incorrect += 1; }
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
                        questionDictionaryOrdered[key].correct,
                        questionDictionaryOrdered[key].incorrect
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Times Correct');
  dataTable.addColumn('number', '# Times Not Correct');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawQuestionDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Question Overview',
                 'width': m_Width,
                 'height': m_Height,
                 'isStacked': stackedType,
                 'hAxis': {
                    title: 'Question ID',
                  },
                  'vAxis': {
                    title: '# Times',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_questions_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_questions_div'));
  chart.draw(dataTable, options);
}