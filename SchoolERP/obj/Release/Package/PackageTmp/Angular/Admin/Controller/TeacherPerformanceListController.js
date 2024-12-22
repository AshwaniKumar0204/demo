(function () {
    'use strict';

    angular
        .module('app')
        .controller('TeacherPerformanceListController', TeacherPerformanceListController);

    TeacherPerformanceListController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function TeacherPerformanceListController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        //$scope.FromDate = new Date();
        //$scope.ToDate = new Date();

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
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getMonthList = function () {
            $scope.dataLoading = true;
            adminService.getMonthList()
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
        }
        $scope.getMonthList();


        $scope.getTeacherPerformanceParameterList = function (PerformanceId) {
            $scope.dataLoading = true;
            adminService.getTeacherPerformanceParameterList(PerformanceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceParameterList = response.data.PerformanceParameterList;
                        $scope.SubjectPerformanceParameterList = response.data.SubjectPerformanceParameterList;
                        $('#modal_Performance_list').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeacherPerformanceList = function () {
            $scope.dataLoading = true;
            var obj = {
                FromDate: loadDataService.getDateTime($scope.FromDate),
                MonthNo: $scope.MonthNo,
                Year: $scope.Year,
                ToDate: loadDataService.getDateTime($scope.ToDate),
                AcademicSessionId: $scope.AcademicSessionId,
            }
            adminService.getTeacherPerformanceList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceList = response.data.TeacherPerformanceList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeacherAttitudeList = function (TeacherId) {
            $scope.dataLoading = true;
            var obj = {
                MonthNo: $scope.MonthNo,
                AcademicSessionId: $scope.AcademicSessionId,
                TeacherId: TeacherId,
            }
            adminService.getTeacherAttitudeList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TeacherAttitudeList = response.data.TeacherAttitudeList;
                        $('#modal_Performance_list').modal('show');
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

