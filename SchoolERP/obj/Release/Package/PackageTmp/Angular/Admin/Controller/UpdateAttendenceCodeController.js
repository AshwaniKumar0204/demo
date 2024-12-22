(function () {
    'use strict';

    angular
        .module('app')
        .controller('UpdateAttendenceCodeController', UpdateAttendenceCodeController);

    UpdateAttendenceCodeController.$inject = ["$scope", "adminService","Factory"];

    function UpdateAttendenceCodeController($scope, adminService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getAttendenceCodeList = function () {
            var obj = {};
            $scope.dataLoading = true;
            adminService.getAttendenceCodeList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffList = response.data.StaffList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })

        }
        $scope.getAttendenceCodeList();

        $scope.updateAttendenceCode = function () {
            $scope.dataLoading = true;
            adminService.updateAttendenceCode($scope.StaffList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("All records updated successfully.");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.getAttendenceCodeList();
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })

        }

    }
})();
