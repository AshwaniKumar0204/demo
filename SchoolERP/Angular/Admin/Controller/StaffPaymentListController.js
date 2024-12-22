(function () {
    'use strict';

    angular
        .module('app')
        .controller('StaffPaymentListController', StaffPaymentListController);

    StaffPaymentListController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function StaffPaymentListController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        var todayDate = new Date();
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

        $scope.calculateTotal = function () {
            $scope.LWPAvailed = 0;
            $scope.CLAvailed = 0;
            $scope.ELAvailed = 0;
            $scope.MLAvailed = 0;
            $scope.HPLAvailed = 0;
            $scope.BasicAmount = 0;
            $scope.Increament = 0;
            $scope.DA = 0;
            $scope.Allowance = 0;
            $scope.Arrear = 0;
            $scope.HRA = 0;
            $scope.MedicalAllowance = 0;
            $scope.SpecialAllowance = 0;
            $scope.GrossTotal = 0;
            $scope.EPF = 0;
            $scope.TDS = 0;
            $scope.ESI = 0;
            $scope.ProfessionalTax = 0;
            $scope.SalaryAdvanced = 0;
            $scope.OtherDeduction = 0;
            $scope.HouseRent = 0;
            $scope.ElectricityCharge = 0;
            $scope.LWP = 0;
            $scope.TotalDeduction = 0;
            $scope.NetPayment = 0;
            angular.forEach($scope.StaffPaymentList, function (model) {
                $scope.LWPAvailed += model.LWPAvailed;
                $scope.CLAvailed += model.CLAvailed;
                $scope.ELAvailed += model.ELAvailed;
                $scope.MLAvailed += model.MLAvailed;
                $scope.HPLAvailed += model.HPLAvailed;
                $scope.BasicAmount += model.BasicAmount;
                $scope.Increament += model.Increament;
                $scope.DA += model.DA;
                $scope.Allowance += model.Allowance;
                $scope.Arrear += model.Arrear;
                $scope.HRA += model.HRA;
                $scope.MedicalAllowance += model.MedicalAllowance;
                $scope.SpecialAllowance += model.SpecialAllowance;
                $scope.GrossTotal += model.GrossTotal;
                $scope.EPF += model.EPF;
                $scope.TDS += model.TDS;
                $scope.ESI += model.ESI;
                $scope.ProfessionalTax += model.ProfessionalTax;
                $scope.SalaryAdvanced += model.SalaryAdvanced;
                $scope.OtherDeduction += model.OtherDeduction;
                $scope.HouseRent += model.HouseRent;
                $scope.ElectricityCharge += model.ElectricityCharge;
                $scope.LWP += model.LWP;
                $scope.TotalDeduction += model.TotalDeduction;
                $scope.NetPayment += model.NetPayment;
            });
        }

        $scope.getStaffPaymentList = function () {
            $scope.TotalAmount = 0;
            var obj = {
                StaffId: $scope.StaffId,
                MonthNo: $scope.MonthNo,
                Year: $scope.Year
            }
            $scope.dataLoading = true;
            adminService.getStaffPaymentList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffPaymentList = response.data.StaffPaymentList;
                        $scope.calculateTotal();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffPaymentList();

        $scope.afterStaffSelectedSearch = function (obj) {
            if (obj != undefined) {
                $scope.StaffId = obj.originalObject.StaffId;
                $scope.getStaffPaymentList();
            } else {
                $scope.StaffId = 0;
            }
        }

        $scope.deleteConfirmation = function (StaffPaymentId) {
            $scope.StaffPaymentId = StaffPaymentId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffPayment = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffPayment($scope.StaffPaymentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Record deleted successfully.", "Deleted Successfully.")
                        $scope.StaffPaymentId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffPaymentList();
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

        $scope.printReport = function (no) {
            var ids = 0;
            angular.forEach($scope.StaffPaymentList, function (s1) {
                ids += ',' + s1.StaffPaymentId;
            });
            window.open("/print/SalarySlip?ids=" + ids + "&no=" + no);
        }
    }
})();
