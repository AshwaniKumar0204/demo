(function () {
    'use strict';

    angular
        .module('app')
        .controller('BusController', BusController);

    BusController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function BusController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.BusStatusList = ConstantData.BusStatus;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formBus.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.GetBusList = function () {
            $scope.dataLoading = true;
            adminService.getBusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.buses = response.data.Buses;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.GetBusList();

        $scope.GetVehicleTypeList = function () {
            $scope.dataLoading = true;
            adminService.getVehicleTypeList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleTypeList = response.data.VehicleTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.GetVehicleTypeList();

        $scope.editBus = function (model) {
            $scope.BusTitle = model.BusTitle;
            $scope.Description = model.Description;
            $scope.Status = model.Status.toString();
            $scope.BusId = model.BusId;
            $scope.VehicleTypeId = model.VehicleTypeId;
        }

        $scope.SaveBus = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var bus = {
                    BusTitle: $scope.BusTitle,
                    Status: $scope.Status,
                    Description: $scope.Description,
                    BusId: $scope.BusId,
                    VehicleTypeId: $scope.VehicleTypeId,
                }
                $scope.dataLoading = true;
                adminService.saveBus(bus)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Bus Details Saved Successfully!");
                            $scope.GetBusList();

                            $scope.Description = "";
                            $scope.BusTitle = "";
                            $scope.BusId = 0;
                            $scope.VehicleTypeId = null;
                            $scope.IsSubmitted = false;
                            $scope.formBus.$setPristine();
                            $scope.formBus.$setUntouched();
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all required fields!", "Validation Error");
            }
        }
    }
})();
