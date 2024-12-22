(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStudentChargeController', ManageStudentChargeController);

    ManageStudentChargeController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function ManageStudentChargeController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StudentCharge = {};
        $scope.StudentCharge.FeeHeadId = null;
        $scope.StudentCharge.FineDate = new Date();

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

        $scope.getStudentChargeList = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getStudentChargeList(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentChargeList = response.data.StudentChargeList;
                        $scope.AdmissionId = AdmissionId;
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
                    $scope.getStudentChargeList($scope.SelectedAdmission.AdmissionId);
                }
            };
        }

        $scope.$watch('formStudentCharge.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeHeadList = response.data.FeeHeadList;
                        var index = { FeeHeadId: null, FeeHeadTitle: '--Select Fee Head--' };
                        $scope.FeeHeadList.unshift(index);
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

        $scope.saveStudentCharge = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StudentChargeModel = {
                    AdmissionId: $scope.SelectedAdmission.AdmissionId,
                    StudentChargeId: $scope.StudentCharge.StudentChargeId,
                    FeeHeadId: $scope.StudentCharge.FeeHeadId,
                    FineAmount: $scope.StudentCharge.FineAmount,
                    Comments: $scope.StudentCharge.Comments,
                    FineDate: loadDataService.getDateTime($scope.StudentCharge.FineDate)
                }

                $scope.dataLoading = true;
                adminService.saveStudentCharge(StudentChargeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StudentChargeModel.StudentChargeId > 0)
                                toastr.success("Student Charge Detail Updated Successfully.");
                            else
                                toastr.success("Student Charge Created Successfully.");
                            $scope.resetForm();
                            $scope.StudentChargeList = response.data.StudentChargeList;
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.StudentCharge = [];
            $scope.StudentCharge.FeeHeadId = null;
            $scope.StudentCharge.FineDate = new Date();
            $scope.formStudentCharge.$setPristine();
            $scope.formStudentCharge.$setUntouched();
        }

        $scope.newStudentCharge = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStudentCharge = function (studentChargeModel) {
            $scope.resetForm();
            $scope.StudentCharge = studentChargeModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StudentChargeId) {
            $scope.StudentChargeId = StudentChargeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStudentCharge = function () {
            $scope.dataLoading = true;
            adminService.deleteStudentCharge($scope.StudentChargeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One student charge deleted successfully.", "Deleted Successfully.")
                        $scope.StudentChargeList = response.data.StudentChargeList;
                        $scope.StudentChargeId = null;
                        $('#modal_confirmation').modal('hide');
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
