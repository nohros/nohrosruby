function LogCtrl($scope) {
  var apps = $scope.apps = {
    "atk.rgo" : {
      name: 'RGO SQL00',
      timestamp:20,
      status: "ERROR"
    }
  };

  $scope.addApp = function() {
  };

  setInterval(function() {
    var now = Date.now();
    for(var name in logs) {
      var log = logs[name];
      if (log.timestamp - now > 5*60) {
        log.status = "ERROR";
      }
    }
  }, 1000);
}