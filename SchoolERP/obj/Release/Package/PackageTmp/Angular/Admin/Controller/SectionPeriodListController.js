(function () {
    'use strict';

    angular
        .module('app')
        .controller('SectionPeriodListController', SectionPeriodListController);

    SectionPeriodListController.$inject = ["$scope", "adminService", "ConstantData"];

    function SectionPeriodListController($scope, adminService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.AcademicSessionId = null;
        $scope.SectionPeriodList = [];

        $scope.getSectionPeriodList = function () {
            if ($scope.AcademicSessionId == null) {
                $scope.SectionPeriodList = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getSectionPeriodList($scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SectionPeriodList = response.data.SectionPeriodList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
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
    }
})();
