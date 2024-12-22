(function () {
    "use strict";

    angular.module("app").controller("StudentRegistrationController", StudentRegistrationController);

    StudentRegistrationController.$inject = ["$scope", "adminService", "loadDataService", "ConstantData"];

    function StudentRegistrationController($scope, adminService, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.GenderList = ConstantData.Gender;
        $scope.PaymentModeList = ConstantData.PaymentMode;
        $scope.StateList = [];
        $scope.PermanentCityList = [];

        $scope.resetRegistration = function () {
            $scope.Registration = {};
            $scope.Registration.StudentTypeId = null;
            $scope.Registration.Gender = null;
            $scope.Registration.ClassId = null;
            $scope.Registration.AcademicSessionId = null;
            $scope.Registration.PermanentCityId = null;
            $scope.Registration.PermanentStateId = null;
            $scope.Registration.PaymentMode = null;
            $scope.Registration.PaymentDate = new Date();
            $scope.Registration.DemandDraftDate = new Date();
            $scope.Registration.ChequeDate = new Date();
            $scope.Registration.RegistrationDate = new Date();
            $scope.RegistrationFeeList = [];
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
            if ($scope.Registration.ClassId == null || $scope.Registration.ClassId == undefined) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.Registration.ClassId,
                Status: 1
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

        $scope.calculateTotalRegistrationFee = function () {
            $scope.TotalRegistrationFee = 0;
            angular.forEach($scope.RegistrationFeeList, function (registrationFee, Key) {
                $scope.TotalRegistrationFee += registrationFee.Amount;
            })
        }

        $scope.getRegistrationFeeList = function () {
            if ($scope.Registration.AcademicSessionId > 0 && $scope.Registration.ClassId > 0 && $scope.Registration.StudentTypeId > 0) {
                $scope.dataLoading = true;
                adminService.getRegistrationFeeListForRegistration($scope.Registration.ClassId, $scope.Registration.StudentTypeId, $scope.Registration.AcademicSessionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.RegistrationFeeList = response.data.RegistrationFeeList;
                            $scope.calculateTotalRegistrationFee();
                            if ($scope.RegistrationFeeList.length == 0) {
                                toastr.error("No registration fee head was found.");
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
                $scope.RegistrationFeeList = [];
                $scope.TotalRegistrationFee = 0;
            }
        }


        $scope.saveStudentRegistration = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                if ($scope.RegistrationFeeList.length == 0) {
                    toastr.error("No registration fee head was found!!");
                    return;
                }

                var saveStudentRegistrationModel = {
                    StudentRegistration: {
                        StudentTypeId: $scope.Registration.StudentTypeId,
                        AcademicSessionId: $scope.Registration.AcademicSessionId,
                        ClassId: $scope.Registration.ClassId,
                        Remarks: $scope.Registration.Remarks,
                        AadharNo: $scope.Registration.AadharNo,
                        Address: $scope.Registration.Address,
                        PermanentCityId: $scope.Registration.PermanentCityId,
                        DOB: loadDataService.getDateTime($scope.Registration.DateOfBirth),
                        EmailId: $scope.Registration.EmailId,
                        FatherName: $scope.Registration.FatherName,
                        Name: $scope.Registration.FullName,
                        Gender: $scope.Registration.Gender,
                        MobileNo: $scope.Registration.MobileNo,
                        MotherName: $scope.Registration.MotherName,
                        RegistrationDate: loadDataService.getDateTime($scope.Registration.RegistrationDate),
                    },
                    FeeCollection: {
                        PaymentDate: loadDataService.getDateTime($scope.Registration.PaymentDate),
                        PaymentMode: $scope.Registration.PaymentMode,
                        Remarks: $scope.Registration.PaymentRemarks,
                        ChequeNo: $scope.Registration.ChequeNo,
                        ChequeDate: loadDataService.getDateTime($scope.Registration.ChequeDate),
                        ChequeBank: $scope.Registration.ChequeBank,
                        ChequeBankBranch: $scope.Registration.ChequeBankBranch,
                        DemandDraftNo: $scope.Registration.DemandDraftNo,
                        DemandDraftDate: loadDataService.getDateTime($scope.Registration.DemandDraftDate),
                        DemandDraftBank: $scope.Registration.DemandDraftBank,
                        TransactionNo: $scope.Registration.TransactionNo,
                    },
                    RegistrationFeeList: $scope.RegistrationFeeList
                }
                $scope.dataLoading = true;
                adminService.saveStudentRegistration(saveStudentRegistrationModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Registration Successfully Done.");
                            $scope.resetForm();
                            window.open("/admin/PrintRegistrationReceipt/" + response.data.StudentRegistrationId);
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
    }
}());
