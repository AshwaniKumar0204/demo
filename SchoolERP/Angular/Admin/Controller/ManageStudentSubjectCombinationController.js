(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStudentSubjectCombinationController', ManageStudentSubjectCombinationController);

    ManageStudentSubjectCombinationController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function ManageStudentSubjectCombinationController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StudentSubjectCombination = {};
        $scope.StudentSubjectCombination.SubjectCombinationId = null;
        $scope.StartDate = new Date();

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

        $scope.getSubjectCombinationList = function () {
            $scope.dataLoading = true;
            adminService.getSubjectCombinationList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectCombinationList = response.data.SubjectCombinationList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSubjectCombinationList();

        $scope.getStudentSubjectCombinationList = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getStudentSubjectCombinationList(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentSubjectCombinationList = response.data.StudentSubjectCombinationList;
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
                    $scope.SubSubjectCombinationList = [];
                    $scope.getStudentSubjectCombinationList($scope.SelectedAdmission.AdmissionId);
                    angular.forEach($scope.SubjectCombinationList, function (subjectCombination, key) {
                        if (subjectCombination.ClassId == $scope.SelectedAdmission.ClassId && subjectCombination.AcademicSessionId == $scope.SelectedAdmission.AcademicSessionId) {
                            $scope.SubSubjectCombinationList.push(subjectCombination);
                        }
                    })
                    var index = { SubjectCombinationId: null, SubjectCombinationName: '--Select Subject Combination--' };
                    $scope.SubSubjectCombinationList.unshift(index);
                }
            };
        }

        $scope.$watch('formStudentSubjectCombination.$valid', function (value) {
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

        $scope.saveStudentSubjectCombination = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StudentSubjectCombinationModel = {
                    AdmissionId: $scope.SelectedAdmission.AdmissionId,
                    StudentSubjectCombinationId: $scope.StudentSubjectCombination.StudentSubjectCombinationId,
                    SubjectCombinationId: $scope.StudentSubjectCombination.SubjectCombinationId,
                    StartDate: loadDataService.getDateTime($scope.StartDate),
                    EndDate: null
                }
                if ($scope.EndDate != undefined && $scope.EndDate != null) {
                    StudentSubjectCombinationModel.EndDate = loadDataService.getDateTime($scope.EndDate);
                }

                $scope.dataLoading = true;
                adminService.saveStudentSubjectCombination(StudentSubjectCombinationModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StudentSubjectCombinationModel.SubjectCombinationId > 0)
                                toastr.success("Student Subject Combination Detail Updated Successfully.");
                            else
                                toastr.success("Student Subject Combination Created Successfully.");
                            $scope.resetForm();
                            $scope.StudentSubjectCombinationList = response.data.StudentSubjectCombinationList;
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
            $scope.StudentSubjectCombination = [];
            $scope.StudentSubjectCombination.SubjectCombinationId = null;
            $scope.StartDate = new Date();
            $scope.EndDate = null;
            $scope.formStudentSubjectCombination.$setPristine();
            $scope.formStudentSubjectCombination.$setUntouched();
        }

        $scope.newStudentSubjectCombination = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStudentSubjectCombination = function (StudentSubjectCombinationModel) {
            $scope.resetForm();
            $scope.StudentSubjectCombination = StudentSubjectCombinationModel;
            $scope.StartDate = new Date($scope.StudentSubjectCombination.StartDate);
            if ($scope.StudentSubjectCombination.EndDate != null) {
                $scope.EndDate = new Date($scope.StudentSubjectCombination.EndDate);
            }
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StudentSubjectCombinationId) {
            $scope.StudentSubjectCombinationId = StudentSubjectCombinationId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStudentSubjectCombination = function () {
            $scope.dataLoading = true;
            adminService.deleteStudentSubjectCombination($scope.StudentSubjectCombinationId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One student charge deleted successfully.", "Deleted Successfully.")
                        $scope.StudentSubjectCombinationList = response.data.StudentSubjectCombinationList;
                        $scope.StudentSubjectCombinationId = null;
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
