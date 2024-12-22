(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageMenuController', ManageMenuController);

    ManageMenuController.$inject = ["$scope", "superAdminService"];

    function ManageMenuController($scope, superAdminService) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Menu = {};
        $scope.getMenuList = function () {
            $scope.dataLoading = true;
            superAdminService.getMenuList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MenuList = response.data.MenuList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMenuList();

        $scope.getTaskMasterListForMenu = function () {
            $scope.dataLoading = true;
            superAdminService.getTaskMasterListForMenu()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TaskMasterList = response.data.TaskMasterList;
                        var index = { TaskMasterId: null, ActionName: "--Select Task--" };
                        $scope.TaskMasterList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getTaskMasterListForMenu();


        $scope.$watch('formMenu.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveMenu = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var MenuModel = {
                    MenuId: $scope.Menu.MenuId,
                    MenuTitle: $scope.Menu.MenuTitle,
                    IconClass: $scope.Menu.IconClass,
                    ParentMenuId: $scope.Menu.ParentMenuId,
                    MenuPosition: $scope.Menu.MenuPosition,
                    TaskMasterId: $scope.Menu.TaskMasterId
                }

                $scope.dataLoading = true;
                superAdminService.saveMenu(MenuModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (MenuModel.MenuId > 0)
                                toastr.success("Menu Detail Updated Successfully.");
                            else
                                toastr.success("Menu Created Successfully.");
                            $scope.resetForm();
                            $scope.MenuList = response.data.MenuList;
                            $scope.getTaskMasterListForMenu();
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
            $scope.Menu = [];
            $scope.Menu.TaskMasterId = null;
            $scope.formMenu.$setPristine();
            $scope.formMenu.$setUntouched();
        }

        $scope.newMenu = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.newChildMenu = function (Menu) {
            $scope.resetForm();
            $scope.Menu.ParentMenuId = Menu.MenuId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editMenu = function (MenuModel) {
            $scope.Menu = MenuModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (MenuId) {
            $scope.MenuId = MenuId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteMenu = function () {
            $scope.dataLoading = true;
            superAdminService.deleteMenu($scope.MenuId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Menu deleted successfully.", "Deleted Successfully.")
                        $scope.MenuList = response.data.MenuList;
                        $scope.MenuId = null;
                        $scope.getTaskMasterListForMenu();
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

        $scope.menuUp = function (MenuId) {
            $scope.dataLoading = true;
            superAdminService.menuUp(MenuId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MenuList = response.data.MenuList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.menuDown = function (MenuId) {
            $scope.dataLoading = true;
            superAdminService.menuDown(MenuId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MenuList = response.data.MenuList;
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
})();
