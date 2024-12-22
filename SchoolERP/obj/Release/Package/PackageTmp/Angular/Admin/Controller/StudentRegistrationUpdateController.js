(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentRegistrationUpdateController', StudentRegistrationUpdateController);

    StudentRegistrationUpdateController.$inject = ["$scope", "adminService", "loadDataService", "ConstantData","$location"];

    function StudentRegistrationUpdateController($scope, adminService, loadDataService, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.GenderList = ConstantData.Gender;
        $scope.StateList = [];
        $scope.PermanentCityList = [];

        $scope.getStudentRegistrationListForSearch = function () {
            $scope.dataLoading = true;
            adminService.getStudentRegistrationListForSearch(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentRegistrationList = response.data.StudentRegistrationList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStudentRegistrationListForSearch();

        $scope.loadRegistration = function () {
            $scope.Registration.DOB = new Date($scope.Registration.DOB);
            $scope.Registration.RegistrationDate = new Date($scope.Registration.RegistrationDate);
            $scope.Registration.AadharNo = parseInt($scope.Registration.AadharNo);
            $scope.Registration.MobileNo = parseInt($scope.Registration.MobileNo);
            $scope.Registration.PermanentStateId = parseInt($scope.Registration.StateId);
            $scope.changePermanentState();
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.Registration = selected.originalObject;
                $scope.loadRegistration();
            };
        }

        $scope.resetRegistration = function () {
            $scope.Registration = {};
            $scope.Registration.StudentTypeId = null;
            $scope.Registration.Gender = null;
            $scope.Registration.ClassId = null;
            $scope.Registration.AcademicSessionId = null;
            $scope.Registration.PermanentCityId = null;
            $scope.Registration.PermanentStateId = null;
            $scope.Registration.RegistrationDate = new Date();
            $scope.IsSubmitted = false;
        }
        $scope.resetRegistration();

        $scope.$watch('formRegistration.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
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

        $scope.changePermanentState = function () {
            $scope.PermanentCityList = [];
            angular.forEach($scope.CityList, function (city, key) {
                if (city.StateId == $scope.Registration.PermanentStateId) {
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

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        var index = { AcademicSessionId: null, SessionName: '--Select Session--' };
                        $scope.AcademicSessionList.unshift(index);
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

        $scope.updateStudentRegistration = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StudentRegistrationModel = {
                    StudentTypeId: $scope.Registration.StudentTypeId,
                    StudentRegistrationId: $scope.Registration.StudentRegistrationId,
                    AcademicSessionId: $scope.Registration.AcademicSessionId,
                    ClassId: $scope.Registration.ClassId,
                    Remarks: $scope.Registration.Remarks,
                    AadharNo: $scope.Registration.AadharNo,
                    Address: $scope.Registration.Address,
                    PermanentCityId: $scope.Registration.PermanentCityId,
                    DOB: loadDataService.getDateTime($scope.Registration.DOB),
                    EmailId: $scope.Registration.EmailId,
                    FatherName: $scope.Registration.FatherName,
                    Name: $scope.Registration.Name,
                    Gender: $scope.Registration.Gender,
                    MobileNo: $scope.Registration.MobileNo,
                    MotherName: $scope.Registration.MotherName,
                    RegistrationDate: loadDataService.getDateTime($scope.Registration.RegistrationDate),
                }
                $scope.dataLoading = true;
                adminService.updateStudentRegistration(StudentRegistrationModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Registration Successfully Done.");
                            if ($scope.IsEdit) {
                                $location.path('/StudentRegistrationList')
                            }
                            //$scope.resetForm();
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
            $scope.resetRegistration();
            $scope.changePermanentState();
            $scope.formRegistration.$setPristine();
            $scope.formRegistration.$setUntouched();
        }

        $scope.checkRegistrationSession = function () {
            if (sessionStorage.getItem('editStudentRegistrationDetail') != null && sessionStorage.getItem('editStudentRegistrationDetail') != undefined) {
                $scope.IsEdit = true;
                $scope.Registration = JSON.parse(sessionStorage.getItem('editStudentRegistrationDetail'));
                $scope.$broadcast('angucomplete-alt:changeInput', 'StudentAutoComplete', $scope.Registration.RegistrationNo + " (" + $scope.Registration.Name + ")");
                $scope.loadRegistration();
                sessionStorage.removeItem('editStudentRegistrationDetail');
            }
        }
        $scope.checkRegistrationSession();
    }
})();
