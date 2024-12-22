(function () {
    'use strict';

    angular
        .module('app')
        .controller('ClassFeeHeadController', ClassFeeHeadController);

    ClassFeeHeadController.$inject = ["$scope", "adminService"];

    function ClassFeeHeadController($scope, adminService) {
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

        $scope.getClassFeeHeadList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.StudentTypeId > 0) {
                $scope.dataLoading = true;
                var ClassFeeHeadModel = {
                    AcademicSessionId: $scope.AcademicSessionId,
                    StudentTypeId: $scope.StudentTypeId,
                    ClassId: $scope.ClassId,
                }
                adminService.getClassFeeHeadList(ClassFeeHeadModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.MonthList = response.data.MonthList;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.ClassFeeHeadList = [];
            }
        }

        $scope.saveClassFeeHead = function () {
            $scope.dataLoading = true;
            adminService.saveClassFeeHead($scope.MonthList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getClassFeeHeadList();
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
