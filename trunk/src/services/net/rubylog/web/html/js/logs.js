function LogCtrl($scope) {
  $scope.apps = [];

  $scope.addApp = function() {
  };

  // starts the SignalR persistent connection.
  var connection = $scope.connection = $.connection('/logs');
  connection.received(function(data) {
    var log = JSON.parse(data);
    var app = apps[log.application];
    $scope.apply(function() { });
  });
  connection.start().done(function() { });
}