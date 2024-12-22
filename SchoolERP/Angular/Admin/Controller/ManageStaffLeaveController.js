(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffLeaveController', ManageStaffLeaveController);

    ManageStaffLeaveController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStaffLeaveController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StaffLeave = {};
        var todayDate = new Date();
        $scope.MonthNo = todayDate.getMonth() + 1;
        $scope.Year = todayDate.getFullYear();

        $scope.getStaffAvailableLeaves = function () {
            if ($scope.StaffLeave.StaffId == null || $scope.StaffLeave.StaffId == undefined) {
                $scope.StaffAvailableLeaves = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getStaffAvailableLeaves($scope.StaffLeave.StaffId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffAvailableLeaves = response.data.StaffAvailableLeaves;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

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

        $scope.getLeaveTypeList = function () {
            $scope.dataLoading = true;
            adminService.getLeaveTypeList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.LeaveTypeList = response.data.LeaveTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getLeaveTypeList();

        $scope.afterStaffSelected = function (obj) {
            if (obj != undefined) {
                $scope.StaffLeave.StaffId = obj.originalObject.StaffId;
                $scope.getStaffAvailableLeaves();
            } else {
                $scope.StaffLeave.StaffId = null;
                $scope.StaffAvailableLeaves = [];
            }
        }

        $scope.getMonthList = function () {
            $scope.TotalAmount = 0;
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
        $scope.getMonthList();

        $scope.getStaffLeaveList = function () {
            $scope.TotalAmount = 0;
            $scope.dataLoading = true;
            var obj = {
                StaffId: $scope.StaffId,
                MonthNo: $scope.MonthNo,
                Year: $scope.Year
            }
            adminService.getStaffLeaveList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffLeaveList = response.data.StaffLeaveList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffLeaveList();

        $scope.afterStaffSelectedSearch = function (obj) {
            if (obj != undefined) {
                $scope.StaffId = obj.originalObject.StaffId;
                $scope.getStaffLeaveList();
            } else {
                $scope.StaffId = 0;
            }
        }

        $scope.$watch('formStaffLeave.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStaffLeave = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StaffLeaveModel = {
                    StaffLeaveId: $scope.StaffLeave.StaffLeaveId,
                    StaffId: $scope.StaffLeave.StaffId,
                    LeaveType: $scope.StaffLeave.LeaveType,
                    Remarks: $scope.StaffLeave.Remarks,
                    MonthNo: $scope.StaffLeave.MonthNo,
                    NoOfLeaves: $scope.StaffLeave.NoOfLeaves,
                    Year: $scope.StaffLeave.Year,
                }

                $scope.dataLoading = true;
                adminService.saveStaffLeave(StaffLeaveModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StaffLeaveModel.StaffLeaveId > 0)
                                toastr.success("Staff Leave Updated Successfully.");
                            else
                                toastr.success("New Staff Leave Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStaffLeaveList();
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
            $scope.StaffLeave = {};
            $scope.formStaffLeave.$setPristine();
            $scope.formStaffLeave.$setUntouched();
        }

        $scope.newStaffLeave = function () {
            $scope.resetForm();
            $scope.StaffLeave.StaffId = $scope.StaffId;
            $scope.getStaffAvailableLeaves();
            $scope.StaffLeave.MonthNo = todayDate.getMonth() + 1;
            $scope.StaffLeave.Year = todayDate.getFullYear();
            document.getElementById("StaffAnguComplete_value").value = null;
            for (var i = 0; i < $scope.StaffList.length; i++) {
                var staff = $scope.StaffList[i];
                if (staff.StaffId == $scope.StaffId) {
                    document.getElementById("StaffAnguComplete_value").value = staff.StaffName + " " + (staff.DesignationName != null ? staff.DesignationName : '') + " " + staff.StaffCode;
                    break;
                }
            }
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStaffLeave = function (StaffLeaveModel) {
            $scope.resetForm();
            $scope.StaffLeave = StaffLeaveModel;
            $scope.getStaffAvailableLeaves();
            document.getElementById("StaffAnguComplete_value").value = StaffLeaveModel.StaffName + " " + StaffLeaveModel.DesignationName + " " + StaffLeaveModel.StaffCode;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StaffLeaveId) {
            $scope.StaffLeaveId = StaffLeaveId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffLeave = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffLeave($scope.StaffLeaveId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Staff Leave deleted successfully.", "Deleted Successfully.")
                        $scope.StaffLeaveId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffLeaveList();
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
