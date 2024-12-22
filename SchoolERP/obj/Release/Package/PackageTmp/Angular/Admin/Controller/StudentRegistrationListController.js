(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentRegistrationListController', StudentRegistrationListController);

    StudentRegistrationListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function StudentRegistrationListController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.GenderList = ConstantData.Gender;
        $scope.ClassId = 0;
        $scope.StudentTypeId = 0;

        $scope.getAcademicSessionList = function () {
            $scope.StudentRegistrationList = [];

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

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: 0, ClassName: '--All Classes--' };
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

        $scope.getStudentTypeList = function () {
            $scope.dataLoading = true;
            adminService.getStudentTypeList(ConstantData.StudentTypeStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentTypeList = response.data.StudentTypeList;
                        var index = { StudentTypeId: 0, TypeName: '--Select All Type--' };
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

        $scope.filterList = function () {
            $scope.StudentRegistrationList = [];
            angular.forEach($scope.MainStudentRegistrationList, function (StudentRegistration, key) {
                if ($scope.ClassId > 0 && $scope.StudentTypeId > 0) {
                    if ($scope.ClassId == StudentRegistration.ClassId && $scope.StudentTypeId == StudentRegistration.StudentTypeId) {
                        $scope.StudentRegistrationList.push(StudentRegistration);
                    }
                } else if ($scope.ClassId > 0) {
                    if ($scope.ClassId == StudentRegistration.ClassId) {
                        $scope.StudentRegistrationList.push(StudentRegistration);
                    }
                } else if ($scope.StudentTypeId > 0) {
                    if ($scope.StudentTypeId == StudentRegistration.StudentTypeId) {
                        $scope.StudentRegistrationList.push(StudentRegistration);
                    }
                } else {
                    $scope.StudentRegistrationList.push(StudentRegistration);
                }
            });
        }

        $scope.getStudentRegistrationList = function () {
            if ($scope.AcademicSessionId != null || $scope.AcademicSessionId != undefined || $scope.AcademicSessionId > 0) {
                $scope.dataLoading = true;

                adminService.getStudentRegistrationList($scope.AcademicSessionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.MainStudentRegistrationList = response.data.StudentRegistrationList;
                            $scope.filterList();
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

        $scope.editStudentDetail = function (StudentRegistrationId) {
            sessionStorage.setItem('editStudentRegistrationId', StudentRegistrationId);
            sessionStorage.setItem('backURL', 'StudentRegistrationList');
            $location.path('/StudentDetailUpdate')
        }

        $scope.takeAdmission = function (StudentRegistrationModel) {
            sessionStorage.setItem('studentRegistrationDetail', JSON.stringify(StudentRegistrationModel));
            $location.path('/NewAdmission');
        }

        $scope.editStudentRegistration = function (StudentRegistrationModel) {
            sessionStorage.setItem('editStudentRegistrationDetail', JSON.stringify(StudentRegistrationModel));
            $location.path('/StudentRegistrationUpdate');
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
