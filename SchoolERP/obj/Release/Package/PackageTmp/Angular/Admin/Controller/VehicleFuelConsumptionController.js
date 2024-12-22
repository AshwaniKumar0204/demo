(function () {
    'use strict';

    angular
        .module('app')
        .controller('VehicleFuelConsumptionController', VehicleFuelConsumptionController);

    VehicleFuelConsumptionController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function VehicleFuelConsumptionController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.VehicleFuelConsumption = {};
        $scope.VehicleFuelConsumption.BillDate = new Date();

        $scope.getBusList = function () {
            $scope.dataLoading = true;
            adminService.getBusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BusList = response.data.Buses;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBusList();

        $scope.getVehicleFuelConsumptionList = function () {
            $scope.dataLoading = true;
            var fileterModel = {
                BusId: $scope.SelectedBus.BusId
            }
            adminService.getVehicleFuelConsumptionList(fileterModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleFuelConsumptionList = response.data.VehicleFuelConsumptionList;
                        $scope.PreviousKillometers = response.data.PreviousKillometers;
                        $scope.TotalQuantity = 0;
                        $scope.TotalAmount = 0;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.afterBusSelected = function (selected) {
            if (selected != undefined) {
                $scope.VehicleFuelConsumption.BusId = selected.originalObject.BusId;
                $scope.SelectedBus = selected.originalObject;
                $scope.getVehicleFuelConsumptionList();
            } else {
                $scope.VehicleFuelConsumption.BusId = null;
                $scope.SelectedBus = {};
            }
        }

        $scope.$watch('formVehicleFuelConsumption.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(1,3)
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

        $scope.afterSupplierSelected = function (obj) {
            if (obj != null && obj != undefined) {
                $scope.VehicleFuelConsumption.SupplierId = obj.originalObject.SupplierId;
            } else {
                $scope.VehicleFuelConsumption.SupplierId = null;
            }
        }

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

        $scope.saveVehicleFuelConsumption = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var supplierName = document.getElementById('SupplierAutoComplete_value').value;
                if (supplierName == undefined || supplierName == null || supplierName == '') {
                    toastr.error("Party name is required.");
                    return;
                }
                var VehicleFuelConsumptionModel = {
                    BusId: $scope.SelectedBus.BusId,
                    VehicleFuelConsumptionId: $scope.VehicleFuelConsumption.VehicleFuelConsumptionId,
                    SupplierId: $scope.VehicleFuelConsumption.SupplierId,
                    BillNo: $scope.VehicleFuelConsumption.BillNo,
                    Material: $scope.VehicleFuelConsumption.Material,
                    UnitId: $scope.VehicleFuelConsumption.UnitId,
                    PreviousKillometeres: $scope.VehicleFuelConsumption.PreviousKillometeres,
                    CurrentKillometers: $scope.VehicleFuelConsumption.CurrentKillometers,
                    Quantity: $scope.VehicleFuelConsumption.Quantity,
                    //Average: $scope.VehicleFuelConsumption.Average,
                    Rate: $scope.VehicleFuelConsumption.Rate,
                    Amount: $scope.VehicleFuelConsumption.Amount,
                    Remarks: $scope.VehicleFuelConsumption.Remarks,
                    BillDate: loadDataService.getDateTime($scope.VehicleFuelConsumption.BillDate),
                    Supplier: {
                        SupplierName: supplierName
                    }
                }

                $scope.dataLoading = true;
                adminService.saveVehicleFuelConsumption(VehicleFuelConsumptionModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (VehicleFuelConsumptionModel.VehicleFuelConsumptionId > 0)
                                toastr.success("Fuel Consumption Detail Updated Successfully.");
                            else
                                toastr.success("Fuel Consumption Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Bus_Charge').modal('hide');
                            $scope.getVehicleFuelConsumptionList();
                            $scope.getSupplierList();
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
            $scope.VehicleFuelConsumption = [];
            $scope.VehicleFuelConsumption.BillDate = new Date();
            $scope.formVehicleFuelConsumption.$setPristine();
            $scope.formVehicleFuelConsumption.$setUntouched();
        }

        $scope.newVehicleFuelConsumption = function () {
            $scope.resetForm();
            $scope.VehicleFuelConsumption.PreviousKillometeres = parseFloat($scope.PreviousKillometers);
            $('#modal_Bus_Charge').modal('show');
        }

        $scope.editVehicleFuelConsumption = function (VehicleFuelConsumptionModel) {
            $scope.resetForm();
            $scope.VehicleFuelConsumption = VehicleFuelConsumptionModel;
            $scope.VehicleFuelConsumption.BillDate = new Date(VehicleFuelConsumptionModel.BillDate);
            $scope.VehicleFuelConsumption.PreviousKillometeres = parseFloat(VehicleFuelConsumptionModel.PreviousKillometeres);
            $scope.VehicleFuelConsumption.Amount = parseFloat(VehicleFuelConsumptionModel.Amount);
            $scope.VehicleFuelConsumption.Quantity = parseFloat(VehicleFuelConsumptionModel.Quantity);
            $scope.VehicleFuelConsumption.CurrentKillometers = parseFloat(VehicleFuelConsumptionModel.CurrentKillometers);
            $scope.VehicleFuelConsumption.Rate = parseFloat(VehicleFuelConsumptionModel.Rate);
            document.getElementById('SupplierAutoComplete_value').value = $scope.VehicleFuelConsumption.SupplierName;
            $('#modal_Bus_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (VehicleFuelConsumptionId) {
            $scope.VehicleFuelConsumptionId = VehicleFuelConsumptionId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteVehicleFuelConsumption = function () {
            $scope.dataLoading = true;
            adminService.deleteVehicleFuelConsumption($scope.VehicleFuelConsumptionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Fuel Consumption deleted successfully.", "Deleted Successfully.")
                        $scope.VehicleFuelConsumptionId = null;
                        $scope.getVehicleFuelConsumptionList();
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

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions - 1];
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
