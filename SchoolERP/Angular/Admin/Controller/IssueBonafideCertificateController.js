(function () {
    'use strict';

    angular
        .module('app')
        .controller('IssueBonafideCertificateController', IssueBonafideCertificateController);

    IssueBonafideCertificateController.$inject = ["$scope", "adminService", "loadDataService", "$routeParams", "$location"];

    function IssueBonafideCertificateController($scope, adminService, loadDataService, $routeParams, $location) {
        $scope.checkSysytemLogin();
        $scope.BonafideCertificate = {};
        $scope.id = $routeParams.id;

        $scope.getBonafideCertificateList = function () {
            if ($routeParams.id) {
                $scope.dataLoading = true;
                adminService.getBonafideCertificateList(0, 0, 0, $routeParams.id)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.BonafideCertificate = response.data.BonafideCertificateList[0];
                            $scope.BonafideCertificate.DateOfBirth = new Date($scope.BonafideCertificate.DateOfBirth);
                            $scope.BonafideCertificate.DateOfIssue = new Date($scope.BonafideCertificate.DateOfIssue);
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
        $scope.getBonafideCertificateList();

        $scope.getUnissuedBonafideCertificateAdmissionList = function () {
            if ($routeParams.id) {
                return;
            }
            $scope.dataLoading = true;
            adminService.getUnissuedBonafideCertificateAdmissionList()
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
        $scope.getUnissuedBonafideCertificateAdmissionList();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.BonafideCertificate.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.BonafideCertificate.StudentName = $scope.SelectedAdmission.FullName;
                    $scope.BonafideCertificate.AdmissionNo = $scope.SelectedAdmission.AdmissionNo;
                    $scope.BonafideCertificate.ClassName = $scope.SelectedAdmission.ClassName;
                    $scope.BonafideCertificate.FatherName = $scope.SelectedAdmission.FatherName;
                    $scope.BonafideCertificate.Gender = $scope.SelectedAdmission.Gender;
                    $scope.BonafideCertificate.SessionName = $scope.SelectedAdmission.SessionName;
                    if ($scope.SelectedAdmission.DateOfBirth != null)
                        $scope.BonafideCertificate.DateOfBirth = new Date($scope.SelectedAdmission.DateOfBirth);

                }
            };
        }

        $scope.$watch('formBonafideCertificate.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBonafideCertificate = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var BonafideCertificateModel = {
                    AdmissionId: $scope.BonafideCertificate.AdmissionId,
                    CertificateNo: $scope.BonafideCertificate.CertificateNo,
                    DateOfIssue: loadDataService.getDateTime($scope.BonafideCertificate.DateOfIssue),
                    DateOfBirth: loadDataService.getDateTime($scope.BonafideCertificate.DateOfBirth),
                    StudentName: $scope.BonafideCertificate.StudentName,
                    AdmissionNo: $scope.BonafideCertificate.AdmissionNo,
                    ClassName: $scope.BonafideCertificate.ClassName,
                    FatherName: $scope.BonafideCertificate.FatherName,
                    SessionName: $scope.BonafideCertificate.SessionName,
                    IssuePurpose: $scope.BonafideCertificate.IssuePurpose,
                    Gender: $scope.BonafideCertificate.Gender,
                    BonafideCertificateId: $scope.BonafideCertificate.BonafideCertificateId,
                }
                $scope.dataLoading = true;
                adminService.saveBonafideCertificate(BonafideCertificateModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (BonafideCertificateModel.BonafideCertificateId > 0) {
                                toastr.success("Bonafide certificate detail updated successfully.");
                                window.open("/admin/PrintBonafideCertificate/" + response.data.BonafideCertificateId);
                                $location.path('BonafideCertificateList');
                            } else {
                                toastr.success("Bonafide Certificate issued successfully,", "Success");
                                $scope.getUnissuedBonafideCertificateAdmissionList();
                                window.open("/admin/PrintBonafideCertificate/" + response.data.BonafideCertificateId);
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
            $scope.BonafideCertificate = {};
            $scope.SelectedAdmission = {};
            $scope.IsSubmitted = false;
            $scope.formBonafideCertificate.$setPristine();
            $scope.formBonafideCertificate.$setUntouched();
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }
    }
})();
