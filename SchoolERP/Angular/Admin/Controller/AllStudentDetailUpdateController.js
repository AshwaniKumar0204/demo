(function () {
    'use strict';

    angular
        .module('app')
        .controller('AllStudentDetailUpdateController', AllStudentDetailUpdateController);

    AllStudentDetailUpdateController.$inject = ["$scope", "adminService", "loadDataService"];

    function AllStudentDetailUpdateController($scope, adminService, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.AdmissionList = [];
        $scope.sortKey = 'RollNo';
        $scope.currentPage = 1;
        $scope.pageSize = 5;
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.AcademicSessionId = null;
        $scope.ClassId = null;
        $scope.SectionId = null;

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
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

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select Section--' };
            $scope.SectionList.unshift(index);
            $scope.getAdmissionList();
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changeClass();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(1)
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

        $scope.getAdmissionList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.SectionId > 0) {
                $scope.dataLoading = true;
                adminService.getAdmissionList($scope.AcademicSessionId, $scope.ClassId, $scope.SectionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.AdmissionList = response.data.AdmissionList;
                            angular.forEach($scope.AdmissionList, function (admission, key) {
                                admission.Student.DateOfBirth = new Date(admission.Student.DateOfBirth);
                            })

                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.AdmissionList = [];
            }
        }

        $scope.updateAllStudentDetails = function () {

            angular.forEach($scope.AdmissionList, function (admission, key) {
                admission.Student.DateOfBirth = loadDataService.getDateTime(admission.Student.DateOfBirth);
            })

            $scope.dataLoading = true;
            adminService.updateAllStudentDetails($scope.AdmissionList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("All Student Detail updated successfully.");
                        $scope.AdmissionList = [];
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
