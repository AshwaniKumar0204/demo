(function () {
    'use strict';

    angular
        .module('app')
        .controller('TeacherFeedbackListController', TeacherFeedbackListController);

    TeacherFeedbackListController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function TeacherFeedbackListController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

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
        
        $scope.getTeacherPerformanceList = function () {
            $scope.dataLoading = true;
            var obj = {
                //FromDate: loadDataService.getDateTime($scope.FromDate),
                //ToDate: loadDataService.getDateTime($scope.ToDate),
                AcademicSessionId: $scope.AcademicSessionId,
                //MonthNo: $scope.MonthNo,
                //Year: $scope.Year,
            }
            adminService.getTeacherFeedbackList(obj)
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

