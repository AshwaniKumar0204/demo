(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffSalaryController', ManageStaffSalaryController);

    ManageStaffSalaryController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStaffSalaryController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StaffSalary = {};
        $scope.StaffSalary.IsDA = true;
        $scope.StatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;

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

        $scope.afterStaffSelected = function (obj) {
            if (obj != undefined) {
                $scope.StaffSalary.StaffId = obj.originalObject.StaffId;
            } else {
                $scope.StaffSalary.StaffId = null;
            }
        }

        $scope.changeAmount = function () {
            if ($scope.StaffSalary.IsDA == true)
                $scope.StaffSalary.DA = $scope.StaffSalary.BasicAmount * 0.05;

            $scope.StaffSalary.GrossTotal = 0;
            if ($scope.StaffSalary.BasicAmount)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.BasicAmount;
            if ($scope.StaffSalary.DA)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.DA;
            if ($scope.StaffSalary.Increament)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.DA;
            if ($scope.StaffSalary.Allowance)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.Allowance;
            if ($scope.StaffSalary.Arrear)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.Arrear;
            if ($scope.StaffSalary.HRA)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.HRA;
            if ($scope.StaffSalary.MedicalAllowance)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.MedicalAllowance;
            if ($scope.StaffSalary.SpecialAllowance)
                $scope.StaffSalary.GrossTotal += $scope.StaffSalary.SpecialAllowance;
        }

        $scope.changeDA = function () {
            $scope.StaffSalary.IsDA = false;
            $scope.changeAmount();
        }

        $scope.calculateTotal = function () {
            $scope.TotalBasicAmount = 0;
            $scope.TotalIncreament = 0;
            $scope.TotalDA = 0;
            $scope.TotalAllowance = 0;
            $scope.TotalArrear = 0;
            $scope.TotalHRA = 0;
            $scope.TotalMedicalAllowance = 0;
            $scope.TotalSpecialAllowance = 0;
            $scope.TotalGrossTotal = 0;
            angular.forEach($scope.StaffSalaryList, function (salary) {
                $scope.TotalBasicAmount += salary.BasicAmount;
                $scope.TotalIncreament += salary.Increament;
                $scope.TotalDA += salary.DA;
                $scope.TotalAllowance += salary.Allowance;
                $scope.TotalArrear += salary.Arrear;
                $scope.TotalHRA += salary.HRA;
                $scope.TotalMedicalAllowance += salary.MedicalAllowance;
                $scope.TotalSpecialAllowance += salary.SpecialAllowance;
                $scope.TotalGrossTotal += salary.GrossTotal;
            })
        }

        $scope.getStaffSalaryList = function () {
            $scope.TotalAmount = 0;
            $scope.dataLoading = true;
            adminService.getStaffSalaryList($scope.StaffId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffSalaryList = response.data.StaffSalaryList;
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
        $scope.getStaffSalaryList();

        $scope.afterStaffSelectedSearch = function (obj) {
            if (obj != undefined) {
                $scope.StaffId = obj.originalObject.StaffId;
            } else {
                $scope.StaffId = 0;
            }
            $scope.getStaffSalaryList();
        }

        $scope.$watch('formStaffSalary.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStaffSalary = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StaffSalaryModel = {
                    StaffSalaryId: $scope.StaffSalary.StaffSalaryId,
                    StaffId: $scope.StaffSalary.StaffId,
                    BasicAmount: $scope.StaffSalary.BasicAmount,
                    Increament: $scope.StaffSalary.Increament,
                    DA: $scope.StaffSalary.DA,
                    Allowance: $scope.StaffSalary.Allowance,
                    Arrear: $scope.StaffSalary.Arrear,
                    HRA: $scope.StaffSalary.HRA,
                    MedicalAllowance: $scope.StaffSalary.MedicalAllowance,
                    SpecialAllowance: $scope.StaffSalary.SpecialAllowance,
                    GrossTotal: $scope.StaffSalary.GrossTotal,
                    Remarks: $scope.StaffSalary.Remarks,
                    Status: $scope.StaffSalary.Status,
                    SalaryStartDate: loadDataService.getDateTime($scope.StaffSalary.SalaryStartDate),
                }

                $scope.dataLoading = true;
                adminService.saveStaffSalary(StaffSalaryModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StaffSalaryModel.StaffSalaryId > 0)
                                toastr.success("Staff Salary Updated Successfully.");
                            else
                                toastr.success("New Staff Salary Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStaffSalaryList();
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
            $scope.StaffSalary = {};
            $scope.StaffSalary.IsDA = true;
            $scope.StaffSalary.Status = 1;
            $scope.formStaffSalary.$setPristine();
            $scope.formStaffSalary.$setUntouched();
        }

        $scope.newStaffSalary = function () {
            $scope.resetForm();
            $scope.StaffSalary.StaffId = $scope.StaffId;
            for (var i = 0; i < $scope.StaffList.length; i++) {
                var staff = $scope.StaffList[i];
                if (staff.StaffId == $scope.StaffId) {
                    document.getElementById("StaffAnguComplete_value").value = staff.StaffName + " " + staff.DesignationName + " " + staff.StaffCode;
                    break;
                }
            }
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStaffSalary = function (StaffSalaryModel) {
            $scope.resetForm();
            $scope.StaffSalary = StaffSalaryModel;
            if (StaffSalaryModel.SalaryStartDate)
                $scope.StaffSalary.SalaryStartDate = new Date(StaffSalaryModel.SalaryStartDate);
            if ($scope.StaffSalary.DA == 0)
                $scope.StaffSalary.IsDA = false;
            else
                $scope.StaffSalary.IsDA = true;

            document.getElementById("StaffAnguComplete_value").value = StaffSalaryModel.StaffName + " " + StaffSalaryModel.DesignationName + " " + StaffSalaryModel.StaffCode;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StaffSalaryId) {
            $scope.StaffSalaryId = StaffSalaryId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffSalary = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffSalary($scope.StaffSalaryId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Staff Salary deleted successfully.", "Deleted Successfully.")
                        $scope.StaffSalaryId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffSalaryList();
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
    }
})();
