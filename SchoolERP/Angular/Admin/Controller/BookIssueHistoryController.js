(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookIssueHistoryController', BookIssueHistoryController);

    BookIssueHistoryController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function BookIssueHistoryController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookAccessionStatusList = [{ Key: 0, Value: '--Select All--' }, { Key: 1, Value: 'Not Returned' }, { Key: 2, Value: 'Returned' },]
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();

        $scope.init = function (bookAccessionStatus) {
            $scope.BookAccessionStatus = bookAccessionStatus;
            $scope.getBookIssueHistory();
        }

        $scope.getBookIssueHistory = function () {
            $scope.dataLoading = true;
            adminService.getBookIssueHistory($scope.FromDate, $scope.ToDate, $scope.BookAccessionStatus)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookIssueList = response.data.BookIssueList;
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
