(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeePaymentHistoryController', FeePaymentHistoryController);

    FeePaymentHistoryController.$inject = ["$scope", "adminService", "Factory", "ConstantData"];

    function FeePaymentHistoryController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.PaymentModes = ConstantData.PaymentModes;
        $scope.MonthNo = (new Date()).getMonth() + 1;
        $scope.CurrentYear = (new Date()).getFullYear();

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

        $scope.getFeePaymentListByStudentId = function (StudentId) {
            $scope.dataLoading = true;
            adminService.getFeePaymentListByStudentId(StudentId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeCollectionList = response.data.FeeCollectionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                $scope.getFeePaymentListByStudentId($scope.SelectedAdmission.StudentId);
                $scope.getFeeDueReport($scope.SelectedAdmission.AdmissionId);
            };
        }

        $scope.getFeeDueReport = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getFeeDueReportByAdmissionId(AdmissionId, $scope.MonthNo, $scope.CurrentYear)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeDueReport = response.data.FeeDueReport.DueFeeDetail;
                        $scope.DueFeeHeads = response.data.FeeDueReport.DueFeeDetail.FeeHeads;
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
