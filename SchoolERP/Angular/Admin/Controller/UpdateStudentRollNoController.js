(function () {
    'use strict';

    angular
        .module('app')
        .controller('UpdateStudentRollNoController', UpdateStudentRollNoController);

    UpdateStudentRollNoController.$inject = ["$scope", "adminService"];

    function UpdateStudentRollNoController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.AdmissionList = [];
        $scope.sortKey = 'RollNo';
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
        $scope.$watch('formUnit.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getAdmissionList = function () {
            //$scope.IsSubmitted = true;
            //if (!$scope.IsFormValid) {
            //    toastr.error("Fill all required fields!!");
            //    return;
            //}
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

        $scope.updateStudentRollNo = function () {
            var changedAdmissionList = [];

            //angular.forEach($scope.AdmissionList, function (admission, key) {
            for (var j = 0; j < $scope.AdmissionList.length; j++) {
                var admission = $scope.AdmissionList[j];

                for (var i = 0; i < changedAdmissionList.length; i++) {
                    if (changedAdmissionList[i].NewRollNo == admission.NewRollNo && changedAdmissionList[i].NewRollNo != null) {
                        toastr.error("Roll No: " + admission.NewRollNo + "", "A data duplication error has occurred.");
                        changedAdmissionList = [];
                        return;
                    }
                }

                //if (admission.RollNo != admission.NewRollNo)
                changedAdmissionList.push(admission);
            }
            //})

            if (changedAdmissionList.length == 0) {
                toastr.success("Roll No updated successfully.");
                $scope.AdmissionList = [];
                return;
            }

            $scope.dataLoading = true;
            adminService.updateStudentRollNo($scope.AdmissionList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Roll No updated successfully.");
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
