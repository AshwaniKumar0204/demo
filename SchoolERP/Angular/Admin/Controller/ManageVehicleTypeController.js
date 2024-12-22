(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageVehicleTypeController', ManageVehicleTypeController);

    ManageVehicleTypeController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageVehicleTypeController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.VehicleTypeStatusList = ConstantData.VehicleTypeStatus;

        $scope.VehicleType = {};
        $scope.VehicleType.VehicleTypeStatus = null;

        $scope.getVehicleTypeList = function () {
            $scope.dataLoading = true;
            adminService.getVehicleTypeList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleTypeList = response.data.VehicleTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getVehicleTypeList();

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

        $scope.$watch('formVehicleType.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveVehicleType = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var VehicleTypeModel = {
                    VehicleTypeId: $scope.VehicleType.VehicleTypeId,
                    VehicleTypeStatus: $scope.VehicleType.VehicleTypeStatus,
                    VehicleTypeName: $scope.VehicleType.VehicleTypeName,
                    ServiceDuration: $scope.VehicleType.ServiceDuration,
                }

                $scope.dataLoading = true;
                adminService.saveVehicleType(VehicleTypeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (VehicleTypeModel.VehicleTypeId > 0)
                                toastr.success("Vehicle Type Updated Successfully.");
                            else
                                toastr.success("New Vehicle Type Created Successfully.");
                            $scope.resetForm();
                            $scope.getVehicleTypeList();
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
        $scope.VehicleType = {};
        $scope.VehicleType.VehicleTypeStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.VehicleType.VehicleTypeId = null;
            $scope.VehicleType.VehicleTypeName = "";
            $scope.VehicleType.ServiceDuration = null;
            $scope.formVehicleType.$setPristine();
            $scope.formVehicleType.$setUntouched();
        }

        $scope.newVehicleType = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editVehicleType = function (VehicleTypeModel) {
            $scope.resetForm();
            $scope.VehicleType.VehicleTypeName = VehicleTypeModel.VehicleTypeName;
            $scope.VehicleType.VehicleTypeId = VehicleTypeModel.VehicleTypeId;
            $scope.VehicleType.VehicleTypeStatus = VehicleTypeModel.VehicleTypeStatus;
            $scope.VehicleType.ServiceDuration = parseFloat(VehicleTypeModel.ServiceDuration);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (VehicleTypeId) {
            $scope.VehicleTypeId = VehicleTypeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteVehicleType = function () {
            $scope.dataLoading = true;
            adminService.deleteVehicleType($scope.VehicleTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Vehicle Type deleted successfully.", "Deleted Successfully.")
                        $scope.getVehicleTypeList();
                        $scope.VehicleTypeId = null;
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
