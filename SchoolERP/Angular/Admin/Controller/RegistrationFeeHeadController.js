(function () {
    'use strict';

    angular
        .module('app')
        .controller('RegistrationFeeHeadController', RegistrationFeeHeadController);

    RegistrationFeeHeadController.$inject = ["$scope", "adminService"];

    function RegistrationFeeHeadController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.MonthList = [];
        $scope.AcademicSessionId = null;
        $scope.ClassId = null;
        $scope.StudentTypeId = null;

        $scope.getStudentTypeList = function () {
            $scope.dataLoading = true;
            adminService.getStudentTypeList(1)
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

        $scope.getRegistrationFeeHeadList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.StudentTypeId > 0) {
                $scope.dataLoading = true;
                var saveRegistrationFeeHeadModel = {
                    AcademicSessionId: $scope.AcademicSessionId,
                    StudentTypeId: $scope.StudentTypeId,
                    ClassId: $scope.ClassId,
                }
                adminService.getRegistrationFeeHeadList(saveRegistrationFeeHeadModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.RegistrationFeeHeadList = response.data.RegistrationFeeHeadList;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.RegistrationFeeHeadList = [];
            }
        }

        $scope.saveRegistrationFeeHead = function () {
            var saveRegistrationFeeHeadModel = {
                AcademicSessionId: $scope.AcademicSessionId,
                StudentTypeId: $scope.StudentTypeId,
                ClassId: $scope.ClassId,
                RegistrationFeeHeadList: $scope.RegistrationFeeHeadList
            }

            $scope.dataLoading = true;
            adminService.saveRegistrationFeeHead(saveRegistrationFeeHeadModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getRegistrationFeeHeadList();
                        toastr.success("Data Updated Successfully.")
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
