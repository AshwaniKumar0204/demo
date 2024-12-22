(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageItemTypeController', ManageItemTypeController);

    ManageItemTypeController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageItemTypeController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ItemTypeStatusList = ConstantData.ItemTypeStatus;

        $scope.ItemType = {};
        $scope.ItemType.ItemTypeStatus = null;

        $scope.getItemTypeList = function () {
            $scope.dataLoading = true;
            adminService.getItemTypeList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ItemTypeList = response.data.ItemTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getItemTypeList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
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

        $scope.$watch('formItemType.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveItemType = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var ItemTypeModel = {
                    ItemTypeId: $scope.ItemType.ItemTypeId,
                    ItemTypeStatus: $scope.ItemType.ItemTypeStatus,
                    ItemTypeName: $scope.ItemType.ItemTypeName,
                }

                $scope.dataLoading = true;
                adminService.saveItemType(ItemTypeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (ItemTypeModel.ItemTypeId > 0)
                                toastr.success("Item Type Updated Successfully.");
                            else
                                toastr.success("New Item Type Created Successfully.");
                            $scope.resetForm();
                            $scope.ItemTypeList = response.data.ItemTypeList;
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
        $scope.ItemType = {};
        $scope.ItemType.ItemTypeStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.ItemType.ItemTypeId = null;
            $scope.ItemType.ItemTypeName = "";
            $scope.formItemType.$setPristine();
            $scope.formItemType.$setUntouched();
        }

        $scope.newItemType = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editItemType = function (ItemTypeModel) {
            $scope.resetForm();
            $scope.ItemType.ItemTypeName = ItemTypeModel.ItemTypeName;
            $scope.ItemType.ItemTypeId = ItemTypeModel.ItemTypeId;
            $scope.ItemType.ItemTypeStatus = ItemTypeModel.ItemTypeStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (ItemTypeId) {
            $scope.ItemTypeId = ItemTypeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteItemType = function () {
            $scope.dataLoading = true;
            adminService.deleteItemType($scope.ItemTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Item Type deleted successfully.", "Deleted Successfully.")
                        $scope.ItemTypeList = response.data.ItemTypeList;
                        $scope.ItemTypeId = null;
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
    }
})();
