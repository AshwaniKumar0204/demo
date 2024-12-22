(function () {
    'use strict';

    angular
        .module('app')
        .controller('ClassFeeController', ClassFeeController);

    ClassFeeController.$inject = ["$scope", "adminService"];

    function ClassFeeController($scope, adminService) {
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

        $scope.getClassFeeList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.StudentTypeId > 0) {
                $scope.dataLoading = true;
                var ClassFeeModel = {
                    AcademicSessionId: $scope.AcademicSessionId,
                    StudentTypeId: $scope.StudentTypeId,
                    ClassId: $scope.ClassId,
                }
                adminService.getClassFeeList(ClassFeeModel)
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
                $scope.ClassFeeList = [];
            }
        }
        $scope.getTotalAmount = function (classFeeHeadList) {
            var total = 0;
            for (var i = 0; i < classFeeHeadList.length; i++) {
                var amount = classFeeHeadList[i].ClassFee.Amount || 0;
                total += amount;
            }
            return total;
        };

        $scope.saveClassFee = function () {
            $scope.dataLoading = true;
            adminService.saveClassFee($scope.MonthList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getClassFeeList();
                        toastr.success("Class fee updated successfully.")
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
