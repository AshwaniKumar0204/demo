(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeePaymentCancleController', FeePaymentCancleController);

    FeePaymentCancleController.$inject = ['$scope', 'adminService', 'ConstantData', 'loadDataService', 'Factory'];

    function FeePaymentCancleController($scope, adminService, ConstantData, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.FeePayment = {};
        $scope.FeePayment.FeeHeadId = null;
        $scope.FeeTypeList = ConstantData.FeeType;
        $scope.PaymentModes = ConstantData.PaymentModes;

        $scope.$watch('formFeePayment.$valid', function (value) {
            $scope.IsFormFeePaymentValid = value;
        })

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeHeads = response.data.FeeHeadList;
                        var index = { FeeHeadId: null, FeeHeadTitle: '--Select Fee Head--' };
                        $scope.FeeHeads.unshift(index);
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

        $scope.resetForm = function () {
            $scope.FeePayment = {};
            $scope.FeePayment.FeeHeadId = null;
            $scope.IsSubmitted = false;
            $scope.FeeCollection = {};
            $scope.ReceiptNo = null;
            $scope.formFeePayment.$setPristine();
            $scope.formFeePayment.$setUntouched();
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

        $scope.feePaymentCancel = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormFeePaymentValid) {
                toastr.error("Fill all required fields.")
                return;
            }
            if ($scope.FeeCollection.FeeCollectionId > 0) {
                var FeePaymentCancleModel = {
                    FeeCollectionId: $scope.FeeCollection.FeeCollectionId,
                    FineAmount: $scope.FeePayment.FineAmount,
                    FeeHeadId: $scope.FeePayment.FeeHeadId,
                    Comments: $scope.FeePayment.Comments,
                }
                $scope.dataLoading = true;
                adminService.feePaymentCancle(FeePaymentCancleModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Cheque Cancelled Successfully.");
                            $scope.resetForm();
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Invalid Receipt No");
                return;
            }
        }
    }
})();
