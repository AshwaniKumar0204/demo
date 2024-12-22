(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageDepartmentController', ManageDepartmentController);

    ManageDepartmentController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageDepartmentController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.DepartmentStatusList = ConstantData.DepartmentStatus;

        $scope.Department = {};
        $scope.Department.DepartmentStatus = null;

        $scope.getDepartmentList = function () {
            $scope.dataLoading = true;
            adminService.getDepartmentList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DepartmentList = response.data.DepartmentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getDepartmentList();

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

        $scope.$watch('formDepartment.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveDepartment = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var DepartmentModel = {
                    DepartmentId: $scope.Department.DepartmentId,
                    DepartmentStatus: $scope.Department.DepartmentStatus,
                    DepartmentName: $scope.Department.DepartmentName,
                }

                $scope.dataLoading = true;
                adminService.saveDepartment(DepartmentModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (DepartmentModel.DepartmentId > 0)
                                toastr.success("Department Updated Successfully.");
                            else
                                toastr.success("New Department Created Successfully.");
                            $scope.resetForm();
                            $scope.DepartmentList = response.data.DepartmentList;
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
        $scope.Department = {};
        $scope.Department.DepartmentStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Department.DepartmentId = null;
            $scope.Department.DepartmentName = "";
            $scope.formDepartment.$setPristine();
            $scope.formDepartment.$setUntouched();
        }

        $scope.newDepartment = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editDepartment = function (DepartmentModel) {
            $scope.resetForm();
            $scope.Department.DepartmentName = DepartmentModel.DepartmentName;
            $scope.Department.DepartmentId = DepartmentModel.DepartmentId;
            $scope.Department.DepartmentStatus = DepartmentModel.DepartmentStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (DepartmentId) {
            $scope.DepartmentId = DepartmentId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteDepartment = function () {
            $scope.dataLoading = true;
            adminService.deleteDepartment($scope.DepartmentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Department deleted successfully.", "Deleted Successfully.")
                        $scope.DepartmentList = response.data.DepartmentList;
                        $scope.DepartmentId = null;
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
