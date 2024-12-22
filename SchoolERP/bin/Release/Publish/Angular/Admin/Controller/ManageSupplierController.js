
(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSupplierController', ManageSupplierController);

    ManageSupplierController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageSupplierController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SupplierStatusList = ConstantData.SupplierStatus;
        $scope.SupplierId = null;
        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStateList();

        $scope.getSupplierTypeList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierTypeList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SupplierTypeList = response.data.SupplierTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSupplierTypeList();

        $scope.Supplier = {};
        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(0,0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SupplierList = response.data.SupplierList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSupplierList();

        $scope.$watch('formSupplier.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveSupplier = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SupplierModel = {
                    SupplierId: $scope.Supplier.SupplierId,
                    SupplierStatus: $scope.Supplier.SupplierStatus,
                    SupplierName: $scope.Supplier.SupplierName,
                    MobileNo: $scope.Supplier.MobileNo,
                    Email: $scope.Supplier.Email,
                    Fax: $scope.Supplier.Fax,
                    Address: $scope.Supplier.Address,
                    StateId: $scope.Supplier.StateId,
                    GSTNo: $scope.Supplier.GSTNo,
                    SupplierType: $scope.Supplier.SupplierType,
                }
                console.log(SupplierModel)
                $scope.dataLoading = true;
                adminService.saveSupplier(SupplierModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SupplierModel.SupplierId > 0)
                                toastr.success("Supplier Updated Successfully.");
                            else
                                toastr.success("New Supplier Created Successfully.");
                            $scope.getSupplierList();
                            $('#modal_Student_Charge').modal('hide');
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

        $scope.Supplier = {};
        $scope.Supplier.SupplierStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Supplier = {};
            $scope.Supplier.SupplierStatus = null;
            $scope.formSupplier.$setPristine();
            $scope.formSupplier.$setUntouched();
        }

        $scope.newSupplier = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editSupplier = function (SupplierModel) {
            $scope.resetForm();
            $scope.Supplier = SupplierModel;
            $scope.Supplier.MobileNo = parseInt(SupplierModel.MobileNo);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (SupplierId) {
            $scope.SupplierId = SupplierId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSupplier = function () {
            $scope.dataLoading = true;
            adminService.deleteSupplier($scope.SupplierId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Supplier deleted successfully.", "Deleted Successfully.")
                        $scope.SupplierId = null;
                        $scope.getSupplierList();
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

