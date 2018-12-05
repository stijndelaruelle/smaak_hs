function CreateDetailedQuestionGraph()
{
  var dataTable = CreateDetailedQuestionDataTable(m_JSONObjects);
  DrawDetailedQuestionDataTable(dataTable);
}

function CreateDetailedQuestionDataTable(JSONObjects)
{
  console.log(JSONObjects);

  var questionDictionary = {};

  //Filter all question_answered events
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
    var answer = questionAnsweredEvents[i].custom_params.answer;
    var answerID = answer.substring(answer.length - 1, answer.length); // the last character is always a number 1-4 (hopefully?)

    //If the key doesn't yet, add a new object         
    if ((question in questionDictionary) == false)                                
    {
      questionDictionary[question] = {answer1:0, answer2:0, answer3:0, answer4:0};
    }

    questionDictionary[question]["answer" + answerID] += 1;
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
                        questionDictionaryOrdered[key].answer1,
                        questionDictionaryOrdered[key].answer2,
                        questionDictionaryOrdered[key].answer3,
                        questionDictionaryOrdered[key].answer4,
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Level ID');
  dataTable.addColumn('number', '# Answer 1');
  dataTable.addColumn('number', '# Answer 2');
  dataTable.addColumn('number', '# Answer 3');
  dataTable.addColumn('number', '# Answer 4');
  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawDetailedQuestionDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var options = {'title': 'Detailed Question Overview',
                 'width': m_Width,
                 'height': m_Height,
                 'isStacked': stackedType,
                 'hAxis': {
                    title: 'Question ID'
                  },
                  'vAxis': {
                    title: '# Times',
                    minValue: 0,
                    viewWindow: { min: 0 },
                    viewWindowMode: 'explicit'
                  }
                };

  document.getElementById("js_p").innerHTML = '<div id="chart_detailedquestions_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_detailedquestions_div'));
  chart.draw(dataTable, options);
}