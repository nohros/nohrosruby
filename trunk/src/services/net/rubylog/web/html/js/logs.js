/**
 * Defines the backend service.
 */
angular.module('webService', ['ngResource'])
  .factory('WebService', function($resource) {
    var WebService = {
      status : $resource('/api.ashx/services/status?ServiceName=:name', {}, {}),
      services: $resource('/api.ashx/services', {}, {})
    }
    return WebService;
  });

/**
 * Defines the main application module.
 */
angular.module('logging', ['webService'])
  .directive('rbStatus', function() {
    return {
      scope:false,
      link: function(scope, elm, attrs) {
        attrs.$observe("rbStatus", function(value) {
          switch(parseInt(value)) {
            case 1:
              angular.element(elm)
                .removeClass('error warn unknown')
                .addClass('info');
              break;

            case 2:
              angular.element(elm)
                .removeClass('info warn unknown')
                .addClass('error');
              break;

            case 3:
              angular.element(elm)
                .removeClass('info error unknown')
                .addClass('warn');
              break;

            case 100:
              angular.element(elm)
                .removeClass('info error warn')
                .addClass('unknown');
              break;
          }
        });
      }
    };
  });

/**
 * Defines the controller that is used to manipulate the behavior of the
 * monitored services.
 *
 * @param $scope The current angular scope.
 * @param WebService The backend communication module.
 */
function LogCtrl($scope, WebService) {
  var services = $scope.services = [];//WebService.services.query();

  setInterval(function() {
    var now = Date.now();
    for (var i = 0,j = services.length; i < j; i++) {
      var service = services[i];
      WebService.status.get({name:service.name}, function(status) {
        if (status.timestamp-service.timestamp > service.maxIdleTime) {
          service.status.type = status.type;
          service.status.timestamp = now;
        } else if (status.type != service.status.type) {
          service.status.type = status.type;
          service.status.timestamp = status.timestamp;
        }
      });
    }
  }, 1*60*1000);
}