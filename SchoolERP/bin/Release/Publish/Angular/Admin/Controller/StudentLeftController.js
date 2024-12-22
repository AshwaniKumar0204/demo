(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentLeftController', StudentLeftController);

    StudentLeftController.$inject = ["$scope", "adminService", "loadDataService", "ConstantData"];

    function StudentLeftController($scope, adminService, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.LeftStudentTypeList = ConstantData.LeftStudentType;
        $scope.LeftStudentType = null;

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
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.LeftDate = new Date();
                    $scope.getStudentPreviousDues($scope.SelectedAdmission.AdmissionId);
                }
            };
        }

        $scope.getStudentPreviousDues = function (AdmissionId) {
            $scope.dataLoading = true;
            adminService.getStudentPreviousDues(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PreviousDues = response.data.PreviousDues;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('fromStudent.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.leftStudent = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var LeftStudentModel = {
                    AdmissionId: $scope.SelectedAdmission.AdmissionId,
                    LeftDate: loadDataService.getDateTime($scope.LeftDate),
                    LeftStudentType: $scope.LeftStudentType,
                    Reason: $scope.Reason
                }
                $scope.dataLoading = true;
                adminService.leftStudent(LeftStudentModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Left Successfully");
                            $scope.resetForm();
                            $scope.getSearchAdmissionList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.LeftDate = new Date();
            $scope.SelectedAdmission = [];
            $scope.LeftStudentType = null;
            $scope.Reason = null;
            $scope.IsSubmitted = false;
            $scope.PreviousDues = 0;
            $scope.fromStudent.$setPristine();
            $scope.fromStudent.$setUntouched();
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }
    }
})();
