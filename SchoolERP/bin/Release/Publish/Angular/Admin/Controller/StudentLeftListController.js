(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentLeftListController', StudentLeftListController);

    StudentLeftListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function StudentLeftListController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.LeftStudentTypeList = ConstantData.LeftStudentType;

        $scope.getLeftStudentList = function () {
            $scope.dataLoading = true;
            adminService.getLeftStudentList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.LeftStudentList = response.data.LeftStudentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getLeftStudentList();

        $scope.rejoinLeftStudentConfirmation = function (LeftStudentId) {
            $scope.LeftStudentId = LeftStudentId;
            $('#modal_confirmation').modal('show');
        }

        $scope.rejoinLeftStudent = function () {
            $scope.dataLoading = true;
            adminService.rejoinLeftStudent($scope.LeftStudentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getLeftStudentList();
                        $scope.LeftStudentId = null;
                        toastr.success("Rejoined Successfully.");
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }


    }
})();
