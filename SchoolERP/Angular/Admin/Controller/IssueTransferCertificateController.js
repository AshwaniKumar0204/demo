(function () {
    'use strict';

    angular
        .module('app')
        .controller('IssueTransferCertificateController', IssueTransferCertificateController);

    IssueTransferCertificateController.$inject = ["$scope", "adminService", "loadDataService", "ConstantData", "$routeParams", "$location"];

    function IssueTransferCertificateController($scope, adminService, loadDataService, ConstantData, $routeParams, $location) {
        $scope.checkSysytemLogin();
        $scope.TransferCertificate = {};
        $scope.NationalityList = ConstantData.Nationality;
        $scope.CategoryList = ConstantData.Category;
        $scope.WhetherFailedTwiceList = ConstantData.WhetherFailedTwice;
        $scope.WhetherQualifiedForPromotionList = ConstantData.WhetherQualifiedForPromotion;
        $scope.AnyFeeConcessionAvailedList = ConstantData.AnyFeeConcessionAvailed;
        $scope.WhetherNCCCadetList = ConstantData.WhetherNCCCadet;

        $scope.TransferCertificate = {};
        $scope.TransferCertificate.Nationality = null;
        $scope.TransferCertificate.WhetherQualifiedForPromotion = null;
        $scope.TransferCertificate.WhetherSCOrST = null;
        $scope.TransferCertificate.WhetherFailedTwice = null;
        $scope.TransferCertificate.AnyFeeConcessionAvailed = null;
        $scope.TransferCertificate.WhetherNCCCadet = null;
        $scope.id = $routeParams.id;

        $scope.getTransferCertificateList = function () {
            if ($scope.id) {
                $scope.dataLoading = true;
                adminService.getTransferCertificateList(0, 0, 0, $scope.id)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.TransferCertificate = response.data.TransferCertificateList[0];
                            $scope.TransferCertificate.AdmissionDate = new Date($scope.TransferCertificate.AdmissionDate);
                            $scope.TransferCertificate.DateOfBirth = new Date($scope.TransferCertificate.DateOfBirth);
                            $scope.TransferCertificate.DateOfApplicationOfCertiticate = new Date($scope.TransferCertificate.DateOfApplicationOfCertiticate);
                            $scope.TransferCertificate.DateOfIssue = new Date($scope.TransferCertificate.DateOfIssue);
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
        $scope.getTransferCertificateList();

        $scope.getUnissuedTransferCertificateAdmissionList = function () {
            if ($scope.id)
                return;
            $scope.dataLoading = true;
            adminService.getUnissuedTransferCertificateAdmissionList()
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
        $scope.getUnissuedTransferCertificateAdmissionList();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                    $scope.TransferCertificate = $scope.SelectedAdmission.AdmissionId;
                    $scope.TransferCertificate.StudentName = $scope.SelectedAdmission.FullName;
                    $scope.TransferCertificate.AdmissionNo = $scope.SelectedAdmission.AdmissionNo;
                    $scope.TransferCertificate.LastStudiedClass = $scope.SelectedAdmission.ClassName;
                    $scope.TransferCertificate.FatherName = $scope.SelectedAdmission.FatherName;
                    $scope.TransferCertificate.MotherName = $scope.SelectedAdmission.MotherName;
                    $scope.TransferCertificate.Nationality = $scope.SelectedAdmission.Nationality;
                    $scope.TransferCertificate.WhetherSCOrST = $scope.SelectedAdmission.Category;
                    $scope.TransferCertificate.SubjectStudied1 = $scope.SelectedAdmission.SubjectStudied1;
                    $scope.TransferCertificate.SubjectStudied2 = $scope.SelectedAdmission.SubjectStudied2;
                    $scope.TransferCertificate.SubjectStudied3 = $scope.SelectedAdmission.SubjectStudied3;
                    $scope.TransferCertificate.SubjectStudied4 = $scope.SelectedAdmission.SubjectStudied4;
                    $scope.TransferCertificate.SubjectStudied5 = $scope.SelectedAdmission.SubjectStudied5;
                    $scope.TransferCertificate.SubjectStudied6 = $scope.SelectedAdmission.SubjectStudied6;
                    if ($scope.SelectedAdmission.RegistrationDate != null)
                        $scope.TransferCertificate.AdmissionDate = new Date($scope.SelectedAdmission.RegistrationDate);
                    else if ($scope.SelectedAdmission.AdmissionDate != null)
                        $scope.TransferCertificate.AdmissionDate = new Date($scope.SelectedAdmission.AdmissionDate);
                    $scope.TransferCertificate.DateOfApplicationOfCertiticate = new Date();
                    $scope.TransferCertificate.DateOfIssue = new Date();
                    if ($scope.SelectedAdmission.DateOfBirth != null)
                        $scope.TransferCertificate.DateOfBirth = new Date($scope.SelectedAdmission.DateOfBirth);

                    $scope.dataLoading = true;
                    adminService.getStudentConcessionDetail($scope.SelectedAdmission.StudentId)
                        .then(function (response) {
                            if (response.data.Message == 'Success') {
                                $scope.TransferCertificate.AnyFeeConcessionAvailed = response.data.Concession;
                                $scope.TransferCertificate.ClassName = response.data.AdmissionClassName;
                            } else {
                                toastr.error(response.data.Message);
                            }
                            $scope.dataLoading = false;
                        }, function (err) {
                            toastr.error("Some error occured while fetching data");
                            $scope.dataLoading = false;
                        })

                }
            };
        }

        $scope.$watch('formTransferCertificate.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveTransferCertificate = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var TransferCertificateModel = {
                    TransferCertificateId: $scope.TransferCertificate.TransferCertificateId,
                    AdmissionId: $scope.TransferCertificate.AdmissionId,
                    AdmissionDate: loadDataService.getDateTime($scope.TransferCertificate.AdmissionDate),
                    DateOfApplicationOfCertiticate: loadDataService.getDateTime($scope.TransferCertificate.DateOfApplicationOfCertiticate),
                    DateOfIssue: loadDataService.getDateTime($scope.TransferCertificate.DateOfIssue),
                    DateOfBirth: loadDataService.getDateTime($scope.TransferCertificate.DateOfBirth),
                    StudentName: $scope.TransferCertificate.StudentName,
                    AdmissionNo: $scope.TransferCertificate.AdmissionNo,
                    ClassName: $scope.TransferCertificate.ClassName,
                    FatherName: $scope.TransferCertificate.FatherName,
                    MotherName: $scope.TransferCertificate.MotherName,
                    Nationality: $scope.TransferCertificate.Nationality,
                    WhetherSCOrST: $scope.TransferCertificate.WhetherSCOrST,
                    Category: $scope.TransferCertificate.Category,
                    LastStudiedClass: $scope.TransferCertificate.LastStudiedClass,
                    ExamPassedClass: $scope.TransferCertificate.ExamPassedClass,
                    WhetherFailedTwice: $scope.TransferCertificate.WhetherFailedTwice,
                    SubjectStudied1: $scope.TransferCertificate.SubjectStudied1,
                    SubjectStudied2: $scope.TransferCertificate.SubjectStudied2,
                    SubjectStudied3: $scope.TransferCertificate.SubjectStudied3,
                    SubjectStudied4: $scope.TransferCertificate.SubjectStudied4,
                    SubjectStudied5: $scope.TransferCertificate.SubjectStudied5,
                    SubjectStudied6: $scope.TransferCertificate.SubjectStudied6,
                    WhetherQualifiedForPromotion: $scope.TransferCertificate.WhetherQualifiedForPromotion,
                    QualifiedForClass: $scope.TransferCertificate.QualifiedForClass,
                    MonthUptoPaid: $scope.TransferCertificate.MonthUptoPaid,
                    AnyFeeConcessionAvailed: $scope.TransferCertificate.AnyFeeConcessionAvailed,
                    TotalWorkingDays: $scope.TransferCertificate.TotalWorkingDays,
                    TotalPresentDays: $scope.TransferCertificate.TotalPresentDays,
                    WhetherNCCCadet: $scope.TransferCertificate.WhetherNCCCadet,
                    ExtraCurricularActivities: $scope.TransferCertificate.ExtraCurricularActivities,
                    GeneralConduct: $scope.TransferCertificate.GeneralConduct,
                    ReasonForLeaving: $scope.TransferCertificate.ReasonForLeaving,
                    Remarks: $scope.TransferCertificate.Remarks,
                }
                $scope.dataLoading = true;
                adminService.saveTransferCertificate(TransferCertificateModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (TransferCertificateModel.TransferCertificateId > 0) {
                                toastr.success("Transfer certificate detail updated successfully,");
                                $scope.resetForm();
                                window.open("/admin/PrintTransferCertificate/" + response.data.TransferCertificateId);
                                $location.path('TransferCertificateList');
                            } else {
                                toastr.success("Transfer Certificate issued successfully,", "Success");
                                $scope.resetForm();
                                $scope.getUnissuedTransferCertificateAdmissionList();
                                window.open("/admin/PrintTransferCertificate/" + response.data.TransferCertificateId);
                            }
                            $scope.dataLoading = false;
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
            $scope.TransferCertificate = {};
            $scope.SelectedAdmission = {};
            $scope.IsSubmitted = false;
            $scope.formTransferCertificate.$setPristine();
            $scope.formTransferCertificate.$setUntouched();
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }
    }
})();
