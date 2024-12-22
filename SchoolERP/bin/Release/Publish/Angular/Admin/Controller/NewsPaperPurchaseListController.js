(function () {
    'use strict';

    angular
        .module('app')
        .controller('NewsPaperPurchaseListController', NewsPaperPurchaseListController);

    NewsPaperPurchaseListController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function NewsPaperPurchaseListController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getNewsPaperYearList = function () {
            $scope.dataLoading = true;
            adminService.getNewsPaperYearList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.YearList = response.data.YearList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getNewsPaperYearList();

        $scope.getEnumMonthList = function () {
            $scope.dataLoading = true;
            adminService.getEnumMonthList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MonthList = response.data.MonthList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEnumMonthList();

        $scope.getNewPaperPurchaseList = function () {
            if ($scope.MonthNo > 0 && $scope.Year > 0) {
                $scope.dataLoading = true;
                adminService.getNewPaperPurchaseList($scope.MonthNo, $scope.Year)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.NewsPaperPurchaseList = response.data.NewsPaperPurchaseList;
                            $scope.HeaderList = response.data.HeaderList;
                            $scope.FooterList = response.data.FooterList;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.NewPaperPurchaseList = [];
            }
        }
        $scope.getNewPaperPurchaseList();

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
