(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStoreItemController', ManageStoreItemController);

    ManageStoreItemController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStoreItemController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StoreItemStatusList = ConstantData.StoreItemStatus;
        $scope.StoreItem = {};

        $scope.getUnitList = function () {
            $scope.dataLoading = true;
            adminService.getUnitList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.UnitList = response.data.UnitList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getUnitList();

        $scope.getStoreItemList = function () {
            $scope.dataLoading = true;
            adminService.getStoreItemList(0, $scope.ItemTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StoreItemList = response.data.StoreItemList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStoreItemList();

        $scope.getItemTypeList = function () {
            $scope.dataLoading = true;
            adminService.getItemTypeList(1)
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

        $scope.$watch('formStoreItem.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStoreItem = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StoreItemModel = {
                    StoreItemId: $scope.StoreItem.StoreItemId,
                    ItemName: $scope.StoreItem.ItemName,
                    ItemCode: $scope.StoreItem.ItemCode,
                    ItemTypeId: $scope.StoreItem.ItemTypeId,
                    UnitId: $scope.StoreItem.UnitId,
                    Quantity: $scope.StoreItem.Quantity,
                    StoreItemStatus: $scope.StoreItem.StoreItemStatus,
                }

                $scope.dataLoading = true;
                adminService.saveStoreItem(StoreItemModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StoreItemModel.StoreItemId > 0)
                                toastr.success("Item Updated Successfully.");
                            else
                                toastr.success("New Item Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStoreItemList();
                            //$scope.StoreItemList = response.data.StoreItemList;
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
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
            $scope.StoreItem = {};
            $scope.StoreItem.StoreItemStatus = null;
            $scope.formStoreItem.$setPristine();
            $scope.formStoreItem.$setUntouched();
        }

        $scope.newStoreItem = function () {
            $scope.resetForm();
            $scope.StoreItem.ItemTypeId = $scope.ItemTypeId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStoreItem = function (StoreItemModel) {
            $scope.resetForm();
            $scope.StoreItem = StoreItemModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StoreItemId) {
            $scope.StoreItemId = StoreItemId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStoreItem = function () {
            $scope.dataLoading = true;
            adminService.deleteStoreItem($scope.StoreItemId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Item deleted successfully.", "Deleted Successfully.")
                        $scope.StoreItemId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStoreItemList();
                        //$scope.StoreItemList = response.data.StoreItemList;
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
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
