(function () {
  'use strict';

  // Thin wrapper around $http. Add what you need.
  angular.module('orderRevenue').factory('Api', ['$http', function ($http) {
    var base = '/api';

    function data(response) {
      return response.data;
    }

    return {
      getOrders: function (params) {
        return $http.get(base + '/orders', { params: params }).then(data);
      },

      getRevenueSummary: function () {
        return $http.get(base + '/reports/revenue-summary').then(data);
      }

      // TODO(candidate): updateOrder(id, payload) -> PUT /api/orders/{id}
    };
  }]);
})();
