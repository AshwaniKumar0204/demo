(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageLeaveController', ManageLeaveController);

    ManageLeaveController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageLeaveController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.StaffAvailableLeaves = [];
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;

        $scope.Leave = {};
        $scope.Leave.Status = null;

        $scope.getStaffAvailableLeaveList = function () {
            if ($scope.Leave.LeaveType == null || $scope.Leave.Year == null) {
                $scope.StaffAvailableLeaves = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getStaffAvailableLeaveList($scope.Leave.LeaveType, $scope.Leave.Year)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffAvailableLeaves = response.data.StaffAvailableLeaveList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getLeaveTypeList = function () {
            $scope.dataLoading = true;
            adminService.getLeaveTypeList()
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

        $scope.getLeaveYearList = function () {
            $scope.dataLoading = true;
            adminService.getLeaveYearList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.YearList = response.data.YearList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getLeaveYearList();

        $scope.getLeaveList = function () {
            $scope.dataLoading = true;
            adminService.getLeaveList($scope.Status, $scope.StaffType, $scope.Year)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.LeaveList = response.data.LeaveList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getLeaveList();

        $scope.$watch('formLeave.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveLeave = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                $scope.Leave.Status = 1;
                var LeaveModel = {
                    LeaveId: $scope.Leave.LeaveId,
                    Status: $scope.Leave.Status,
                    LeaveType: $scope.Leave.LeaveType,
                    NoOfDays: $scope.Leave.NoOfDays,
                    Year: $scope.Leave.Year,
                    StaffAvailableLeaves: $scope.StaffAvailableLeaves,
                }

                $scope.dataLoading = true;
                adminService.saveLeave(LeaveModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (LeaveModel.LeaveId > 0)
                                toastr.success("Leave Updated Successfully.");
                            else
                                toastr.success("New Leave Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getLeaveList();
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
            $scope.Leave = {};
            $scope.Leave.Status = null;
            $scope.StaffAvailableLeaves = [];
            $scope.formLeave.$setPristine();
            $scope.formLeave.$setUntouched();
        }

        $scope.newLeave = function () {
            $scope.resetForm();
            $scope.IsChangeStaffLeave = true;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.changeStaffLeave = function () {
            if ($scope.IsChangeStaffLeave == true) {
                angular.forEach($scope.StaffAvailableLeaves, function (s1) {
                    s1.NoOfLeaves = $scope.Leave.NoOfDays;
                })
            }
        }


        $scope.editLeave = function (LeaveModel) {
            $scope.resetForm();
            $scope.Leave = LeaveModel;
            $scope.IsChangeStaffLeave = false;
            $scope.getStaffAvailableLeaveList();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (LeaveId) {
            $scope.LeaveId = LeaveId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteLeave = function () {
            $scope.dataLoading = true;
            adminService.deleteLeave($scope.LeaveId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Leave deleted successfully.", "Deleted Successfully.")
                        $scope.LeaveId = null;
                        $scope.getLeaveList();
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
