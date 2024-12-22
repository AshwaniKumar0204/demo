(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeePaymentDeleteController', FeePaymentDeleteController);

    FeePaymentDeleteController.$inject = ['$scope', 'adminService', 'ConstantData', 'loadDataService', 'Factory'];

    function FeePaymentDeleteController($scope, adminService, ConstantData, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.FeeTypeList = ConstantData.FeeType;
        $scope.PaymentModes = ConstantData.PaymentModes;

        $scope.getFeeCollectionForDelete = function () {
            if ($scope.ReceiptNo == null || $scope.ReceiptNo == undefined) {
                $scope.ReceiptNo = "";
            }
            $scope.dataLoading = true;
            adminService.getFeeCollectionForDelete($scope.ReceiptNo)
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

        $scope.deleteConfirmation = function () {
            $("#modal_confirmation").modal('show');
        }

        $scope.deleteFeeCollection = function () {
            if ($scope.FeeCollection.FeeCollectionId > 0) {
                $scope.dataLoading = true;
                adminService.deleteFeeCollection($scope.FeeCollection.FeeCollectionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Receipt Deleted Successfully");
                            $scope.ReceiptNo = null;
                            $scope.FeeCollection = {};
                            $("#modal_confirmation").modal('hide');
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
