(function () {
    'use strict';

    angular
        .module('app')
        .controller('SearchRegistrationOnlineController', SearchRegistrationOnlineController);

    SearchRegistrationOnlineController.$inject = ["$scope", "adminService", "loadDataService", "Factory", "ConstantData"];

    function SearchRegistrationOnlineController($scope, adminService, loadDataService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.GenderList = ConstantData.Gender;
        $scope.NationalityList = ConstantData.Nationality;
        $scope.ReligionList = ConstantData.Religion;
        $scope.CategoryList = ConstantData.Category;
        $scope.BloodGroupList = ConstantData.BloodGroup;
        $scope.FathersOccupationList = ConstantData.FathersOccupation;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.RegistrationOnline = {};

        $scope.getSearchRegistrationOnlineList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationOnlineList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchRegistrationOnlineList();

        $scope.getClassStreamTypeList = function () {
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.RegistrationOnline.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassStreamTypeList = response.data.ClassStreamTypeList;
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
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
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
                    toastr.error(err.data.Message);
                   $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getClassListDistinct = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassListDistinct = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassListDistinct();

        $scope.changeSubjectOptionOne = function () {
            $scope.SubjectOptionTwoList = [];
            if ($scope.RegistrationOnline.SubjectOptionOne > 0) {
                angular.forEach($scope.SubjectOptionOneList, function (subjectOption, key1) {
                    if (subjectOption.SubjectOptionId != $scope.RegistrationOnline.SubjectOptionOne) {
                        $scope.SubjectOptionTwoList.push(subjectOption);
                    }
                });
            }
        }

        $scope.getSubjectOptionList = function () {
            if ($scope.RegistrationOnline.ClassStreamTypeId == null || $scope.RegistrationOnline.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.RegistrationOnline.ClassNo, $scope.RegistrationOnline.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        $scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            $scope.changeSubjectOptionOne();
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.dataLoading = true;
                adminService.getRegistrationOnlineDetail(selected.originalObject.RegistrationOnlineId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.RegistrationOnline = response.data.RegistrationOnline;
                            $scope.RegistrationOnline.DOB = new Date($scope.RegistrationOnline.DOB);
                            $scope.RegistrationOnline.MobileNo = parseInt($scope.RegistrationOnline.MobileNo);
                            $scope.RegistrationOnline.WhatsappNo = parseInt($scope.RegistrationOnline.WhatsappNo);
                            $scope.RegistrationOnline.SMSAlertNo = parseInt($scope.RegistrationOnline.SMSAlertNo);
                            $scope.RegistrationOnline.FathersAnnualIncome = parseInt($scope.RegistrationOnline.FathersAnnualIncome);
                            $scope.RegistrationOnline.FathersMobileNo = parseInt($scope.RegistrationOnline.FathersMobileNo);
                            $scope.RegistrationOnline.MothersMobileNo = parseInt($scope.RegistrationOnline.MothersMobileNo);
                            $scope.RegistrationOnline.GuardiansMobileNo = parseInt($scope.RegistrationOnline.GuardiansMobileNo);
                            $scope.RegistrationOnline.MothersAnnualIncome = parseInt($scope.RegistrationOnline.MothersAnnualIncome);
                            $scope.getClassStreamTypeList();
                            $scope.getSubjectOptionList();
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching Registration detail.");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.SelectedRegistrationOnline = {};
            }
        }
    }
})();
