(function () {
    'use strict';

    angular
        .module('app')
        .controller('ChequebookController', ChequebookController);

    ChequebookController.$inject = ["$scope", "adminService", "loadDataService"];

    function ChequebookController($scope, adminService, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.PaymentDate = new Date();

        $scope.calculateTotal = function () {
            $scope.TotalOpeningAmount = 0;
            $scope.TotalClosingAmount = 0;
            $scope.TotalTodayChequeAmount = 0;
            $scope.TotalDepositAmount = 0;
            $scope.TotalChequeAmount = 0;
            angular.forEach($scope.ChequebookDetailList, function (chequebookDetail, key) {
                $scope.TotalChequeAmount += chequebookDetail.TotalChequeAmount;
                $scope.TotalOpeningAmount += chequebookDetail.OpeningAmount;
                $scope.TotalClosingAmount += chequebookDetail.ClosingAmount;
                $scope.TotalDepositAmount += chequebookDetail.DepositAmount;
                $scope.TotalTodayChequeAmount += chequebookDetail.TodayChequeAmount;
            })
            //if ($scope.TotalTodayChequeAmount == 0) {
            //    toastr.error("No data found");
            //    $scope.ChequebookDetailList = [];
            //}
        }

        $scope.changeChequeDeposit = function (chequebookDetail) {
            chequebookDetail.ClosingAmount = chequebookDetail.TotalChequeAmount - chequebookDetail.DepositAmount;
            $scope.calculateTotal();
            if ($scope.TotalDepositAmount > $scope.TotalChequeAmount) {
                chequebookDetail.ClosingAmount = 0;
                chequebookDetail.DepositAmount = 0;
                $scope.calculateTotal();
                toastr.error("Deposit amount should not be more than total cheque amount.")
            }
        }

        $scope.getChequebookDetail = function () {
            $scope.dataLoading = true;
            adminService.getChequebookDetail(loadDataService.getDateDDMMYYYY($scope.PaymentDate))
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ChequebookDetailList = response.data.ChequebookDetailList;
                        if ($scope.ChequebookDetailList.length == 0) {
                            toastr.error("No data found.");
                        } else {
                            $scope.calculateTotal();
                        }
                    } else if (response.data.Message == 'Exist') {
                        window.open("/admin/PrintChequebook?id=" + loadDataService.getDateDDMMYYYY($scope.PaymentDate));
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.saveChequebook = function () {
            $scope.dataLoading = true;
            adminService.saveChequebook($scope.ChequebookDetailList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Success");
                        $scope.ChequebookDetailList = [];
                        window.open("/admin/PrintChequebook?id=" + loadDataService.getDateDDMMYYYY($scope.PaymentDate));
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
