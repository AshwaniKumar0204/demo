(function () {
    'use strict';

    angular
        .module('app')
        .controller('UpdateSuccessFeePaymentStatusController', UpdateSuccessFeePaymentStatusController);

    UpdateSuccessFeePaymentStatusController.$inject = ["$scope", "adminService", "Factory", "loadDataService", "ConstantData"];

    function UpdateSuccessFeePaymentStatusController($scope, adminService, Factory, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.PaymentModes = ConstantData.PaymentModes;
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();
        $scope.IsSelectAll = false;

        $scope.getSuccessTempPaymentList = function () {
            $scope.dataLoading = true;
            adminService.getSuccessTempPaymentList(loadDataService.getDateMMDDYYYY($scope.FromDate), loadDataService.getDateMMDDYYYY($scope.ToDate), 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeCollectionTempList = response.data.FeeCollectionTempList;
                        $scope.IsSelected = false;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkOnlineFeePaymentStatus = function () {
            var feeCollectionTemps = [];
            angular.forEach($scope.FeeCollectionTempList, function (feeColl, key) {
                if (feeColl.IsSelected)
                    feeCollectionTemps.push(feeColl);
            });
            if (feeCollectionTemps.length > 0) {
                $scope.dataLoading = true;
                adminService.checkOnlineFeePaymentStatus(feeCollectionTemps)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Payment status updated successfully");
                            //$scope.getSuccessTempPaymentList();
                            $scope.FeeCollectionTempList = [];
                            $scope.IsSelectAll = false;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.warning("No one is selected.");
            }
        }

        $scope.selectAll = function () {
            if ($scope.IsSelectAll) {
                angular.forEach($scope.FeeCollectionTempList, function (feeColl, key) {
                    feeColl.IsSelected = true;
                });
            } else {
                angular.forEach($scope.FeeCollectionTempList, function (feeColl, key) {
                    feeColl.IsSelected = false;
                });
            }
        }
    }
})();
