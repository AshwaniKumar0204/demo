(function () {
    'use strict';

    angular
        .module('app')
        .controller('DailyCollectionReportController', DailyCollectionReportController);

    DailyCollectionReportController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData','loadDataService'];

    function DailyCollectionReportController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.PaymentModeList = ConstantData.PaymentModeAll;
        $scope.DCR = {};
        $scope.DCR.PaymentMode = null;
        $scope.DCR.LoginId = 0;
        $scope.DCR.FromDate = new Date();
        $scope.DCR.ToDate = new Date();
        $scope.DailyCollectionReport = [];
        $scope.DailyCollectionReport.TableHeaderList = [];


        $scope.$watch('formDCR.$valid', function (value) {
            $scope.IsFormDCRValid = value;
        })

        $scope.getSystemLoginList = function () {
            $scope.dataLoading = true;
            adminService.getSystemLoginList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SystemLoginList = response.data.SystemLoginList;
                        var index = { LoginId: 0, UserName: '--All Users--' };
                        $scope.SystemLoginList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSystemLoginList();

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeHeadList = response.data.FeeHeadList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeHeadList();

        $scope.getDailyCollectionReport = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormDCRValid) {
                toastr.error("From Date and To Date is required!!");
                return;
            }
            var DCRRequest = {
                FromDate: loadDataService.getDateTime($scope.DCR.FromDate),
                ToDate: loadDataService.getDateTime($scope.DCR.ToDate),
                FeeHeadList: $scope.FeeHeadList,
                LoginId: $scope.DCR.LoginId,
                PaymentMode: $scope.DCR.PaymentMode,
            }
            $scope.dataLoading = true;
            adminService.getDailyCollectionReport(DCRRequest)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DailyCollectionReport = response.data.DailyCollectionReport;
                        if ($scope.DailyCollectionReport.TableItemList.length == 0) {
                            toastr.error("No fee collection is found!!");
                        }

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
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
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
