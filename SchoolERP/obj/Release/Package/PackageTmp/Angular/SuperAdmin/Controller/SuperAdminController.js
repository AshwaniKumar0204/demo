(function () {
    "use strict";

    angular.module("app").controller("SuperAdminController", SuperAdminController);

    SuperAdminController.$inject = ["$scope", "superAdminService", "$rootScope", "loadDataService"];

    function SuperAdminController($scope, superAdminService, $rootScope, loadDataService) {
        $scope.getUserDetail = function () {
            $scope.UserDetail = loadDataService.getUserDetail();
            if (!$scope.UserDetail)
                window.location.href = '/site/systemlogin';
        }
        $scope.getUserDetail();


        $scope.checkSystemLogin = function () {
            $scope.dataLoading = true;
            superAdminService.checkSysytemLogin()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        if (response.data.LoginId == 0)
                            window.location.href = '/site/systemlogin';
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.checkSystemLogin();


        $scope.logoutSystemLogin = function () {
            $scope.dataLoading = true;
            superAdminService.logoutSystemLogin()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        window.location.href = '/site/systemlogin';
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
    }
}());
