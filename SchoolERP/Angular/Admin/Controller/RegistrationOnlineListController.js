(function () {
    'use strict';

    angular
        .module('app')
        .controller('RegistrationOnlineListController', RegistrationOnlineListController);

    RegistrationOnlineListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function RegistrationOnlineListController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.RegistrationOnlineStatus = "0";

        $scope.getClassStreamTypeList = function () {
            $scope.SubjectOptionOne = null;
            $scope.SubjectOptionTwo = null;
            $scope.ClassStreamTypeId = null;
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.ClassNo)
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

        //$scope.changeSubjectOptionOne = function () {
        //    $scope.SubjectOptionTwoList = [];
        //    if ($scope.SubjectOptionOne > 0) {
        //        angular.forEach($scope.SubjectOptionOneList, function (subjectOption, key1) {
        //            if (subjectOption.SubjectOptionId != $scope.SubjectOptionOne) {
        //                $scope.SubjectOptionTwoList.push(subjectOption);
        //            }
        //        });
        //    }
        //}

        $scope.getSubjectOptionList = function () {
            if ($scope.ClassStreamTypeId == null || $scope.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.ClassNo, $scope.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        //$scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            //$scope.changeSubjectOptionOne();
        }

        $scope.getAcademicSessionList = function () {
            var obj = {
                ClassNo: $scope.ClassNo,
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

        $scope.getRegistrationFormList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationFormList(0, $scope.AcademicSessionId, $scope.ClassNo)
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
        $scope.getRegistrationFormList();

        $scope.changeRegistrationForm = function () {
            $scope.SelectedRegistrationForm = {};
            for (var i = 0; i < $scope.RegistrationFormList.length; i++) {
                if ($scope.RegistrationFormId == $scope.RegistrationFormList[i].RegistrationFormId) {
                    $scope.SelectedRegistrationForm = $scope.RegistrationFormList[i];
                    break;
                }
            }
        }

        $scope.getRegistrationOnlineList = function () {
            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                RegistrationFormId: $scope.RegistrationFormId,
                RegistrationOnlineStatus: $scope.RegistrationOnlineStatus
            }
            $scope.dataLoading = true;
            adminService.getRegistrationOnlineList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                        //$scope.RegistrationOnlineList = [];

                       
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
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
