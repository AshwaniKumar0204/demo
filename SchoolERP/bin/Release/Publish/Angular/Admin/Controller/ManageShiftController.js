(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageShiftController', ManageShiftController);

    ManageShiftController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageShiftController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;

        $scope.getShiftList = function () {
            $scope.dataLoading = true;
            adminService.getShiftList(0)
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


        $scope.$watch('formShift.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveShift = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var ShiftModel = {
                    ShiftId: $scope.Shift.ShiftId,
                    ShiftName: $scope.Shift.ShiftName,
                    InTime: document.getElementById('InTime').value,
                    OutTime: document.getElementById('OutTime').value,
                    GapeTime: document.getElementById('GapeTime').value,
                    ShiftDuration: $scope.Shift.ShiftDuration,
                    Status: $scope.Shift.Status,
                }

                $scope.dataLoading = true;
                adminService.saveShift(ShiftModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (ShiftModel.ShiftId > 0)
                                toastr.success("Shift Updated Successfully.");
                            else
                                toastr.success("New Shift Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getShiftList();
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

        $scope.calculateShiftDuration = function () {
            var InTime = document.getElementById('InTime').value;
            var OutTime = document.getElementById('OutTime').value;
            var hh = OutTime.substring(0, 2) - InTime.substring(0, 2);
            var mm = OutTime.substring(3, 5) - InTime.substring(3, 5);
            if (mm < 0) {
                $scope.Shift.ShiftDurationString = (hh - 1) + " hours " + (mm + 60) + " minutes";
            } else {
                $scope.Shift.ShiftDurationString = hh + " hours " + mm + " minutes";
            }
            $scope.Shift.ShiftDuration = hh * 60 + mm;
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Shift = {};
            document.getElementById("InTime").value = "";
            document.getElementById("OutTime").value = "";
            document.getElementById("GapeTime").value = "";
            $scope.formShift.$setPristine();
            $scope.formShift.$setUntouched();
        }

        $scope.newShift = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editShift = function (Shift) {
            $scope.resetForm();
            $scope.Shift = Shift;
            document.getElementById('InTime').value = Shift.InTime;
            document.getElementById('OutTime').value = Shift.OutTime;
            document.getElementById('GapeTime').value = Shift.GapeTime;
            $scope.calculateShiftDuration();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (ShiftId) {
            $scope.ShiftId = ShiftId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteShift = function () {
            $scope.dataLoading = true;
            adminService.deleteShift($scope.ShiftId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Shift deleted successfully.", "Deleted Successfully.")
                        $scope.ShiftId = null;
                        $('#modal_confirmation').modal('hide'); 
                        $scope.getShiftList();
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
