(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStudentConcessionController', ManageStudentConcessionController);

    ManageStudentConcessionController.$inject = ['$scope', 'adminService', 'Factory'];

    function ManageStudentConcessionController($scope, adminService, Factory) {
        $scope.checkSysytemLogin();
        $scope.MonthFeeList = [];

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

        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };


        $scope.resetForm = function () {
            $scope.MonthFeeList = [];
            $scope.SelectedAdmission = {};
            $scope.Admission = {};
            $scope.AdmissionId = 0;
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }

        $scope.getFeePaymentDetail = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getFeePaymentDetail(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentChargeFeeHeadList = response.data.StudentChargeFeeHeadList;
                        $scope.Admission = response.data.AdmissionDetail;
                        $scope.MonthFeeList = response.data.MonthFeeList;
                        $scope.AdmissionId = $scope.Admission.AdmissionId;
                        $scope.PaidFeeHeadList = response.data.PaidFeeHeadList;
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
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.getFeePaymentDetail($scope.SelectedAdmission.AdmissionId);
                }
            };
        }

        $scope.changeConcession = function (monthFeeHead) {
            if (monthFeeHead.Concession > monthFeeHead.FeeAmount) {
                monthFeeHead.Concession = monthFeeHead.FeeAmount;
                toastr.warning("Concession amount should not be more than fee amount.", "Warning");
            }
            monthFeeHead.LineTotal = monthFeeHead.FeeAmount - monthFeeHead.Concession;
        }

        $scope.saveStudentConcession = function () {
            var saveAdmissionModel = {
                AdmissionId: $scope.SelectedAdmission.AdmissionId,
                MonthFeeHeadList: $scope.MonthFeeList
            }
            $scope.dataLoading = true;
            adminService.saveStudentConcession(saveAdmissionModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Student Concession Updated Successfully.", "Success");
                        $scope.resetForm();
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
