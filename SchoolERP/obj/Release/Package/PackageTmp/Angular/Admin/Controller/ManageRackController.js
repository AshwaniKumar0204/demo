(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRackController', ManageRackController);

    ManageRackController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageRackController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.RackStatusList = ConstantData.RackStatus;

        $scope.Rack = {};
        $scope.Rack.RackStatus = null;

        $scope.getRackList = function () {
            $scope.dataLoading = true;
            adminService.getRackList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RackList = response.data.RackList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRackList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.$watch('formRack.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveRack = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var RackModel = {
                    RackId: $scope.Rack.RackId,
                    RackStatus: $scope.Rack.RackStatus,
                    RackName: $scope.Rack.RackName,
                }

                $scope.dataLoading = true;
                adminService.saveRack(RackModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (RackModel.RackId > 0)
                                toastr.success("Rack Updated Successfully.");
                            else
                                toastr.success("New Rack Created Successfully.");
                            $scope.resetForm();
                            $scope.RackList = response.data.RackList;
                            $('#modal_Student_Charge').modal('hide');
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
        $scope.Rack = {};
        $scope.Rack.RackStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Rack.RackId = null;
            $scope.Rack.RackName = "";
            $scope.formRack.$setPristine();
            $scope.formRack.$setUntouched();
        }

        $scope.newRack = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editRack = function (RackModel) {
            $scope.resetForm();
            $scope.Rack.RackName = RackModel.RackName;
            $scope.Rack.RackId = RackModel.RackId;
            $scope.Rack.RackStatus = RackModel.RackStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (RackId) {
            $scope.RackId = RackId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteRack = function () {
            $scope.dataLoading = true;
            adminService.deleteRack($scope.RackId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Rack deleted successfully.", "Deleted Successfully.")
                        $scope.RackList = response.data.RackList;
                        $scope.RackId = null;
                        $('#modal_confirmation').modal('hide');
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
})();
