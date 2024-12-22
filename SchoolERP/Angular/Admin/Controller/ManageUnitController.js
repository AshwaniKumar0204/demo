(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageUnitController', ManageUnitController);

    ManageUnitController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageUnitController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.UnitStatusList = ConstantData.UnitStatus;

        $scope.Unit = {};
        $scope.Unit.UnitStatus = null;

        $scope.getUnitList = function () {
            $scope.dataLoading = true;
            adminService.getUnitList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.UnitList = response.data.UnitList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getUnitList();

        $scope.$watch('formUnit.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveUnit = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var UnitModel = {
                    UnitId: $scope.Unit.UnitId,
                    UnitStatus: $scope.Unit.UnitStatus,
                    UnitName: $scope.Unit.UnitName,
                }

                $scope.dataLoading = true;
                adminService.saveUnit(UnitModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (UnitModel.UnitId > 0)
                                toastr.success("Unit Updated Successfully.");
                            else
                                toastr.success("New Unit Created Successfully.");
                            $scope.resetForm();
                            $scope.UnitList = response.data.UnitList;
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

        $scope.Unit = {};
        $scope.Unit.UnitStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Unit.UnitId = null;
            $scope.Unit.UnitName = "";
            $scope.formUnit.$setPristine();
            $scope.formUnit.$setUntouched();
        }

        $scope.newUnit = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editUnit = function (UnitModel) {
            $scope.resetForm();
            $scope.Unit.UnitName = UnitModel.UnitName;
            $scope.Unit.UnitId = UnitModel.UnitId;
            $scope.Unit.UnitStatus = UnitModel.UnitStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (UnitId) {
            $scope.UnitId = UnitId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteUnit = function () {
            $scope.dataLoading = true;
            adminService.deleteUnit($scope.UnitId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Unit deleted successfully.", "Deleted Successfully.")
                        $scope.UnitList = response.data.UnitList;
                        $scope.UnitId = null;
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
