(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdmissionNoUpdateController', AdmissionNoUpdateController);

    AdmissionNoUpdateController.$inject = ["$scope", "adminService"];

    function AdmissionNoUpdateController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.AdmissionList = [];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
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
            $scope.getAcademicSessionList();
            //$scope.getAdmissionList();
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
            if ($scope.ClassId == null) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.ClassId,
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

        $scope.getAdmissionList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.SectionId > 0) {
                $scope.dataLoading = true;
                adminService.getAdmissionListForUpdateAdmisionNo($scope.AcademicSessionId, $scope.ClassId, $scope.SectionId)
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
            } else {
                $scope.AdmissionList = [];
            }
        }

        $scope.updateAdmissionNos = function () {
            var changedAdmissionList = [];
            angular.forEach($scope.AdmissionList, function (admission, key) {
                if (admission.AdmissionNo != admission.NewAdmissionNo)
                    changedAdmissionList.push(admission);
            })

            if (changedAdmissionList.length == 0) {
                toastr.success("Admission No updated successfully.")
                return;
            }

            $scope.dataLoading = true;
            adminService.updateAdmissionNos(changedAdmissionList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Admission No updated successfully.");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.getAdmissionList();
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })

        }

    }
})();
