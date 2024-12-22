(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRoleTaskController', ManageRoleTaskController);

    ManageRoleTaskController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData"];

    function ManageRoleTaskController($scope, superAdminService, loadDataService, ConstantData) {
        $scope.checkSystemLogin();
        $scope.RoleId = null;
        $scope.getRoleList = function () {
            $scope.dataLoading = true;
            superAdminService.getRoleList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RoleList = response.data.RoleList;
                        var index = { RoleId: null, RoleTitle: "--Select Role--" };
                        $scope.RoleList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRoleList();

        $scope.getAllRoleTaskList = function () {
            if ($scope.RoleId == null) {
                $scope.TaskGroupList = [];
                return;
            }
            $scope.dataLoading = true;
            superAdminService.getAllRoleTaskList(0, $scope.RoleId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TaskGroupList = response.data.TaskGroupList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkSession = function () {
            if (sessionStorage.getItem("RoleId") != null && sessionStorage.getItem("RoleId") != undefined) {
                $scope.RoleId = parseInt(sessionStorage.getItem("RoleId"));
                $scope.getAllRoleTaskList();
                sessionStorage.removeItem("RoleId");
            }
        }
        $scope.checkSession();

        $scope.saveRoleTask = function () {
            var RoleTaskList = [];
            angular.forEach($scope.TaskGroupList, function (taskGroup, key) {
                angular.forEach(taskGroup.RoleTaskList, function (roleTask, key) {
                    RoleTaskList.push(roleTask);
                })
            })
            var RoleModel = {
                RoleId: $scope.RoleId,
                RoleTaskList: RoleTaskList
            }
            $scope.dataLoading = true;
            superAdminService.saveRoleTask(RoleModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Role Updated Successfully.", "Success");
                       // $scope.getAllRoleTaskList();
                        $scope.TaskGroupList = [];
                        $scope.RoleId = null;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkAll = function (taskGroup) {
            angular.forEach(taskGroup.RoleTaskList, function (roleTask, key) {
                if (taskGroup.CheckAll) {
                    roleTask.IsSelected = true;
                    roleTask.CanEdit = true;
                    roleTask.CanDelete = true;
                } else {
                    roleTask.IsSelected = false;
                    roleTask.CanEdit = false;
                    roleTask.CanDelete = false;
                }
            })
        }
    }
})();
