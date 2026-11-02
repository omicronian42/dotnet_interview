(function () {
  'use strict';

  angular.module('orderRevenue').controller('SummaryController', ['Api', function (Api) {
    var vm = this;

    vm.summary = null;
    vm.loading = true;
    vm.error = null;

    // TODO(candidate): render whatever your revenue-summary endpoint returns
    // (monthly revenue table or chart, top 5 customers). Plain is fine.
    Api.getRevenueSummary().then(function (summary) {
      vm.summary = summary;
    }).catch(function (err) {
      vm.error = (err.data && (err.data.detail || err.data.title)) || 'Could not load the summary.';
    }).finally(function () {
      vm.loading = false;
    });
  }]);
})();
