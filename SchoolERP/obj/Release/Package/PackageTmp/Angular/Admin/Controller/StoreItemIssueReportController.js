(function () {
    'use strict';

    angular
        .module('app')
        .controller('StoreItemIssueReportController', StoreItemIssueReportController);

    StoreItemIssueReportController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function StoreItemIssueReportController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();

        $scope.init = function (IssueStatus) {
            $scope.IssueStatus = IssueStatus;
            $scope.getStoreItemIssueHistory();
        }

        $scope.getStoreItemIssueHistory = function () {
            $scope.dataLoading = true;
            adminService.getStoreItemIssueHistory($scope.FromDate, $scope.ToDate, $scope.IssueStatus)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StoreItemIssueList = response.data.StoreItemIssueList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

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
