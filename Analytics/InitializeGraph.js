var m_IsInitialized = false;
var m_JSONObjects;

function LoadData()
{
  m_IsInitialized = false;

  //Avoids "not well-formed XML data error"
  $.ajaxSetup({beforeSend: function(xhr)
    {
      if (xhr.overrideMimeType)
      {
        xhr.overrideMimeType("application/json");
      }
    }
  });
  
  //Unity analytics doesn't give us "valid JSON", every line is a JSON object instead of the entire file being one as well
  //Because of this we parse the file as text, split it in lines and make objects per line.

  //NOTE: Doesn't work on chrome, FIX!
  m_JSONObjects = [];

  console.log("Started loading data!");
  
  LoadDataRecursive(m_StartDate, m_EndDate);
  //LoadDataRecursiveOld(0, 9);

  /*
  //Load the JSON Data file
  $.getJSON( "TestData.json", function(JSONData)
  {
    OnJSONDataLoaded(JSONData)
  })

  .done(function() { console.log("Successfully loaded the json data file"); })
  .fail(function() { console.log("Error loading the json data file"); })
  //.always(function() { console.log("complete"); });
  */
}

function LoadDataRecursive(currentDate, endDate)
{
  var dayString = (currentDate.getDate() < 10 ? '0' : '') + currentDate.getDate();
  var monthString = ((currentDate.getMonth() + 1) < 10 ? '0' : '') + (currentDate.getMonth() + 1);
  
  var dateString = dayString + "-" + monthString + "-" + currentDate.getFullYear();

  console.log("Started loading data " + dateString);
  document.getElementById("loading_p").innerHTML = "Loading data from " + dateString + "...";

  $.get('./RAW Data/' + dateString, // + '.json',

    function(rawData)
    {
      OnDataLoaded(rawData);
      console.log("Loaded data from " + dateString);

      if (currentDate < endDate)
      {
		currentDate.setDate(currentDate.getDate() + 1);
		LoadDataRecursive(currentDate, endDate);
      }
      else
      {
        OnAllDataLoaded();
        document.getElementById("loading_p").innerHTML = "Loading complete!";
      }
    },

    'text');
}

function LoadDataRecursiveOld(currentFileID, maxFileID)
{
  console.log("Started loading data " + currentFileID);
  document.getElementById("loading_p").innerHTML = "Loading data... (this can take a couple of seconds)";

  $.get('./RAW Data/' + currentFileID + '.json',

    function(rawData)
    {
      OnDataLoaded(rawData);
      console.log("Loaded data " + currentFileID);

      if (currentFileID + 1 < maxFileID)
      {
        LoadDataRecursive(currentFileID + 1, maxFileID);
      }
      else
      {
        OnAllDataLoaded();
        document.getElementById("loading_p").innerHTML = "Loading complete!";
      }
    },

    'text');
}

function OnDataLoaded(rawData)
{
  CreateJSONObjects(rawData);
}

function OnAllDataLoaded()
{
  LoadChartLibrary();
}

function CreateJSONObjects(rawData)
{
  //Turn all these lines in JSON objects
  //rawData = encodeURI(rawData);
  var lines = rawData.split("\n");

  for (i = 0; i < lines.length; i++)
  {
      if (lines[i] != "")
      {
        var obj = JSON.parse(lines[i]); 
        m_JSONObjects.push(obj);
      } 
  } 
}

function LoadChartLibrary()
{
  // Load the Visualization API and the corechart package.
  google.charts.load('current', {'packages':['corechart']});

  // Set a callback to run when the Google Visualization API is loaded.
  google.charts.setOnLoadCallback(OnChartLibraryInitialized);
}

function OnChartLibraryInitialized()
{   
    m_IsInitialized = true;
}