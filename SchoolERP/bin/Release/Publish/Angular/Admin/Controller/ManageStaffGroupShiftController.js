(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffGroupShiftController', ManageStaffGroupShiftController);

    ManageStaffGroupShiftController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStaffGroupShiftController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getStaffGroupList = function () {
            $scope.dataLoading = true;
            adminService.getStaffGroupList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffGroupList = response.data.StaffGroupList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffGroupList();

        $scope.afterStaffGroupSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedStaffGroup = selected.originalObject;
                $scope.getStaffGroupShiftList();
            };
        }

        $scope.getStaffGroupShiftList = function () {
            $scope.dataLoading = true;
            adminService.getStaffGroupShiftList($scope.SelectedStaffGroup.StaffGroupId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffGroupShiftList = response.data.StaffGroupShiftList;
                        $scope.LastStaffGroupShift = response.data.LastStaffGroupShift;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('formStaffGroupShift.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getShiftList = function () {
            $scope.dataLoading = true;
            adminService.getShiftList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ShiftList = response.data.ShiftList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getShiftList();

        $scope.newShifts = function () {
            $scope.IsUpdate = false;
            $scope.StaffGroupShift = {};
            $scope.NewStaffGroupShiftList = [];
            if ($scope.NewStaffGroupShiftList.length == 0) {
                $scope.resetForm();
                var FromDate = new Date();
                if ($scope.LastStaffGroupShift != null) {
                    FromDate = new Date($scope.LastStaffGroupShift.ShiftDate);
                    FromDate.setDate(FromDate.getDate() + 1);
                }

                $scope.StaffGroupShift.FromDate = FromDate;
                //$scope.StaffGroupShift.ToDate = ToDate;
            }
            $("#modal_Student_Charge").modal('show');
        }

        $scope.resetForm = function () {
            $scope.submitted = false;
            $scope.StaffGroupShift = {};
            $scope.formStaffGroupShift.$setPristine();
            $scope.formStaffGroupShift.$setUntouched();
        }

        $scope.addStaffGroupShift = function () {
            $scope.submitted = true;
            if ($scope.IsFormValid && $scope.StaffGroupShift.Shift) {
                var NewShift = {
                    FromDate: $scope.StaffGroupShift.FromDate,
                    ToDate: $scope.StaffGroupShift.ToDate,
                    ShiftId: $scope.StaffGroupShift.Shift.ShiftId,
                    Shift: $scope.StaffGroupShift.Shift
                }
                $scope.NewStaffGroupShiftList.push(NewShift);
                $scope.resetForm();
                var FromDate = new Date(NewShift.ToDate);
                FromDate.setDate(FromDate.getDate() + 1);
                if ($scope.LastStaffGroupShift == null) {
                    $scope.LastStaffGroupShift = {};
                    $scope.LastStaffGroupShift.ShiftDate = FromDate;
                }
                $scope.StaffGroupShift.FromDate = FromDate;
                $scope.StaffGroupShift.ToDate = FromDate;
            } else {
                toastr.error("Selected shift is invalid", "Validation Error");
            }
        }


        $scope.saveStaffGroupShift = function () {
            if ($scope.NewStaffGroupShiftList.length == 0) {
                toastr.error("No data found!!");
                return;
            }

            angular.forEach($scope.NewStaffGroupShiftList, function (shift, key) {
                shift.FromDate = loadDataService.getDateTime(shift.FromDate);
                shift.ToDate = loadDataService.getDateTime(shift.ToDate);
            });

            var obj = {
                StaffGroupShiftList: $scope.NewStaffGroupShiftList,
                StaffGroupId: $scope.SelectedStaffGroup.StaffGroupId
            }

            $scope.dataLoading = true;
            adminService.saveStaffGroupShift(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        if ($scope.IsUpdate)
                            toastr.success("Staff Group Shift Detail Updated Created Successfully.");
                        else
                            toastr.success("New Staff Group Shift Created Successfully.");
                        $scope.NewStaffGroupShiftList = {};
                        $('#modal_Student_Charge').modal('hide');
                        $scope.getStaffGroupShiftList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        

        $scope.editStaffGroupShift = function (StaffGroupShiftModel) {
            $scope.resetForm();
            $scope.IsUpdate = true;
            $scope.NewStaffGroupShiftList = [];
            var NewShift = {
                FromDate: new Date(StaffGroupShiftModel.ShiftDate),
                ToDate: new Date(StaffGroupShiftModel.ShiftDate),
                ShiftId: StaffGroupShiftModel.ShiftId,
            }

            for (var i = 0; i < $scope.ShiftList.length; i++) {
                if ($scope.ShiftList[i].ShiftId == StaffGroupShiftModel.ShiftId) {
                    NewShift.Shift = $scope.ShiftList[i];
                    break;
                }
            }

            $scope.NewStaffGroupShiftList.push(NewShift);
            $scope.resetForm();
            var FromDate = new Date(NewShift.ToDate);
            FromDate.setDate(FromDate.getDate() + 1);
            if ($scope.LastStaffGroupShift == null) {
                $scope.LastStaffGroupShift = {};
                $scope.LastStaffGroupShift.ShiftDate = FromDate;
            }
            $scope.StaffGroupShift.FromDate = FromDate;
            $scope.StaffGroupShift.ToDate = FromDate;

            $('#modal_Student_Charge').modal('show');
        }

        $scope.changeShift = function (StaffGroupShiftModel) {
            StaffGroupShiftModel.ShiftId = StaffGroupShiftModel.Shift.ShiftId;
        }

        $scope.deleteConfirmation = function (StaffGroupShiftId) {
            $scope.StaffGroupShiftId = StaffGroupShiftId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffGroupShift = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffGroupShift($scope.StaffGroupShiftId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Staff Group Shift deleted successfully.", "Deleted Successfully.")
                        $scope.StaffGroupShiftId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffGroupShiftList();
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
