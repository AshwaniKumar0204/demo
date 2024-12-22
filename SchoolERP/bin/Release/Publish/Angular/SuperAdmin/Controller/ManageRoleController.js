(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRoleController', ManageRoleController);

    ManageRoleController.$inject = ["$scope", "superAdminService", "ConstantData","$location"];

    function ManageRoleController($scope, superAdminService, ConstantData, $location) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Role = {};
        $scope.RoleStatusList = ConstantData.RoleStatus;

        $scope.getRoleList = function () {
            $scope.dataLoading = true;
            superAdminService.getRoleList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RoleList = response.data.RoleList;
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

        $scope.$watch('formRole.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveRole = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var RoleModel = {
                    RoleId: $scope.Role.RoleId,
                    RoleStatus: $scope.Role.RoleStatus,
                    RoleTitle: $scope.Role.RoleTitle
                }

                $scope.dataLoading = true;
                superAdminService.saveRole(RoleModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (RoleModel.RoleId > 0)
                                toastr.success("Role Detail Updated Successfully.");
                            else
                                toastr.success("Role Created Successfully.");
                            $scope.resetForm();
                            $scope.RoleList = response.data.RoleList;
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Role = [];
            $scope.Role.RoleStatus = null;
            $scope.formRole.$setPristine();
            $scope.formRole.$setUntouched();
        }

        $scope.newRole = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editRole = function (RoleModel) {
            $scope.Role.RoleId = RoleModel.RoleId;
            $scope.Role.RoleTitle = RoleModel.RoleTitle;
            $scope.Role.RoleStatus = RoleModel.RoleStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (RoleId) {
            $scope.RoleId = RoleId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteRole = function () {
            $scope.dataLoading = true;
            superAdminService.deleteRole($scope.RoleId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Role deleted successfully.", "Deleted Successfully.")
                        $scope.RoleList = response.data.RoleList;
                        $scope.RoleId = null;
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.viewTasks = function (RoleId) {
            sessionStorage.setItem("RoleId", RoleId);
            $location.path("/ManageRoleTask");
        }
    }
})();
