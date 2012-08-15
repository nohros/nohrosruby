var app = angular
  .module('status', ['status.services', 'status.directives'])
  .config(function($routeProvider, $locationProvider) {
    $routeProvider.when('/detail/:serviceId', {
      templateUrl:'detail/',
      controller:'StatusDetailCtrl'
    });
  });