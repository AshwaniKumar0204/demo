(function () {
    'use strict';

    angular
        .module('app')
        .controller('SettlementDetailsController', SettlementDetailsController);

    SettlementDetailsController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', 'loadDataService'];

    function SettlementDetailsController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();
        $scope.SettlementDetails = [];
        $scope.SettlementDetails.TableHeaderList = [];


        $scope.$watch('formSD.$valid', function (value) {
            $scope.IsFormDCRValid = value;
        })


        $scope.getSettlementDetails = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormDCRValid) {
                toastr.error("From Date and To Date is required!!");
                return;
            }
            var SDRequest = {
                FromDate: loadDataService.getDateTime($scope.FromDate),
                ToDate: loadDataService.getDateTime($scope.ToDate)
            }
            $scope.dataLoading = true;
            adminService.getSettlementDetails(SDRequest)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SettlementDetails = response.data.Settlements;
                        if ($scope.SettlementDetails.length == 0) {
                            toastr.error("No fee settlement is found!!");
                        }
                        else
                            $scope.TotalTransactionsInSettlement();

                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err);
                    $scope.dataLoading = false;
                })
        }

        $scope.getTransactionList = function (SettlementId) {
            $scope.IsSubmitted = true;
            $scope.dataLoading = true;
            adminService.getTransactionList(SettlementId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $('#modal_Transaction_List').modal('show');
                        $scope.TransactionList = response.data.TransactionList;
                        $scope.TotalTransactions();
                        if ($scope.TransactionList.length == 0) {
                            toastr.error("No Transactions found!!");
                        }
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err);
                    $scope.dataLoading = false;
                })
        }



        $scope.Total = 0;
        $scope.TotalTransactions = function () {
            var total = 0;
            angular.forEach($scope.TransactionList, function (key, value) {
                total += key.MerchantNetAmount;
            })
            $scope.Total = total;
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

        $scope.TotalReceipt = 0;
        $scope.PendingSettlement = 0;
        $scope.TotalTransactionsR = function () {
            var totalR = 0;
            $scope.PendingSettlement = 0;

            angular.forEach($scope.SettlementReceiptDetails, function (key, value) {
                totalR += key.MerchantNetAmount;
                if (!key.RequestAction) { // check if UtrNumber is null or empty
                    $scope.PendingSettlement += key.MerchantNetAmount;
                }
            })
            $scope.TotalReceipt = totalR;
        }

        $scope.TotalSettlementAmount = 0;
        $scope.TotalChargebackReturned = 0;
        $scope.TotalChargeback = 0;
        $scope.TotalRefundAmount = 0;
        $scope.TotalCapturedAmount = 0;

        $scope.TotalTransactionsInSettlement = function () {
            angular.forEach($scope.SettlementDetails, function (key, value) {
                $scope.TotalSettlementAmount += key.SettlementAmount;
                $scope.TotalChargebackReturned += key.ChargebackReturned;
                $scope.TotalChargeback += key.Chargeback;
                $scope.TotalRefundAmount += key.RefundAmount;
                $scope.TotalCapturedAmount += key.CapturedAmount;
            })
        }


        $scope.getSettlementReceiptDetails = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormDCRValid) {
                toastr.error("From Date and To Date is required!!");
                return;
            }
            var SDRequest = {
                FromDate: loadDataService.getDateTime($scope.FromDate),
                ToDate: loadDataService.getDateTime($scope.ToDate)
            }
            $scope.dataLoading = true;
            adminService.getSettlementReceiptDetails(SDRequest)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SettlementReceiptDetails = response.data.TransactionList;
                        $scope.TotalTransactionsR();
                        if ($scope.SettlementReceiptDetails.length == 0) {
                            toastr.error("No fee settlement is found!!");
                        }

                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err);
                    $scope.dataLoading = false;
                })
        }

    }
})();
