(function () {
    'use strict';

    angular
        .module('app')
        .controller('VehicleServiceController', VehicleServiceController);

    VehicleServiceController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService", "$routeParams"];

    function VehicleServiceController($scope, adminService, ConstantData, Factory, loadDataService, $routeParams) {
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
        $scope.VehicleService.VehicleServiceDetails = [];
        $scope.VehicleServiceDetail = {};

        $scope.getVehicleServiceTypeList = function () {
            $scope.dataLoading = true;
            adminService.getVehicleServiceTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleServiceTypeList = response.data.VehicleServiceTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getVehicleServiceTypeList();

        $scope.getVehicleServiceDetail = function (VehicleServiceId) {
            $scope.dataLoading = true;
            adminService.getVehicleServiceDetail(VehicleServiceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleService = response.data.VehicleService;
                        $scope.VehicleService.BillDate = new Date($scope.VehicleService.BillDate);
                        document.getElementById('BusAutoComplete_value').value = $scope.VehicleService.BusTitle;
                        document.getElementById('SupplierAutoComplete_value').value = $scope.VehicleService.SupplierName;
                        document.getElementById('BillAutoComplete_value').value = $scope.VehicleService.BillNo;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.init = function () {
            if ($routeParams.id > 0) {
                $scope.getVehicleServiceDetail($routeParams.id);
            }
        }
        $scope.init();

        //Bill
        $scope.getVehicleServiceList = function () {
            $scope.dataLoading = true;
            var fileterModel = {
                BusId: null,
                SupplierId: null,
                FromDate: loadDataService.getDateTime($scope.FromDate),
                ToDate: loadDataService.getDateTime($scope.ToDate),
            }
            adminService.getVehicleServiceList(fileterModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.VehicleServiceList = response.data.VehicleServiceList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getVehicleServiceList();

        $scope.afterBillSelected = function (selected) {
            if (selected != undefined) {
                $scope.getVehicleServiceDetail(selected.originalObject.VehicleServiceId);
            } else {
                $scope.VehicleService
            }
        }

        //Bus
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
                $scope.VehicleService.BusId = selected.originalObject.BusId;
            } else {
                $scope.VehicleService.BusId = null;
            }
        }

        //Supplier
        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(1, 3)
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
                $scope.VehicleServiceDetail.SupplierId = obj.originalObject.SupplierId;
            } else {
                $scope.VehicleServiceDetail.SupplierId = null;
            }
        }

        //JobType
        $scope.getJobTypeList = function () {
            $scope.dataLoading = true;
            adminService.getJobTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.JobTypeList = response.data.JobTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getJobTypeList();

        $scope.afterJobTypeSelected = function (obj) {
            if (obj != null & obj != undefined) {
                $scope.VehicleServiceDetail.JobType = obj.originalObject.JobType;
            } else {
                $scope.VehicleServiceDetail.JobType = null;
            }
        }

        //Vehicle Service Detail
        $scope.$watch('formVehicleServiceDetail.$valid', function (value) {
            $scope.IsFormVehicleServiceDetailValid = value;
        })

        $scope.resetVehicleServiceDetailForm = function () {
            $scope.IsVehicleServiceDetailSubmitted = false;
            $scope.VehicleServiceDetail = {};
            $scope.$broadcast('angucomplete-alt:clearInput', 'JobTypeAnguComplete');
            $scope.formVehicleServiceDetail.$setPristine();
            $scope.formVehicleServiceDetail.$setUntouched();
        }

        $scope.addVehicleServiceDetail = function () {
            $scope.IsVehicleServiceDetailSubmitted = true;
            if (!$scope.IsFormVehicleServiceDetailValid) {
                toastr.error("Fill all required fields.");
                return;
            }

            $scope.VehicleServiceDetail.JobType = document.getElementById('JobTypeAnguComplete_value').value;
            if ($scope.VehicleServiceDetail.JobType == null || $scope.VehicleServiceDetail.JobType == undefined) {
                toastr.error("Job Type is required!!");
                return;
            }

            //for (var i = 0; i < $scope.VehicleServiceDetails.length; i++) {
            //    if ($scope.VehicleServiceDetails[i].JobTypeId == $scope.SelectedJobType.JobTypeId) {
            //        toastr.error("This JobType is already added");
            //        return;
            //    }
            //}

            var VehicleServiceDetailModel = {
                JobType: $scope.VehicleServiceDetail.JobType,
                Remarks: $scope.VehicleServiceDetail.Remarks,
                Amount: $scope.VehicleServiceDetail.Amount,
                Quantity: $scope.VehicleServiceDetail.Quantity,
                UnitCost: $scope.VehicleServiceDetail.UnitCost,
            }
            $scope.VehicleService.VehicleServiceDetails.push(VehicleServiceDetailModel);
            $scope.calculateTotal();
            $scope.resetVehicleServiceDetailForm();
        }

        $scope.deleteVehicleServiceDetail = function (index) {
            $scope.VehicleService.VehicleServiceDetails.splice(index, 1);
        }


        $scope.calculateItemTotal = function () {
            $scope.VehicleServiceDetail.Amount = ($scope.VehicleServiceDetail.UnitCost * $scope.VehicleServiceDetail.Quantity);
        }


        $scope.calculateTotal = function () {
            $scope.VehicleService.BillAmount = 0;
            angular.forEach($scope.VehicleService.VehicleServiceDetails, function (VehicleServiceDetail, key1) {
                $scope.VehicleService.BillAmount += VehicleServiceDetail.Amount;
            })
        }

        //VehicleService
        $scope.$watch('formVehicleService.$valid', function (value) {
            $scope.IsFormVehicleServiceValid = value;
        })

        $scope.SaveVehicleService = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormVehicleServiceValid) {
                var supplierName = document.getElementById('SupplierAutoComplete_value').value;
                if (supplierName == undefined || supplierName == null || supplierName == '') {
                    toastr.error("Supplier is required.");
                    return;
                }

                if ($scope.VehicleService.BusId == undefined || $scope.VehicleService.BusId == null || $scope.VehicleService.BusId == '') {
                    toastr.error("Vehicle No is required.");
                    return;
                }


                if ($scope.VehicleService.Discount == null || $scope.VehicleService.Discount == undefined)
                    $scope.VehicleService.Discount = 0;

                if ($scope.VehicleService.VehicleServiceId > 0) {
                    var BillNo = document.getElementById('BillAutoComplete_value').value;
                    if (BillNo == null || BillNo == undefined) {
                        toastr.error("Bill No is required");
                        return;
                    }
                    $scope.VehicleService.BillNo = BillNo;
                }

                var VehicleServiceModel = {
                    VehicleServiceId: $scope.VehicleService.VehicleServiceId,
                    VehicleServiceType: $scope.VehicleService.VehicleServiceType,
                    SupplierId: $scope.VehicleService.SupplierId,
                    BusId: $scope.VehicleService.BusId,
                    MeterReading: $scope.VehicleService.MeterReading,
                    BillNo: $scope.VehicleService.BillNo,
                    BillAmount: $scope.VehicleService.BillAmount,
                    Discount: $scope.VehicleService.Discount,
                    PaidAmount: $scope.VehicleService.BillAmount - $scope.VehicleService.Discount,
                    Remarks: $scope.VehicleService.Remarks,
                    BillDate: loadDataService.getDateTime($scope.VehicleService.BillDate),
                    VehicleServiceDetails: $scope.VehicleService.VehicleServiceDetails,
                    Supplier: {
                        SupplierName: supplierName
                    }
                }
                $scope.dataLoading = true;
                adminService.saveVehicleService(VehicleServiceModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Vehicle Servicing is Successfully done.");
                            $scope.resetVehicleServiceForm();
                            $scope.getSupplierList();
                            $scope.getJobTypeList();
                            document.getElementById('BillAutoComplete_value').value = null;
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

        $scope.resetVehicleServiceForm = function () {
            $scope.IsSubmitted = false;
            $scope.VehicleService = {};
            $scope.VehicleService.VehicleServiceDetails = [];
            $scope.VehicleService.BillDate = new Date();
            document.getElementById('SupplierAutoComplete_value').value = '';
            document.getElementById('BusAutoComplete_value').value = '';
            $scope.formVehicleService.$setPristine();
            $scope.formVehicleService.$setUntouched();
            $scope.resetVehicleServiceDetailForm();
        }
    }
})();

