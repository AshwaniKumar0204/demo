(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageVehicleRunningRecordController', ManageVehicleRunningRecordController);

    ManageVehicleRunningRecordController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function ManageVehicleRunningRecordController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.VehicleRunningRecord = {};
        $scope.getVehicleRunningRecordList = function () {
            var filterModel = {
                BusId: $scope.VehicleRunningRecord.BusId,
            }
            $scope.dataLoading = true;
            adminService.getVehicleRunningRecordList(filterModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleRunningRecordList = response.data.VehicleRunningRecordList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('formVehicleRunningRecord.$valid', function (value) {
            $scope.IsFormValid = value;
        })

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

        $scope.afterBusSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedBus = selected.originalObject;
                $scope.getVehicleRunningRecordList();
            } else {
                $scope.SelectedBus = {};
            }
        }

        $scope.saveVehicleRunningRecord = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var VehicleRunningRecordModel = {
                    VehicleRunningRecordId: $scope.VehicleRunningRecord.VehicleRunningRecordId,
                    BusId: $scope.SelectedBus.BusId,
                    EntryDate: loadDataService.getDateTime($scope.VehicleRunningRecord.EntryDate),
                    MeterReading: $scope.VehicleRunningRecord.MeterReading,
                }

                $scope.dataLoading = true;
                adminService.saveVehicleRunningRecord(VehicleRunningRecordModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (VehicleRunningRecordModel.VehicleRunningRecordId > 0)
                                toastr.success("Vehicle Running Record Updated Successfully.");
                            else
                                toastr.success("New Vehicle Running Record Created Successfully.");
                            $scope.resetForm();
                            $scope.getVehicleRunningRecordList();
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.VehicleRunningRecord = {};
            $scope.VehicleRunningRecord.EntryDate = new Date();
            $scope.formVehicleRunningRecord.$setPristine();
            $scope.formVehicleRunningRecord.$setUntouched();
        }

        $scope.newVehicleRunningRecord = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editVehicleRunningRecord = function (VehicleRunningRecordModel) {
            $scope.resetForm();
            $scope.VehicleRunningRecord = VehicleRunningRecordModel;
            $scope.VehicleRunningRecord.EntryDate = new Date(VehicleRunningRecordModel.EntryDate);
            $scope.VehicleRunningRecord.MeterReading = parseFloat(VehicleRunningRecordModel.MeterReading);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (VehicleRunningRecordId) {
            $scope.VehicleRunningRecordId = VehicleRunningRecordId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteVehicleRunningRecord = function () {
            $scope.dataLoading = true;
            adminService.deleteVehicleRunningRecord($scope.VehicleRunningRecordId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Vehicle Running Record deleted successfully.", "Deleted Successfully.")
                        $scope.getVehicleRunningRecordList();
                        $scope.VehicleRunningRecordId = null;
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
