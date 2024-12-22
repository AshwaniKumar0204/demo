(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageDeviceController', ManageDeviceController);

    ManageDeviceController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageDeviceController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.DeviceStatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;

        $scope.Device = {};
        $scope.Device.DeviceStatus = null;

        $scope.getDeviceList = function () {
            $scope.dataLoading = true;
            adminService.getDeviceList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DeviceList = response.data.DeviceList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getDeviceList();

        $scope.getDeviceTypeList = function () {
            $scope.dataLoading = true;
            adminService.getDeviceTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DeviceTypeList = response.data.DeviceTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getDeviceTypeList();

        $scope.$watch('formDevice.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveDevice = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var DeviceModel = {
                    DeviceId: $scope.Device.DeviceId,
                    DeviceNo: $scope.Device.DeviceNo,
                    DeviceName: $scope.Device.DeviceName,
                    DeviceStatus: $scope.Device.DeviceStatus,
                    DeviceSerailNo: $scope.Device.DeviceSerailNo,
                    DeviceType: $scope.Device.DeviceType
                }

                $scope.dataLoading = true;
                adminService.saveDevice(DeviceModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (DeviceModel.DeviceId > 0)
                                toastr.success("Device Updated Successfully.");
                            else
                                toastr.success("New Device Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getDeviceList();
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
        $scope.Device = {};
        $scope.Device.DeviceStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Device = {};
            $scope.formDevice.$setPristine();
            $scope.formDevice.$setUntouched();
        }

        $scope.newDevice = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editDevice = function (DeviceModel) {
            $scope.resetForm();
            $scope.Device = DeviceModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (DeviceId) {
            $scope.DeviceId = DeviceId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteDevice = function () {
            $scope.dataLoading = true;
            adminService.deleteDevice($scope.DeviceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Device deleted successfully.", "Deleted Successfully.")
                        $scope.DeviceId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getDeviceList();
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
