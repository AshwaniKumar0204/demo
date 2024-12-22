(function () {
    'use strict';

    angular
        .module('app')
        .controller('UpdateStudentFeePaymentController', UpdateStudentFeePaymentController);

    UpdateStudentFeePaymentController.$inject = ["$scope", "adminService", "Factory", "loadDataService", "ConstantData"];

    function UpdateStudentFeePaymentController($scope, adminService, Factory, loadDataService, ConstantData) {
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

        $scope.getSearchAdmissionList = function () {
            $scope.dataLoading = true;
            adminService.getSearchAdmissionList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchAdmissionList();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
            };
        }

        $scope.getPendingFeePaymentTempList = function () {
            if ($scope.AdmissionId > 0) {
                $scope.dataLoading = true;
                adminService.getPendingFeePaymentTempList(loadDataService.getDateMMDDYYYY($scope.FromDate), loadDataService.getDateMMDDYYYY($scope.ToDate), $scope.AdmissionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.FeeCollectionTempList = response.data.FeeCollectionTempList;
                            $scope.IsSelected = false;
                            if ($scope.FeeCollectionTempList.length == 0) {
                                toastr.warning("No data found.");
                            }
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Invalid admission no !!");
            }
        }

        $scope.updateStudentFeePayment = function (FeeCollectionTempId, Status) {
            var UpdateFeePaymentStatusModel = {
                FeeCollectionTempId: FeeCollectionTempId,
                Status: Status
            };
            $scope.dataLoading = true;
            adminService.updateStudentFeePayment(UpdateFeePaymentStatusModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Payment status updated successfully");
                        $scope.getPendingFeePaymentTempList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.autoUpdateFeePaymentStatus = function (FeeCollectionTemp) {
            $scope.dataLoading = true;
            adminService.autoUpdateFeePaymentStatus(FeeCollectionTemp)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Payment status updated successfully");
                    } else {
                        toastr.error(response.data.Message);
                    }
                        $scope.getPendingFeePaymentTempList();
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
