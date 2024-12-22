(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookAccessionListController', BookAccessionListController);

    BookAccessionListController.$inject = ["$scope", "adminService", "Factory","ConstantData"];

    function BookAccessionListController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookAccessionStatusList = ConstantData.BookAccessionStatus;
        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getClassList();

        $scope.getBookAccessionList = function () {
            $scope.dataLoading = true;
            adminService.getBookAccessionList($scope.ClassNo, $scope.BookAccessionStatus, $scope.FromDate, $scope.ToDate)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookAccessionList = response.data.BookAccessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookAccessionList();

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
