(function () {
    "use strict";

    angular.module("app").controller("DashboardController", DashboardController);

    DashboardController.$inject = ["$scope", "superAdminService", "$rootScope"];

    function DashboardController($scope, superAdminService, $rootScope) {
        $scope.checkSystemLogin();

    }
}());
