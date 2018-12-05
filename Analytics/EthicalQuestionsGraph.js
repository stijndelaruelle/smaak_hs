function CreateEthicalQuestionGraph()
{
  var dataTable = CreateEthicalQuestionDataTable(m_JSONObjects);
  DrawEthicalQuestionDataTable(dataTable);
}

function CreateEthicalQuestionDataTable(JSONObjects)
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
            data.custom_params.question.startsWith("ETHICAL") == true);
  });

  console.log(questionAnsweredEvents);

  for (i = 0; i < questionAnsweredEvents.length; ++i)
  {
    var answer = questionAnsweredEvents[i].custom_params.answer;
    var answerID = answer.substring(answer.length - 1, answer.length); // the last character is always a number 1-4 (hopefully?)
    
    var level = questionAnsweredEvents[i].custom_params.level_name;
    var chapterID = level.substring(2, 3); //Every level starts with "ch" followed by the chapter number
    var chapterIDText = "Chapter"+ chapterID;

    //If the key doesn't yet, add a new object         
    if ((chapterIDText in questionDictionary) == false)                                
    {
      questionDictionary[chapterIDText] = {answer1:0, answer2:0, answer3:0, answer4:0};
    }

    questionDictionary[chapterIDText]["answer" + answerID] += 1;
  }

  console.log(questionDictionary);

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
                        questionDictionaryOrdered[key].answer4
                       ]);
  }

  console.log(dataTableRows);

  // Create the data table.
  var dataTable = new google.visualization.DataTable();
  dataTable.addColumn('string', 'Chapter ID');
  dataTable.addColumn('number', '# Answer 1');
  dataTable.addColumn('number', '# Answer 2');
  dataTable.addColumn('number', '# Answer 3');
  dataTable.addColumn('number', '# Answer 4');

  dataTable.addRows(dataTableRows);

  return dataTable;
}

function DrawEthicalQuestionDataTable(dataTable)
{
  // Set chart options
  var stackedType = 'absolute';
  if (m_DisplayAsPercentage) {stackedType = 'percent';}

  var title = 'Ethical Question ' + m_Filter + ' Overview';
  if (m_Filter == "") { title = "Not a 'Ethical Question Overview' just yet! Please fill in a question ID (1-7) as filter!"}

  var options = {'title': title,
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

  document.getElementById("js_p").innerHTML = '<div id="chart_ethicalquestions_div"></div>';

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart(document.getElementById('chart_ethicalquestions_div'));
  chart.draw(dataTable, options);
}