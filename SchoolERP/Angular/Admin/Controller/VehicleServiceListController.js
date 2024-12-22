(function () {
    'use strict';

    angular
        .module('app')
        .controller('VehicleServiceListController', VehicleServiceListController);

    VehicleServiceListController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function VehicleServiceListController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.VehicleService = {};
        $scope.VehicleService.BillDate = new Date();

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

        $scope.getVehicleServiceList = function () {
            $scope.dataLoading = true;
            var fileterModel = {
                BusId: $scope.BusId,
                SupplierId: $scope.SupplierId,
                FromDate: loadDataService.getDateTime($scope.FromDate),
                ToDate: loadDataService.getDateTime($scope.ToDate),
            }
            adminService.getVehicleServiceList(fileterModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleServiceList = response.data.VehicleServiceList;
                        $scope.TotalBillAmount = 0;
                        $scope.TotalDiscount = 0;
                        $scope.TotalPaidAmount = 0;
                        angular.forEach($scope.VehicleServiceList, function (veh, key1) {
                            $scope.TotalBillAmount += veh.BillAmount;
                            $scope.TotalDiscount += veh.Discount;
                            $scope.TotalPaidAmount += veh.PaidAmount;
                        })
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
                $scope.BusId = selected.originalObject.BusId;
            } else {
                $scope.BusId = null;
            }
        }

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
                $scope.SupplierId = obj.originalObject.SupplierId;
            } else {
                $scope.SupplierId = null;
            }
        }


        $scope.getVehicleServiceDetailList = function (VehicleService) {
            $scope.VehicleService = VehicleService;
            $scope.dataLoading = true;
            adminService.getVehicleServiceDetailList(VehicleService.VehicleServiceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleServiceDetailList = response.data.VehicleServiceDetailList;
                        $('#modal_VehicleService_list').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
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
