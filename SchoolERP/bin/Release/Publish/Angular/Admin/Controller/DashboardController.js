(function () {
    "use strict";

    angular.module("app").controller("DashboardController", DashboardController);

    DashboardController.$inject = ["$scope", "adminService", "$rootScope"];

    function DashboardController($scope, adminService, $rootScope) {
        $scope.checkSysytemLogin();

    }
}());
