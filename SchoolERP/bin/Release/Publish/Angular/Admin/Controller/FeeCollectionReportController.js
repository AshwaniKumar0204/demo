(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeeCollectionReportController', FeeCollectionReportController);

    FeeCollectionReportController.$inject = ['$scope', 'loadDataService', 'ConstantData', 'adminService'];

    function FeeCollectionReportController($scope, loadDataService, ConstantData, adminService) {
        $scope.checkSysytemLogin();
        $scope.PaymentModeList = ConstantData.PaymentModeAll;
        $scope.selectedPaymentModes = [];
        //angular.forEach($scope.PaymentModeList, function (pm, key) {
        //    $scope.selectedPaymentModes.push(pm);
        //});

        $scope.getAcademicSessionList = function () {
            var obj = {
                ClassId : 0
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.changeSelectAll = function () {
            angular.forEach($scope.PaymentModeList, function (payMode, key1) {
                if ($scope.IsAllSelected) {
                    payMode.IsSelected = true;
                } else {
                    payMode.IsSelected = false;
                }
            })
        }

        $scope.multiSelectOutput = [];

        $scope.$watch('formDCR.$valid', function (value) {
            $scope.IsFormDCRValid = value;
        })

        $scope.printFeeCollectionReport = function () {
            $scope.selectedPaymentModes = [];
            angular.forEach($scope.PaymentModeList, function (pm, key) {
                if (pm.IsSelected && pm.Key != null) {
                    $scope.selectedPaymentModes.push(pm);
                }
            });

            if ($scope.selectedPaymentModes.length == 0) {
                toastr.error("Payment mode is required !!", "Validation Error");
                return 0;
            }

            var paymentModes = "";
            angular.forEach($scope.selectedPaymentModes, function (pm, key) {
                if (key != 0)
                    paymentModes += "_" + pm.Key;
                else
                    paymentModes = pm.Key;
            });
            var AcademicSessionId = 0;
            if ($scope.AcademicSessionId > 0)
                AcademicSessionId = $scope.AcademicSessionId;

            $scope.IsSubmitted = true;
            if ($scope.IsFormDCRValid) {
                var FromDate = loadDataService.getDateDDMMYYYY($scope.FromDate);
                var ToDate = loadDataService.getDateDDMMYYYY($scope.ToDate);
                window.open('/admin/PrintFeeCollectionReport?FromDate=' + FromDate + '&ToDate=' + ToDate + '&Pmodes=' + paymentModes + '&session=' + AcademicSessionId);
            } else {
                toastr.error("Fill all fields !!", "Validation Error");
            }
        }
    }
})();
