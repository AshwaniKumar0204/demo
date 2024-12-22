(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRoleSectionController', ManageRoleSectionController);

    ManageRoleSectionController.$inject = ["$scope", "superAdminService"];

    function ManageRoleSectionController($scope, superAdminService) {
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

        $scope.getAllRoleSectionList = function () {
            if ($scope.RoleId == null) {
                $scope.ClassList = [];
                return;
            }
            $scope.dataLoading = true;
            superAdminService.getAllRoleSectionList($scope.RoleId)
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

        $scope.checkSession = function () {
            if (sessionStorage.getItem("RoleId") != null && sessionStorage.getItem("RoleId") != undefined) {
                $scope.RoleId = parseInt(sessionStorage.getItem("RoleId"));
                $scope.getAllRoleSectionList();
                sessionStorage.removeItem("RoleId");
            }
        }
        $scope.checkSession();

        $scope.saveRoleSection = function () {
            var RoleSectionList = [];
            angular.forEach($scope.ClassList, function (taskGroup, key) {
                angular.forEach(taskGroup.RoleSectionList, function (RoleSection, key) {
                    RoleSectionList.push(RoleSection);
                })
            })
            var RoleModel = {
                RoleId: $scope.RoleId,
                RoleSectionList: RoleSectionList
            }
            $scope.dataLoading = true;
            superAdminService.saveRoleSection(RoleModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Role Class Updated Successfully.", "Success");
                        $scope.getAllRoleSectionList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.checkAll = function (taskGroup) {
            angular.forEach(taskGroup.RoleSectionList, function (RoleSection, key) {
                if (taskGroup.CheckAll) {
                    RoleSection.IsSelected = true;
                } else {
                    RoleSection.IsSelected = false;
                }
            })
        }
    }
})();
