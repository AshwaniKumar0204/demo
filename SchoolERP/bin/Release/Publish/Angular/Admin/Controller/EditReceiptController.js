(function () {
    'use strict';

    angular
        .module('app')
        .controller('EditReceiptController', EditReceiptController);

    EditReceiptController.$inject = ['$scope', 'adminService', 'ConstantData', 'loadDataService', 'Factory'];

    function EditReceiptController($scope, adminService, ConstantData, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.FeeTypeList = ConstantData.FeeType;
        $scope.FeeCollection = {};
        $scope.PaymentModeList = ConstantData.PaymentModes;
        $scope.PaymentModes = ConstantData.PaymentModes;

        $scope.$watch('formFeeCollection.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.resetForm = function () {
            $scope.FeeCollection = {};
            $scope.ReceiptNo = "";
            $scope.IsSubmitted = false;
            $scope.formFeeCollection.$setPristine();
            $scope.formFeeCollection.$setUntouched();
        }

        $scope.getFeeCollectionDetailByReceiptNo = function () {
            if ($scope.ReceiptNo == null || $scope.ReceiptNo == undefined) {
                $scope.ReceiptNo = "";
            }
            $scope.dataLoading = true;
            adminService.getFeeCollectionDetailByReceiptNo($scope.ReceiptNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeCollection = response.data.FeeCollection;
                        $scope.StudentDetail = response.data.StudentDetail;
                    } else {
                        $scope.FeeCollection = {};
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.calculateTotal = function () {
            $scope.FeeCollection.LineTotal = 0;
            $scope.FeeCollection.Concession = 0;
            $scope.FeeCollection.PaidAmount = 0;
            angular.forEach($scope.FeeCollection.FeeCollectionDetailList, function (feeCollectionDetail, key) {
                $scope.FeeCollection.LineTotal += feeCollectionDetail.Amount;
                $scope.FeeCollection.Concession += feeCollectionDetail.Concession;
                $scope.FeeCollection.PaidAmount += feeCollectionDetail.Amount - feeCollectionDetail.Concession;
            });
        }

        $scope.changeConcession = function (feePaymentDetail) {
            if (feePaymentDetail.Concession > feePaymentDetail.Amount) {
                toastr.error("Concession amount should not be more than fee amount!");
                feePaymentDetail.Concession = 0;
            }
            $scope.calculateTotal();
        }

        $scope.changeFeeAmount = function (feePaymentDetail) {
            if (feePaymentDetail.Amount  < 0) {
                feePaymentDetail.Amount = 0;
                toastr.error("Line total amount should be positive!");
            }
            $scope.calculateTotal();
        }


        $scope.updateFeePayment = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all the required fields!!");
                return;
            }
            $scope.dataLoading = true;
            adminService.updateFeePayment($scope.FeeCollection)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Fee collection detail updated successfully.");
                        $scope.resetForm();
                    } else {
                        $scope.FeeCollection = {};
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
