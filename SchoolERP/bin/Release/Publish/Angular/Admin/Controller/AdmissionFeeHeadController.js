(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdmissionFeeHeadController', AdmissionFeeHeadController);

    AdmissionFeeHeadController.$inject = ["$scope", "adminService","ConstantData"];

    function AdmissionFeeHeadController($scope, adminService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.MonthList = [];
        $scope.AcademicSessionId = null;
        $scope.ClassId = null;
        $scope.StudentTypeId = null;
        $scope.AdmissionType = null;
        $scope.AdmissionTypeList = ConstantData.AdmissionType;

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
                        var index = { ClassId: null, ClassName: '--Select Course--' };
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

        $scope.getAdmissionFeeHeadList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0 && $scope.StudentTypeId > 0 && $scope.AdmissionType > 0) {
                $scope.dataLoading = true;
                var saveAdmissionFeeHeadModel = {
                    AcademicSessionId: $scope.AcademicSessionId,
                    StudentTypeId: $scope.StudentTypeId,
                    ClassId: $scope.ClassId,
                    AdmissionType: $scope.AdmissionType,
                }
                adminService.getAdmissionFeeHeadList(saveAdmissionFeeHeadModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.AdmissionFeeHeadList = response.data.AdmissionFeeHeadList;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.AdmissionFeeHeadList = [];
            }
        }

        $scope.saveAdmissionFeeHead = function () {
            var saveAdmissionFeeHeadModel = {
                AcademicSessionId: $scope.AcademicSessionId,
                StudentTypeId: $scope.StudentTypeId,
                ClassId: $scope.ClassId,
                AdmissionType: $scope.AdmissionType,
                AdmissionFeeHeadList: $scope.AdmissionFeeHeadList
            }

            $scope.dataLoading = true;
            adminService.saveAdmissionFeeHead(saveAdmissionFeeHeadModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getAdmissionFeeHeadList();
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
