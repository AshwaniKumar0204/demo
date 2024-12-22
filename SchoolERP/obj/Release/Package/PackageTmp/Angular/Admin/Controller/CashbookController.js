(function () {
    'use strict';

    angular
        .module('app')
        .controller('CashbookController', CashbookController);

    CashbookController.$inject = ["$scope", "adminService", "loadDataService"];

    function CashbookController($scope, adminService, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.PaymentDate = new Date();

        $scope.calculateTotal = function () {
            $scope.TotalOpeningAmount = 0;
            $scope.TotalClosingAmount = 0;
            $scope.TotalTodayCashAmount = 0;
            $scope.TotalDepositAmount = 0;
            $scope.TotalCashAmount = 0;
            angular.forEach($scope.CashbookDetailList, function (cashbookDetail, key) {
                $scope.TotalCashAmount += cashbookDetail.TotalCashAmount;
                $scope.TotalOpeningAmount += cashbookDetail.OpeningAmount;
                $scope.TotalClosingAmount += cashbookDetail.ClosingAmount;
                $scope.TotalDepositAmount += cashbookDetail.DepositAmount;
                $scope.TotalTodayCashAmount += cashbookDetail.TodayCashAmount;
            })
            //if ($scope.TotalTodayCashAmount == 0) {
            //    toastr.error("No data found");
            //    $scope.CashbookDetailList = [];
            //}
        }

        $scope.changeCashDeposit = function (cashbookDetail) {
            cashbookDetail.ClosingAmount = cashbookDetail.TotalCashAmount - cashbookDetail.DepositAmount;
            $scope.calculateTotal();
            if ($scope.TotalDepositAmount > $scope.TotalCashAmount) {
                cashbookDetail.ClosingAmount = 0;
                cashbookDetail.DepositAmount = 0;
                $scope.calculateTotal();
                toastr.error("Deposit amount should not be more than total cash amount.")
            }
        }

        $scope.getCashbookDetail = function () {
            $scope.dataLoading = true;
            adminService.getCashbookDetail(loadDataService.getDateDDMMYYYY($scope.PaymentDate))
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.CashbookDetailList = response.data.CashbookDetailList;
                        if ($scope.CashbookDetailList.length == 0) {
                            toastr.error("No data found.");
                        } else {
                            $scope.calculateTotal();
                        }
                    } else if (response.data.Message == 'Exist') {
                        window.open("/admin/PrintCashbook?id=" + loadDataService.getDateDDMMYYYY($scope.PaymentDate));
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.saveCashbook = function () {
            $scope.dataLoading = true;
            adminService.saveCashbook($scope.CashbookDetailList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Success");
                        $scope.CashbookDetailList = [];
                        window.open("/admin/PrintCashbook?id=" + loadDataService.getDateDDMMYYYY($scope.PaymentDate));
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

    }
})();
