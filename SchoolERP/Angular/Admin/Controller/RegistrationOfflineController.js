(function () {
    'use strict';

    angular
        .module('app')
        .controller('RegistrationOfflineController', RegistrationOfflineController);

    RegistrationOfflineController.$inject = ["$scope", "adminService", "loadDataService", "Factory", "ConstantData"];

    function RegistrationOfflineController($scope, adminService, loadDataService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.RegistrationOnline = {};
        $scope.$watch('formRegistrationOnline.$valid', function (value) {
            $scope.isFormValid = value;
        })

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getRegistrationFormList = function () {
            if ($scope.ClassNo == null) {
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

        $scope.getAcademicSessionList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassNo: $scope.ClassNo
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

        $scope.init = function (RegistrationFormId) {
            $scope.RegistrationOnline.RegistrationFormId = RegistrationFormId;
            $scope.dataLoading = true;
            adminService.getRegistrationForm(RegistrationFormId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationForm = response.data.RegistrationForm;
                        $scope.getSectionList($scope.RegistrationForm.ClassNo);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }

        $scope.registrationLogin = function () {
            var RegistrationOnlineModel = {
                LoginPassword: $scope.LoginPassword,
                RegistrationFormId: $scope.RegistrationFormId,
                RegistrationNo: $scope.RegistrationNo,
            }
            $scope.dataLoading = true;
            adminService.registrationLogin(RegistrationOnlineModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        window.location.href = "/student/layoutStudent#!/RegistrationTwo";
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }

        $scope.getGenderList = function () {
            $scope.dataLoading = true;
            adminService.getGenderList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.GenderList = response.data.GenderList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getGenderList();

        $scope.getSectionList = function (ClassNo) {
            $scope.dataLoading = true;
            adminService.getSectionList(ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SectionList = response.data.SectionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }

        $scope.changeSectionOne = function () {
            $scope.SectionTwoList = [];
            angular.forEach($scope.SectionList, function (section) {
                if ($scope.RegistrationOnline.SectionOne != section.SectionId) {
                    $scope.SectionTwoList.push(section);
                }
            })
            $scope.changeSectionTwo();
        }

        $scope.changeSectionTwo = function () {
            $scope.SectionThreeList = [];
            $scope.SectionThree = null;
            angular.forEach($scope.SectionTwoList, function (section) {
                if ($scope.RegistrationOnline.SectionTwo != section.SectionId) {
                    $scope.SectionThreeList.push(section);
                }
            })
            $scope.changeSectionThree();
        }

        $scope.changeSectionThree = function () {
            $scope.SectionFourList = [];
            $scope.SectionFour = null;
            angular.forEach($scope.SectionThreeList, function (section) {
                if ($scope.RegistrationOnline.SectionThree != section.SectionId) {
                    $scope.SectionFourList.push(section);
                }
            })
            $scope.changeSectionFour();
        }

        $scope.changeSectionFour = function () {
            $scope.SectionFiveList = [];
            $scope.SectionFive = null;
            angular.forEach($scope.SectionFourList, function (section) {
                if ($scope.RegistrationOnline.SectionFour != section.SectionId) {
                    $scope.SectionFiveList.push(section);
                }
            })
        }

        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getStateList();

        $scope.changeState = function () {
            $scope.CityList = [];
            angular.forEach($scope.MainCityList, function (c1) {
                if (c1.StateId == $scope.RegistrationOnline.StateId)
                    $scope.CityList.push(c1);
            })
        }

        $scope.getCityList = function () {
            $scope.dataLoading = true;
            adminService.getCityList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainCityList = response.data.CityList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getCityList();

        $scope.getNationalityList = function () {
            $scope.dataLoading = true;
            adminService.getNationalityList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.NationalityList = response.data.NationalityList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getNationalityList();

        $scope.getPassingStatusList = function () {
            $scope.dataLoading = true;
            adminService.getPassingStatusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PassingStatusList = response.data.PassingStatusList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getPassingStatusList();

        $scope.getCategoryList = function () {
            $scope.dataLoading = true;
            adminService.getCategoryList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.CategoryList = response.data.CategoryList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getCategoryList();

        $scope.getEntryTypeList = function () {
            $scope.dataLoading = true;
            adminService.getEntryTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.EntryTypeList = response.data.EntryTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })

        }
        $scope.getEntryTypeList();

        $scope.changeInterMarks = function () {
            if ($scope.RegistrationForm.ClassNo != 4)
                $scope.RegistrationOnline.PCMPercentage = Math.round(($scope.RegistrationOnline.PhysicsMarks + $scope.RegistrationOnline.ChemistryMarks + $scope.RegistrationOnline.MathMarks) / 3 * 100) / 100;
        }

        $scope.saveRegistrationOne = function () {
            $scope.IsSubmitted = true;
            if ($scope.isFormValid) {
                var RegistrationSectionList = [];
                RegistrationSectionList.push({ SectionId: $scope.RegistrationOnline.SectionOne, Priority: 1 });
                if ($scope.RegistrationForm.ClassNo == 1) {  //B.tech
                    if ($scope.RegistrationOnline.EntryType == null) {
                        toastr.error("Entry Type is required.");
                        return;
                    }

                    if ($scope.RegistrationOnline.SectionTwo == null) {
                        toastr.error("Choice 2 is required.");
                        return;
                    }

                    if ($scope.RegistrationOnline.SectionThree == null) {
                        toastr.error("Choice 3 is required.");
                        return;
                    }
                    RegistrationSectionList.push({ SectionId: $scope.RegistrationOnline.SectionTwo, Priority: 2 });
                    RegistrationSectionList.push({ SectionId: $scope.RegistrationOnline.SectionThree, Priority: 3 });
                    if ($scope.RegistrationOnline.SectionFour)
                        RegistrationSectionList.push({ SectionId: $scope.RegistrationOnline.SectionFour, Priority: 4 });
                    if ($scope.RegistrationOnline.SectionFive)
                        RegistrationSectionList.push({ SectionId: $scope.RegistrationOnline.SectionFive, Priority: 5 });
                }

                var RegistrationOnlineModel = {
                    StudentName: $scope.RegistrationOnline.StudentName,
                    DOB: loadDataService.getDateTime($scope.RegistrationOnline.DOB),
                    Gender: $scope.RegistrationOnline.Gender,
                    BloodGroup: $scope.RegistrationOnline.BloodGroup,
                    EntryType: $scope.RegistrationOnline.EntryType,
                    Religion: $scope.RegistrationOnline.Religion,
                    Nationality: $scope.RegistrationOnline.Nationality,
                    Category: $scope.RegistrationOnline.Category,
                    EmailId: $scope.RegistrationOnline.EmailId,
                    MobileNo: $scope.RegistrationOnline.MobileNo,
                    PhoneNo: $scope.RegistrationOnline.PhoneNo,
                    MothersName: $scope.RegistrationOnline.MothersName,
                    FathersName: $scope.RegistrationOnline.FathersName,
                    PinCode: $scope.RegistrationOnline.PinCode,
                    CityId: $scope.RegistrationOnline.CityId,
                    StateId: $scope.RegistrationOnline.StateId,
                    CorrespondenceAddress: $scope.RegistrationOnline.CorrespondenceAddress,
                    MetricBoardName: $scope.RegistrationOnline.MetricBoardName,
                    MetricPassingYear: $scope.RegistrationOnline.MetricPassingYear,
                    MetricPercentage: $scope.RegistrationOnline.MetricPercentage,
                    InterPassingStatus: $scope.RegistrationOnline.InterPassingStatus,
                    PassingYear: $scope.RegistrationOnline.PassingYear,
                    PhysicsMarks: $scope.RegistrationOnline.PhysicsMarks,
                    ChemistryMarks: $scope.RegistrationOnline.ChemistryMarks,
                    MathMarks: $scope.RegistrationOnline.MathMarks,
                    PCMPercentage: $scope.RegistrationOnline.PCMPercentage,
                    TotalPercentage: $scope.RegistrationOnline.TotalPercentage,
                    BoardOrUniversity: $scope.RegistrationOnline.BoardOrUniversity,
                    SubjectStream: $scope.RegistrationOnline.SubjectStream,
                    RegistrationFormId: $scope.RegistrationOnline.RegistrationFormId,
                    RegistrationOnlineSections: RegistrationSectionList
                }

                $scope.dataLoading = true;
                adminService.saveRegistrationOne(RegistrationOnlineModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Success");
                            $scope.IsSubmitted = false;
                            $scope.RegistrationOnline = [];
                            $scope.formRegistrationOnline.$setUntouched();
                            $scope.formRegistrationOnline.$setPristine();

                            $scope.RegistrationNo = response.data.RegistrationNo;
                            $scope.RegistrationOnlineId = response.data.RegistrationOnlineId;
                            $scope.LoginPassword = response.data.LoginPassword;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Fill the all required fields !!");
            }
        }
    }
})();
