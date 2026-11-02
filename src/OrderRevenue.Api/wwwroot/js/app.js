(function () {
  'use strict';

  angular
    .module('orderRevenue', ['ngRoute'])
    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
      $locationProvider.hashPrefix('!');

      $routeProvider
        .when('/orders', {
          templateUrl: 'views/orders.html',
          controller: 'OrdersController',
          controllerAs: 'vm'
        })
        .when('/summary', {
          templateUrl: 'views/summary.html',
          controller: 'SummaryController',
          controllerAs: 'vm'
        })
        .otherwise({ redirectTo: '/orders' });
    }])
    .controller('NavController', ['$location', function ($location) {
      this.isActive = function (path) {
        return $location.path() === path;
      };
    }]);
})();
