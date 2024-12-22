(function () {
    'use strict';

    angular
        .module('app')
        .controller('ParentFeedBackListController', ParentFeedBackListController);

    ParentFeedBackListController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ParentFeedBackListController($scope, adminService, ConstantData, Factory, loadDataService) {
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

        //$scope.getPerformanceYearList = function () {
        //    $scope.dataLoading = true;
        //    adminService.getPerformanceYearList()
        //        .then(function (response) {
        //            if (response.data.Message == 'Success') {
        //                $scope.YearList = response.data.YearList;
        //            } else {
        //                toastr.error(response.data.Message);
        //            }
        //            $scope.dataLoading = false;
        //        }, function (err) {
        //            toastr.error("Some error occured while fetching data");
        //            $scope.dataLoading = false;
        //        })
        //}
        //$scope.getPerformanceYearList();

        $scope.getParentPerformanceParameterList = function (PerformanceId) {
            $scope.dataLoading = true;
            adminService.getParentPerformanceParameterList(PerformanceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceParameterList = response.data.PerformanceParameterList;
                        $scope.TeacherAttitudeList = response.data.TeacherAttitudeList;
                        console.log(response.data);
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

        $scope.getParentPerformanceList = function () {
            $scope.dataLoading = true;
            var obj = {
                FromDate: loadDataService.getDateTime($scope.FromDate),
                ToDate: loadDataService.getDateTime($scope.ToDate),
                AcademicSessionId: $scope.AcademicSessionId,
                MonthNo: $scope.MonthNo,
                Year: $scope.Year,
            }
            adminService.getParentPerformanceList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceList = response.data.PerformanceList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        //$scope.getParentPerformanceList();

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions - 1];
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

