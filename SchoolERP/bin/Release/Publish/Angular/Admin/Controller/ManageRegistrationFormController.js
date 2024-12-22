(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRegistrationFormController', ManageRegistrationFormController);

    ManageRegistrationFormController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageRegistrationFormController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getAcademicSessionList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.ClassId
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getRegistrationFormList = function () {
            if ($scope.AcademicSessionId == null) {
                $scope.RegistrationFormList = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getRegistrationFormList(0, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationFormList = response.data.RegistrationFormList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.RegistrationFormList = [];

        $scope.$watch('formRegistrationForm.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.setThumbnailPhoto = function (element, id) {
            if (element.files[0].type != 'image/jpeg' && element.files[0].type != 'image/jpg' && element.files[0].type != 'image/png') {
                document.getElementById(id).value = "";
                toastr.error("File Format should be in jpg format.");
            } else if (element.files[0].size > 202400) {
                document.getElementById(id).value = "";
                toastr.error("File size should be less than 200 KB.");
            }

            var reader = new FileReader();
            reader.onload = function (event) {
                var dataUrl = event.target.result;
                var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                $scope.ThumbnailPhoto = base64Data;
                $scope.$apply()
            }
            reader.readAsDataURL(element.files[0]);
        }

        $scope.SaveRegistrationForm = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var RegistrationFormModel = {
                    AcademicSessionId: $scope.RegistrationForm.AcademicSessionId,
                    RegistrationFormId: $scope.RegistrationForm.RegistrationFormId,
                    ClassNo: $scope.RegistrationForm.ClassNo,
                    PhaseName: $scope.RegistrationForm.PhaseName,
                    FromDate: loadDataService.getDateTime($scope.RegistrationForm.FromDate),
                    ToDate: loadDataService.getDateTime($scope.RegistrationForm.ToDate),
                    FromDOB: loadDataService.getDateTime($scope.RegistrationForm.FromDOB),
                    ToDOB: loadDataService.getDateTime($scope.RegistrationForm.ToDOB),
                    FormCharge: $scope.RegistrationForm.FormCharge,
                    AllowStudentPhoto: $scope.RegistrationForm.AllowStudentPhoto,
                    AllowFathersPhoto: $scope.RegistrationForm.AllowFathersPhoto,
                    AllowMothersPhoto: $scope.RegistrationForm.AllowMothersPhoto,
                    AllowSameSchool: $scope.RegistrationForm.AllowSameSchool,
                    SameSchoolFormCharge: $scope.RegistrationForm.SameSchoolFormCharge,
                    NoticeDetail: $scope.RegistrationForm.NoticeDetail,
                    ThumbnailPhoto: $scope.ThumbnailPhoto,
                }

                $scope.dataLoading = true;
                adminService.saveRegistrationForm(RegistrationFormModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (RegistrationFormModel.RegistrationFormId > 0)
                                toastr.success("Application Form Updated Successfully.");
                            else
                                toastr.success("New Application Form Created Successfully.");
                            $scope.RegistrationFormList = response.data.RegistrationFormList;
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

        $scope.RegistrationForm = {};
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.RegistrationForm = {};
            $scope.formRegistrationForm.$setPristine();
            $scope.formRegistrationForm.$setUntouched();
        }

        $scope.newRegistrationForm = function () {
            $scope.resetForm();
            $scope.RegistrationForm.AcademicSessionId = $scope.AcademicSessionId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editRegistrationForm = function (RegistrationFormModel) {
            $scope.resetForm();
            $scope.RegistrationForm = RegistrationFormModel;
            $scope.RegistrationForm.FromDate = new Date(RegistrationFormModel.FromDate);
            if ($scope.RegistrationForm.ToDate != null)
                $scope.RegistrationForm.ToDate = new Date(RegistrationFormModel.ToDate);
            if ($scope.RegistrationForm.FromDOB != null)
                $scope.RegistrationForm.FromDOB = new Date(RegistrationFormModel.FromDOB);
            if ($scope.RegistrationForm.ToDOB != null)
                $scope.RegistrationForm.ToDOB = new Date(RegistrationFormModel.ToDOB);
            $scope.RegistrationForm.FormCharge = parseInt(RegistrationFormModel.FormCharge);
            $scope.RegistrationForm.SameSchoolFormCharge = parseInt(RegistrationFormModel.SameSchoolFormCharge);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (RegistrationFormId) {
            $scope.RegistrationFormId = RegistrationFormId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteRegistrationForm = function () {
            $scope.dataLoading = true;
            adminService.deleteRegistrationForm($scope.RegistrationFormId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Application Form deleted successfully.", "Deleted Successfully.")
                        $scope.RegistrationFormList = response.data.RegistrationFormList;
                        $scope.RegistrationFormId = null;
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

