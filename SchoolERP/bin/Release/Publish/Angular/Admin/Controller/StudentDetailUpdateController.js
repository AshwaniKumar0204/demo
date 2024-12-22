(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentDetailUpdateController', StudentDetailUpdateController);

    StudentDetailUpdateController.$inject = ["$scope", "adminService", "$location", "loadDataService", "ConstantData"];

    function StudentDetailUpdateController($scope, adminService, $location, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
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

        $scope.getAdmissionDetail = function (AdmissionId, InputChange) {
            $scope.dataLoading = true;
            adminService.getAdmissionDetail(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Admission = response.data.AdmissionDetail;
                        $scope.AdmissionId = $scope.Admission.AdmissionId;
                        $scope.Student = $scope.Admission.Student;
                        $scope.Student.DateOfBirth = new Date($scope.Student.DateOfBirth);
                        $scope.Student.FatherAadhaarNo = parseInt($scope.Student.FatherAadhaarNo);
                        $scope.Student.MotherAadharNo = parseInt($scope.Student.MotherAadharNo);
                        $scope.Student.FamilyAnnualIncome = parseInt($scope.Student.FamilyAnnualIncome);
                        $scope.Student.MobileNo = parseInt($scope.Student.MobileNo);
                        $scope.Student.AlternateNo = parseInt($scope.Student.AlternateNo);
                        $scope.Student.AadhaarNo = parseInt($scope.Student.AadhaarNo);
                        $scope.Student.Weight = parseInt($scope.Student.Weight);
                        $scope.Student.Height = parseInt($scope.Student.Height);
                        if ($scope.Student.PermanentPinCode != null)
                            $scope.Student.PermanentPinCode = parseInt($scope.Student.PermanentPinCode);
                        if ($scope.Student.CorrespondecePinCode != null)
                            $scope.Student.CorrespondecePinCode = parseInt($scope.Student.CorrespondecePinCode);
                        if ($scope.Student.PreviousSchoolTCIssueDate != null)
                            $scope.Student.PreviousSchoolTCIssueDate = new Date($scope.Student.PreviousSchoolTCIssueDate);
                        $scope.changeCorrespondenceState();
                        $scope.changePermanentState();
                        if (InputChange)
                            $scope.$broadcast('angucomplete-alt:changeInput', 'StudentAutoComplete', $scope.Admission.Student.AdmissionNo + " (" + $scope.Admission.Student.FullName + ")");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkSession = function () {
            var AdmisssionId = sessionStorage.getItem('editAdmissionId');
            if (AdmisssionId > 0) {
                $scope.IsBack = true;
                $scope.getAdmissionDetail(AdmisssionId, true);
                sessionStorage.removeItem('editAdmissionId');
            } else {
                $scope.getSearchAdmissionList();
            }
        }
        $scope.checkSession();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                //if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                $scope.getAdmissionDetail($scope.SelectedAdmission.AdmissionId, false);
                //}
            };
        }

        $scope.GenderList = ConstantData.Gender;
        $scope.NationalityList = ConstantData.Nationality;
        $scope.ReligionList = ConstantData.Religion;
        $scope.CategoryList = ConstantData.Category;
        $scope.BloodGroupList = ConstantData.BloodGroup;
        $scope.StateList = [];

        $scope.resetStudent = function () {
            $scope.Student = {};
            $scope.Student.StudentTypeId = null;
            $scope.Student.Gender = null;
            $scope.Student.Nationality = null;
            $scope.Student.Religion = null;
            $scope.Student.Category = null;
            $scope.Student.BloodGroup = null;
            $scope.Student.HouseId = null;
            $scope.Student.PermanentCityId = null;
            $scope.Student.PermanentStateId = null;
            $scope.Student.CorrespondenceCityId = null;
            $scope.Student.CorrespondenceStateId = null;
            $scope.Student.SameAddress = false;
            $scope.IsSubmitted = false;
        }
        $scope.resetStudent();

        $scope.$watch('fromStudent.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.changeCorrespondenceState = function () {
            $scope.CorrespondenceCityList = [];
            angular.forEach($scope.CityList, function (city, key) {
                if (city.StateId == $scope.Student.CorrespondenceStateId) {
                    $scope.CorrespondenceCityList.push(city);
                }
            })
            var index = { CityId: null, CityName: '--Select City--' };
            $scope.CorrespondenceCityList.unshift(index);
            $scope.sameAsCorrespondenceAddress();
        }

        $scope.changePermanentState = function () {
            $scope.PermanentCityList = [];
            angular.forEach($scope.CityList, function (city, key) {
                if (city.StateId == $scope.Student.PermanentStateId) {
                    $scope.PermanentCityList.push(city);
                }
            })
            var index = { CityId: null, CityName: '--Select City--' };
            $scope.PermanentCityList.unshift(index);
        }

        $scope.getCityList = function () {
            $scope.dataLoading = true;
            adminService.getCityList(ConstantData.CityStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.CityList = response.data.CityList;
                        $scope.changeCorrespondenceState();
                        $scope.changePermanentState();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getCityList();

        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(ConstantData.StateStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                        var index = { StateId: null, StateName: '--Select State--' };
                        $scope.StateList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStateList();

        $scope.getHouseList = function () {
            $scope.dataLoading = true;
            adminService.getHouseList(ConstantData.HouseStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.HouseList = response.data.HouseList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getHouseList();

        $scope.getStudentTypeList = function () {
            $scope.dataLoading = true;
            adminService.getStudentTypeList(ConstantData.StudentTypeStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentTypeList = response.data.StudentTypeList;
                        var index = { StudentTypeId: null, TypeName: '--Select Student Type--' };
                        $scope.StudentTypeList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStudentTypeList();

        //$scope.sameAsPermanentAddress = function () {
        //    if (document.getElementById('IsSameAddress2').checked) {
        //        $scope.Student.CorrespondenceStateId = $scope.Student.PermanentStateId;
        //        $scope.changeCorrespondenceState();
        //        $scope.Student.CorrespondenceCityId = $scope.Student.PermanentCityId;
        //        $scope.Student.CorrespondenceAddressLine1 = $scope.Student.PermanentAddressLine1;
        //        $scope.Student.CorrespondenceAddressLine2 = $scope.Student.PermanentAddressLine2;
        //    }
        //}

        $scope.sameAsCorrespondenceAddress = function () {
            if (document.getElementById('IsSameAddress2').checked) {
                $scope.Student.PermanentStateId = $scope.Student.CorrespondenceStateId;
                $scope.changePermanentState();
                $scope.Student.PermanentCityId = $scope.Student.CorrespondenceCityId;
                $scope.Student.PermanentAddressLine1 = $scope.Student.CorrespondenceAddressLine1;
                $scope.Student.PermanentAddressLine2 = $scope.Student.CorrespondenceAddressLine2;
                $scope.Student.PermanentPinCode = $scope.Student.CorrespondecePinCode;
            }
        }

        $scope.updateStudentDetail = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StudentModel = {
                    StudentId: $scope.Student.StudentId,
                    AadhaarNo: $scope.Student.AadhaarNo,
                    AlternateNo: $scope.Student.AlternateNo,
                    StudentTypeId: $scope.Student.StudentTypeId,
                    Category: $scope.Student.Category,
                    PermanentAddressLine1: $scope.Student.PermanentAddressLine1,
                    PermanentAddressLine2: $scope.Student.PermanentAddressLine2,
                    PermanentCityId: $scope.Student.PermanentCityId,
                    HouseName: $scope.Student.HouseName,
                    CorrespondenceAddressLine1: $scope.Student.CorrespondenceAddressLine1,
                    CorrespondenceAddressLine2: $scope.Student.CorrespondenceAddressLine2,
                    CorrespondenceCityId: $scope.Student.CorrespondenceCityId,
                    DateOfBirth: loadDataService.getDateTime($scope.Student.DateOfBirth),
                    Email: $scope.Student.Email,
                    FamilyAnnualIncome: $scope.Student.FamilyAnnualIncome,
                    FatherName: $scope.Student.FatherName,
                    FatherOccupation: $scope.Student.FatherOccupation,
                    FatherAadhaarNo: $scope.Student.FatherAadhaarNo,
                    FullName: $scope.Student.FullName,
                    Gender: $scope.Student.Gender,
                    MobileNo: $scope.Student.MobileNo,
                    MotherName: $scope.Student.MotherName,
                    MotherOccupation: $scope.Student.MotherOccupation,
                    MotherAadharNo: $scope.Student.MotherAadharNo,
                    Nationality: $scope.Student.Nationality,
                    RegistrationDate: $scope.Student.RegistrationDate,
                    Religion: $scope.Student.Religion,
                    StudentStatus: $scope.Student.StudentStatus,
                    StudentNo: $scope.Student.StudentNo,
                    BloodGroup: $scope.Student.BloodGroup,
                    HouseId: $scope.Student.HouseId,
                    Remarks: $scope.Student.Remarks,
                    PermanentPinCode: $scope.Student.PermanentPinCode,
                    CorrespondecePinCode: $scope.Student.CorrespondecePinCode,
                    PreviousSchoolName: $scope.Student.PreviousSchoolName,
                    PreviousBoardName: $scope.Student.PreviousBoardName,
                    LastClassAttend: $scope.Student.LastClassAttend,
                    MediumOfInstruction: $scope.Student.MediumOfInstruction,
                    PreviousSchoolTCNo: $scope.Student.PreviousSchoolTCNo,
                    Height: $scope.Student.Height,
                    Weight: $scope.Student.Weight,
                    PreviousSchoolTCIssueDate: loadDataService.getDateTime($scope.Student.PreviousSchoolTCIssueDate)
                }

                $scope.dataLoading = true;
                adminService.updateStudent(StudentModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Detail Updated Successfully");
                            $scope.resetForm();
                            if (sessionStorage.getItem('editAdmissionId') > 0) {
                                sessionStorage.removeItem('editAdmissionId');
                                var backURL = sessionStorage.getItem('backURL');
                                if (backURL != undefined) {
                                    sessionStorage.removeItem('backURL');
                                    $location.path('/' + backURL);
                                    console.log(backURL)
                                }
                            }
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
            $scope.resetStudent();
            $scope.Admission = [];
            $scope.changeCorrespondenceState();
            $scope.changePermanentState();
            $scope.fromStudent.$setPristine();
            $scope.fromStudent.$setUntouched();
            $scope.$broadcast('angucomplete-alt:clearInput', 'StudentAutoComplete');
        }
    }
})();
