(function () {
    'use strict';

    angular
        .module('app')
        .controller('StaffPaymentController', StaffPaymentController);

    StaffPaymentController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService", "$routeParams", "$location"];

    function StaffPaymentController($scope, adminService, ConstantData, Factory, loadDataService, $routeParams, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        var todayDate = new Date();
        $scope.StaffPayment = {};
        $scope.MonthNo = todayDate.getMonth() + 1;
        $scope.Year = todayDate.getFullYear();

        $scope.getStaffList = function () {
            $scope.dataLoading = true;
            adminService.getStaffList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffList = response.data.StaffList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffList();

        $scope.getEnumMonthList = function () {
            $scope.dataLoading = true;
            adminService.getEnumMonthList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MonthList = response.data.MonthList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEnumMonthList();

        $scope.afterStaffSelected = function (obj) {
            if (obj != undefined) {
                $scope.StaffId = obj.originalObject.StaffId;
            } else {
                $scope.StaffId = null;
            }
        }

        $scope.$watch('formStaff.$valid', function (value) {
            $scope.IsFormStaffValid = value;
        })

        $scope.getStaffPaymentDetail = function () {
            $scope.IsStaffSubmitted = true;
            if (!$scope.IsFormStaffValid) {
                toastr.error("Fill all required fields!!");
                return;
            }
            var obj = {
                StaffId: $scope.StaffId,
                MonthNo: $scope.MonthNo,
                Year: $scope.Year,
                StaffPaymentId: $scope.StaffPaymentId,
            };
            $scope.dataLoading = true;
            adminService.getStaffPaymentDetail(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffPayment = response.data.StaffPayment;
                        $scope.caculateTotal($scope.StaffPayment);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.caculateTotal = function (StaffPayment) {
            StaffPayment.EPF = Math.round((StaffPayment.BasicAmount + StaffPayment.DA) * 0.12);
            if (StaffPayment.GrossTotal < 21000) {
                StaffPayment.ESI = Math.round(StaffPayment.GrossTotal * 0.75)/100;
            }
            if (StaffPayment.LWPAvailed > 0) {
                StaffPayment.LWP = Math.round((StaffPayment.BasicAmount + StaffPayment.DA) * StaffPayment.LWPAvailed / 30);
            }
            StaffPayment.TotalDeduction = Math.round(StaffPayment.EPF + StaffPayment.ESI + StaffPayment.TDS + StaffPayment.ProfessionalTax
                + StaffPayment.SalaryAdvanced + StaffPayment.OtherDeduction + StaffPayment.HouseRent + StaffPayment.ElectricityCharge + StaffPayment.LWP);
            StaffPayment.NetPayment = Math.round(StaffPayment.GrossTotal - StaffPayment.TotalDeduction);
        }

        $scope.$watch('formStaffPayment.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStaffPayment = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                $scope.dataLoading = true;
                adminService.saveStaffPayment($scope.StaffPayment)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if ($scope.StaffPayment.StaffPaymentId > 0)
                                toastr.success("Data Updated Successfully.");
                            else
                                toastr.success("Data Saved Successfully.");
                            $scope.resetForm();
                            if ($scope.StaffPaymentId > 0)
                                window.history.back();
                            else
                                window.open("/print/SalarySlip?ids=" + response.data.StaffPaymentId + "&no=1")
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.IsStaffSubmitted = false;
            $scope.StaffPayment = {};
            $scope.StaffId = null;
            document.getElementById("StaffAnguComplete_value").value = "";
            $scope.formStaffPayment.$setPristine();
            $scope.formStaffPayment.$setUntouched();
            //$scope.fromStaff.$setPristine();
            //$scope.fromStaff.$setUntouched();
        }

        $scope.checkUpdate = function () {
            $scope.StaffPaymentId = 0;
            var id = $routeParams.id;
            if (id) {
                $scope.StaffPaymentId = id;
                var obj = {
                    StaffPaymentId: id,
                };
                $scope.dataLoading = true;
                adminService.getStaffPaymentDetail(obj)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.StaffPayment = response.data.StaffPayment;
                            console.log($scope.StaffPayment);
                            $scope.Year = $scope.StaffPayment.Year;
                            $scope.MonthNo = $scope.StaffPayment.MonthNo;
                            $scope.StaffId = $scope.StaffPayment.StaffId;
                            document.getElementById("StaffAnguComplete_value").value = $scope.StaffPayment.StaffName + " " + $scope.StaffPayment.StaffCode;
                            $scope.caculateTotal($scope.StaffPayment);
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
        $scope.checkUpdate();
    }
})();

