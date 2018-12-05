function CreateCharacterCreationGraph()
{
  var characterCreationEvents = InitializeCharacterCreationData(m_JSONObjects);
  InitializeCharacterCreationCanvas();

  //Gender
  var dataTable = CreateGenderDataTable(characterCreationEvents);
  DrawGenderDataTable(dataTable);

  //Skin Color
  var dataTable = CreateSkinColorDataTable(characterCreationEvents);
  DrawSkinColorDataTable(dataTable);

  //Extra Color
  var dataTable = CreateExtraColorDataTable(characterCreationEvents);
  DrawExtraColorDataTable(dataTable);

  CalculateAndDisplayAverageTime(characterCreationEvents);
}

function InitializeCharacterCreationData(JSONObjects)
{
  console.log(JSONObjects);

  //Filter all level_start events
  var characterCreationEvents = JSONObjects.filter(function (data)
  {
    return (data.name == "character_creation" &&

            data.debug_device == false && 
            data.custom_params.Debug == "false");
  });

  console.log(characterCreationEvents);

  return characterCreationEvents;
}

function CreateGenderDataTable(characterCreationEvents)
{
  //Analyse all the character_creation events
  var numMale = 0;
  var numFemale = 0;

  for (i = 0; i < characterCreationEvents.length; ++i)
  {
    //Gender
    if (characterCreationEvents[i].custom_params.male == "true")   { numMale += 1; }
    if (characterCreationEvents[i].custom_params.female == "true") { numFemale += 1; }
  }

  //Turn dictionary into an array
  var dataTable = google.visualization.arrayToDataTable([
          ['Gender', 'Amount'],
          ['Male',     numMale],
          ['Female',   numFemale],
        ]);

  return dataTable;
}

function CreateSkinColorDataTable(characterCreationEvents)
{
  //Analyse all the character_creation events
  var skinColorDictionary = {};

  for (i = 0; i < characterCreationEvents.length; ++i)
  {
    //Skin color
    var skinColor = characterCreationEvents[i].custom_params.skin_color;

    if ((skinColor in skinColorDictionary) == false) { skinColorDictionary[skinColor] = 0; } 
    skinColorDictionary[skinColor] += 1;
  }

  //Turn dictionary into an array
  var dataTableKeys = Object.keys(skinColorDictionary);
  var dataTableRows = [];

  dataTableRows.push(['Skin Color', 'Amount']);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key, skinColorDictionary[key]]);
  }

  var dataTable = google.visualization.arrayToDataTable(dataTableRows);

  return dataTable;
}

function CreateExtraColorDataTable(characterCreationEvents)
{
  //Analyse all the character_creation events
  var extraColorDictionary = {};

  for (i = 0; i < characterCreationEvents.length; ++i)
  {
    //Skin color
    var extraColor = characterCreationEvents[i].custom_params.extra_color;

    if ((extraColor in extraColorDictionary) == false) { extraColorDictionary[extraColor] = 0; } 
    extraColorDictionary[extraColor] += 1;
  }

  //Turn dictionary into an array
  var dataTableKeys = Object.keys(extraColorDictionary);
  var dataTableRows = [];

  dataTableRows.push(['Extra Color', 'Amount']);

  for (i = 0; i < dataTableKeys.length; ++i)
  {
    var key = dataTableKeys[i];
    dataTableRows.push([key, extraColorDictionary[key]]);
  }

  var dataTable = google.visualization.arrayToDataTable(dataTableRows);

  return dataTable;
}

function CalculateAndDisplayAverageTime(characterCreationEvents)
{
  //Analyse all the character_creation events
  var totalTime = 0;

  for (i = 0; i < characterCreationEvents.length; ++i)
  {
    totalTime += parseInt(characterCreationEvents[i].custom_params.time);
  }
  
  console.log(totalTime);
  console.log(characterCreationEvents.length);

  var averageTime = Math.round(totalTime / characterCreationEvents.length);

  document.getElementById("time_div").innerHTML = "Average time spent in character creation is " + averageTime + ' seconds!';
}

//Draw
function InitializeCharacterCreationCanvas()
{
  document.getElementById("js_p").innerHTML = '<table> ' +
                                                '<tr>' +
                                                  '<td><div id="chart_gender_div"></div></td>' +
                                                  '<td><div id="chart_skincolor_div"></div></td>' +
                                                  '<td><div id="chart_extracolor_div"></div></td>' +
                                                '</tr>' +
                                                '<tr>' +
                                                  '<td><div id="time_div"></div></td>' +
                                              '</table>';
}

function DrawGenderDataTable(dataTable)
{
  //Gender chart
  var options = {
                  'title': 'Character Selection - Gender',
                  'pieHole': 0.4,
                  'width': 500,
                  'height': 400,
                  'legend': 'none'
                };

  var genderGraph = new google.visualization.PieChart(document.getElementById("chart_gender_div"));
  genderGraph.draw(dataTable, options);
}

function DrawSkinColorDataTable(dataTable)
{
  //Skin color chart
  var options = {
                  'title': 'Character Selection - Skin color',
                  'pieHole': 0.4,
                  'width': 500,
                  'height': 400,
                  'legend': 'none',
                  'slices': [{color: '#fccaac'}, {color: '#bd8e60'}, {color: '#6e4236'}] //Lame; we should get the color from the array. But it's not immediatly working and I don't want to spend too much time on it.
                };

  var skinColorGraph = new google.visualization.PieChart(document.getElementById("chart_skincolor_div"));
  skinColorGraph.draw(dataTable, options);
}

function DrawExtraColorDataTable(dataTable)
{
  //Extra color chart
  var options = {
                  'title': 'Character Selection - Extra color',
                  'pieHole': 0.4,
                  'width': 500,
                  'height': 400,
                  'legend': 'none',

                  //Lame; we should get the color from the array. But it's not immediatly working and I don't want to spend too much time on it.
                  'slices': [{color: '#27d8bd'},
                             {color: '#01a3b0'},
                             {color: '#edeef1'},
                             {color: '#9dea68'},
                             {color: '#393b43'},
                             {color: '#ff5d28'},
                             {color: '#ffc04b'},
                             {color: '#f81f83'},
                             {color: '#eb0027'}] 
                };

  var extraColorGraph = new google.visualization.PieChart(document.getElementById("chart_extracolor_div"));
  extraColorGraph.draw(dataTable, options);
}