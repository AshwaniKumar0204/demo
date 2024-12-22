(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSchoolController', ManageSchoolController);

    ManageSchoolController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData"];

    function ManageSchoolController($scope, superAdminService, loadDataService, ConstantData) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.School = {};
        $scope.SchoolId = null;
        $scope.SchoolStatusList = ConstantData.SchoolStatus;

        $scope.getSchoolList = function () {
            $scope.dataLoading = true;
            superAdminService.getSchoolList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SchoolList = response.data.SchoolList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSchoolList();

        $scope.$watch('formSchool.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveSchool = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SchoolModel = {
                    SchoolId: $scope.School.SchoolId,
                    SchoolStatus: $scope.School.SchoolStatus,
                    ShortName: $scope.School.ShortName,
                    FullName: $scope.School.FullName,
                    Tagline: $scope.School.Tagline,
                    AffiliationDetails: $scope.School.AffiliationDetails,
                    RegistrationNo: $scope.School.RegistrationNo,
                    RegisteredOffice: $scope.School.RegisteredOffice,
                    Website: $scope.School.Website,
                    Email: $scope.School.Email,
                    ContactNo: $scope.School.ContactNo,
                    AlternateNo: $scope.School.AlternateNo,
                    BankDetail: $scope.School.BankDetail,
                    LogoUrl: $scope.School.LogoUrl
                }

                $scope.dataLoading = true;
                superAdminService.saveSchool(SchoolModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SchoolModel.SchoolId > 0)
                                toastr.success("School Detail Updated Successfully.");
                            else
                                toastr.success("School Created Successfully.");
                            $scope.resetForm();
                            $scope.SchoolList = response.data.SchoolList;
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
            $scope.School = [];
            $scope.School.SchoolStatus = null;
            $scope.School.SchoolId = null;
            $scope.formSchool.$setPristine();
            $scope.formSchool.$setUntouched();
        }

        $scope.newSchool = function () {
            $scope.resetForm();
            $scope.School.SchoolId = $scope.SchoolId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editSchool = function (SchoolModel) {
            $scope.dataLoading = true;
            superAdminService.getSchoolLogo(SchoolModel.SchoolId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.LogoUrl = response.data.SchoolLogo;
                        $scope.resetForm();
                        $scope.School = SchoolModel;
                        $('#modal_Student_Charge').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.setLogo = function (element) {
            if (element.files[0].type != 'image/jpeg' && element.files[0].type != 'image/jpg' && element.files[0].type != 'image/png') {
                document.getElementById("school_logo").value = "";
                toastr.error("File Format should be in jpg, png or jpeg format." ,"Invalid Format");
                return;
            }
            if (element.files[0].size < 512000) {
                var reader = new FileReader();
                $scope.SchoolLogoName = document.getElementById('school_logo').files[0].name;
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.School.LogoUrl = base64Data;
                    $scope.$apply();
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("school_logo").value = "";
                toastr.error("File size should be less than 500 KB.","Invalid Logo Size");
            }
        }

        $scope.deleteConfirmation = function (SchoolId) {
            $scope.SchoolId = SchoolId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSchool = function () {
            $scope.dataLoading = true;
            superAdminService.deleteSchool($scope.SchoolId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One School deleted successfully.", "Deleted Successfully.")
                        $scope.SchoolList = response.data.SchoolList;
                        $scope.SchoolId = null;
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
