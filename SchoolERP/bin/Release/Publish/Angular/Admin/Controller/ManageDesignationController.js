(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageDesignationController', ManageDesignationController);

    ManageDesignationController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageDesignationController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.DesignationStatusList = ConstantData.DesignationStatus;

        $scope.Designation = {};
        $scope.Designation.DesignationStatus = null;

        $scope.getDesignationList = function () {
            $scope.dataLoading = true;
            adminService.getDesignationList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DesignationList = response.data.DesignationList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getDesignationList();

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

        $scope.$watch('formDesignation.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveDesignation = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var DesignationModel = {
                    DesignationId: $scope.Designation.DesignationId,
                    DesignationStatus: $scope.Designation.DesignationStatus,
                    DesignationName: $scope.Designation.DesignationName,
                }

                $scope.dataLoading = true;
                adminService.saveDesignation(DesignationModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (DesignationModel.DesignationId > 0)
                                toastr.success("Designation Updated Successfully.");
                            else
                                toastr.success("New Designation Created Successfully.");
                            $scope.resetForm();
                            $scope.DesignationList = response.data.DesignationList;
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
        $scope.Designation = {};
        $scope.Designation.DesignationStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Designation.DesignationId = null;
            $scope.Designation.DesignationName = "";
            $scope.formDesignation.$setPristine();
            $scope.formDesignation.$setUntouched();
        }

        $scope.newDesignation = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editDesignation = function (DesignationModel) {
            $scope.resetForm();
            $scope.Designation.DesignationName = DesignationModel.DesignationName;
            $scope.Designation.DesignationId = DesignationModel.DesignationId;
            $scope.Designation.DesignationStatus = DesignationModel.DesignationStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (DesignationId) {
            $scope.DesignationId = DesignationId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteDesignation = function () {
            $scope.dataLoading = true;
            adminService.deleteDesignation($scope.DesignationId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Designation deleted successfully.", "Deleted Successfully.")
                        $scope.DesignationList = response.data.DesignationList;
                        $scope.DesignationId = null;
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
