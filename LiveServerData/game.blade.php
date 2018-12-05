<!DOCTYPE html>
<html lang="en-us">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>HackShield Future Cyber Heroes</title>
    <meta name="description" content=" HackShield is een spannend spel over de gevaren op internet. Je ontwikkelt skills, leert over online gevaren en kunt vragen stellen aan experts zodat je je kunt wapenen tegen online criminaliteit.">
    <link rel="shortcut icon" href="TemplateData/favicon.ico">
    <link rel="stylesheet" href="TemplateData/style.css">
    <link rel="stylesheet" href="{{ asset('/css/reset.css') }}">
    <link rel="stylesheet" href="{{ asset('/css/styles.css') }}">
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.5.0/css/all.css" integrity="sha384-B4dIYHKNBt8Bc12p+WXckhzcICo0wtJAoU8YZTY5qE0Id1GSseTk6S+L3BlXeVIU" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="TemplateData/UnityProgress.js"></script>
    <script src="Build/UnityLoader.js"></script>
    <script>
      var gameInstance = UnityLoader.instantiate("gameContainer", "Build/HackShield Update 1.1.json", {onProgress: UnityProgress});
    </script>
  </head>
  <body>

    @include('partials.header')
    @include('partials.nav')

    <div class="webgl-container">
      <div class="webgl-content">
        <div id="gameContainer" style="width: 1280px; height: 720px";></div>
        <div class="footer">
          <div class="fullscreen" onclick="gameInstance.SetFullscreen(1)"><i class="fas fa-expand"></i></div>
        </div>
      </div>
    </div>
  </body>

</html>
