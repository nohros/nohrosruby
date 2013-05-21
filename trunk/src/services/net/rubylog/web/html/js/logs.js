function LogCtrl($scope) {
  var logs = $scope.logs = {};

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

  // starts the SignalR persistent connection.
  var connection = $scope.connection = $.connection('/logs');
  connection.received(function(data) {
    var msg = JSON.parse(data);
    logs[msg.app] = msg;
    $scope.$apply();
  });
  connection.start().done(function() { });
}