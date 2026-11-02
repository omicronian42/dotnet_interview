(function () {
  'use strict';

  angular.module('orderRevenue').controller('OrdersController', ['Api', function (Api) {
    var vm = this;

    vm.query = {
      page: 1,
      pageSize: 25,
      search: '',
      sortBy: 'orderedAtUtc',
      sortDir: 'desc'
    };
    vm.result = { items: [], total: 0 };
    vm.loading = false;
    vm.error = null;

    vm.load = function () {
      vm.loading = true;
      vm.error = null;
      Api.getOrders(vm.query).then(function (result) {
        vm.result = result;
      }).catch(function (err) {
        vm.error = (err.data && (err.data.detail || err.data.title)) || 'Could not load orders.';
      }).finally(function () {
        vm.loading = false;
      });
    };

    vm.totalPages = function () {
      return Math.max(1, Math.ceil(vm.result.total / vm.query.pageSize));
    };

    vm.goTo = function (page) {
      if (page < 1 || page > vm.totalPages()) { return; }
      vm.query.page = page;
      vm.load();
    };

    vm.search = function () {
      vm.query.page = 1;
      vm.load();
    };

    vm.sort = function (column) {
      if (vm.query.sortBy === column) {
        vm.query.sortDir = vm.query.sortDir === 'asc' ? 'desc' : 'asc';
      } else {
        vm.query.sortBy = column;
        vm.query.sortDir = 'asc';
      }
      vm.query.page = 1;
      vm.load();
    };

    vm.sortMark = function (column) {
      if (vm.query.sortBy !== column) { return ''; }
      return vm.query.sortDir === 'asc' ? '▲' : '▼';
    };

    // TODO(candidate): edit dialog / inline form for status + discount, including what the user sees on a conflict.

    vm.load();
  }]);
})();
