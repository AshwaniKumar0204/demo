(function () {
    'use strict';

    angular
        .module('app')
        .controller('IssueCharacterCertificateController', IssueCharacterCertificateController);

    IssueCharacterCertificateController.$inject = ["$scope", "adminService", "loadDataService", "$routeParams","$location"];

    function IssueCharacterCertificateController($scope, adminService, loadDataService, $routeParams, $location) {
        $scope.checkSysytemLogin();
        $scope.CharacterCertificate = {};
        $scope.id = $routeParams.id;

        $scope.getCharacterCertificateList = function () {
            if ($routeParams.id) {
                $scope.dataLoading = true;
                adminService.getCharacterCertificateList(0, 0, 0, $routeParams.id)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.CharacterCertificate = response.data.CharacterCertificateList[0];
                            $scope.CharacterCertificate.DateOfBirth = new Date($scope.CharacterCertificate.DateOfBirth);
                            $scope.CharacterCertificate.FromDate = new Date($scope.CharacterCertificate.FromDate);
                            $scope.CharacterCertificate.ToDate = new Date($scope.CharacterCertificate.ToDate);
                            $scope.CharacterCertificate.IssueDate = new Date($scope.CharacterCertificate.IssueDate);
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
        $scope.getCharacterCertificateList();

        $scope.getUnissuedCharacterCertificateAdmissionList = function () {
            if ($routeParams.id) {
                return;
            }
            $scope.dataLoading = true;
            adminService.getUnissuedCharacterCertificateAdmissionList()
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
        $scope.getUnissuedCharacterCertificateAdmissionList();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.CharacterCertificate.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.CharacterCertificate.StudentName = $scope.SelectedAdmission.FullName;
                    $scope.CharacterCertificate.AdmissionNo = $scope.SelectedAdmission.AdmissionNo;
                    $scope.CharacterCertificate.ClassName = $scope.SelectedAdmission.ClassName;
                    $scope.CharacterCertificate.FatherName = $scope.SelectedAdmission.FatherName;
                    $scope.CharacterCertificate.MotherName = $scope.SelectedAdmission.MotherName;
                    $scope.CharacterCertificate.FromDate = new Date($scope.SelectedAdmission.AdmissionDate);
                    if ($scope.SelectedAdmission.DateOfBirth != null)
                        $scope.CharacterCertificate.DateOfBirth = new Date($scope.SelectedAdmission.DateOfBirth);

                }
            };
        }

        $scope.$watch('formCharacterCertificate.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveCharacterCertificate = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var CharacterCertificateModel = {
                    AdmissionId: $scope.CharacterCertificate.AdmissionId,
                    FromDate: loadDataService.getDateTime($scope.CharacterCertificate.FromDate),
                    ToDate: loadDataService.getDateTime($scope.CharacterCertificate.ToDate),
                    IssueDate: loadDataService.getDateTime($scope.CharacterCertificate.IssueDate),
                    DateOfBirth: loadDataService.getDateTime($scope.CharacterCertificate.DateOfBirth),
                    StudentName: $scope.CharacterCertificate.StudentName,
                    AdmissionNo: $scope.CharacterCertificate.AdmissionNo,
                    ClassName: $scope.CharacterCertificate.ClassName,
                    FatherName: $scope.CharacterCertificate.FatherName,
                    MotherName: $scope.CharacterCertificate.MotherName,
                    CharacterCertificateId: $scope.CharacterCertificate.CharacterCertificateId,
                }
                $scope.dataLoading = true;
                adminService.saveCharacterCertificate(CharacterCertificateModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (CharacterCertificateModel.CharacterCertificateId > 0) {
                                toastr.success("Character certificate detail updated successfully.");
                                window.open("/admin/PrintCharacterCertificate/" + response.data.CharacterCertificateId);
                                $location.path('CharacterCertificateList');
                            } else {
                                toastr.success("Character Certificate issued successfully,", "Success");
                                $scope.getUnissuedCharacterCertificateAdmissionList();
                                window.open("/admin/PrintCharacterCertificate/" + response.data.CharacterCertificateId);
                            }
                            $scope.resetForm();
                        } else {
                            toastr.error(response.data.Message, "Server Side Error");
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
            $scope.CharacterCertificate = {};
            $scope.SelectedAdmission = {};
            $scope.IsSubmitted = false;
            $scope.formCharacterCertificate.$setPristine();
            $scope.formCharacterCertificate.$setUntouched();
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }
    }
})();
