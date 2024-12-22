(function () {
    'use strict';

    angular
        .module('app')
        .controller('EmployeeTypeController', EmployeeTypeController);

    EmployeeTypeController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData"];

    function EmployeeTypeController($scope, superAdminService, loadDataService, ConstantData) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.EmployeeType = {};
        $scope.SchoolId = null;
        $scope.EmployeeTypeStatusList = ConstantData.EmployeeTypeStatus;

        $scope.getSchoolList = function () {
            $scope.dataLoading = true;
            superAdminService.getSchoolList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SchoolList = response.data.SchoolList;
                        var index = { SchoolId: null, ShortName: '--Select School--' };
                        $scope.SchoolList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSchoolList();

        $scope.getEmployeeTypeList = function () {
            $scope.dataLoading = true;
            superAdminService.getEmployeeTypeList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainEmployeeTypeList = response.data.EmployeeTypeList;
                        $scope.EmployeeTypeList = response.data.EmployeeTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEmployeeTypeList();

        $scope.filterList = function () {
            $scope.EmployeeTypeList = [];
            angular.forEach($scope.MainEmployeeTypeList, function (employeeType, key) {
                if (employeeType.SchoolId == $scope.SchoolId || $scope.SchoolId == null)
                    $scope.EmployeeTypeList.push(employeeType);
            })
        }

        $scope.$watch('formEmployeeType.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveEmployeeType = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var EmployeeTypeModel = {
                    EmployeeTypeId: $scope.EmployeeType.EmployeeTypeId,
                    EmployeeTypeStatus: $scope.EmployeeType.EmployeeTypeStatus,
                    TypeName: $scope.EmployeeType.TypeName,
                    SchoolId: $scope.EmployeeType.SchoolId,
                }

                $scope.dataLoading = true;
                superAdminService.saveEmployeeType(EmployeeTypeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (EmployeeTypeModel.EmployeeTypeId > 0)
                                toastr.success("Employee Type Detail Updated Successfully.");
                            else
                                toastr.success("Employee Type Created Successfully.");
                            $scope.resetForm();
                            $scope.MainEmployeeTypeList = response.data.EmployeeTypeList;
                            $scope.filterList();
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.EmployeeType = [];
            $scope.EmployeeType.EmployeeTypeStatus = null;
            $scope.EmployeeType.SchoolId = null;
            $scope.formEmployeeType.$setPristine();
            $scope.formEmployeeType.$setUntouched();
        }

        $scope.newEmployeeType = function () {
            $scope.resetForm();
            $scope.EmployeeType.SchoolId = $scope.SchoolId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editEmployeeType = function (EmployeeTypeModel) {
            $scope.resetForm();
            $scope.EmployeeType.TypeName = EmployeeTypeModel.TypeName;
            $scope.EmployeeType.SchoolId = EmployeeTypeModel.SchoolId;
            $scope.EmployeeType.EmployeeTypeStatus = EmployeeTypeModel.EmployeeTypeStatus;
            $scope.EmployeeType.EmployeeTypeId = EmployeeTypeModel.EmployeeTypeId;
            //$scope.EmployeeType = EmployeeTypeModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (EmployeeTypeId) {
            $scope.EmployeeTypeId = EmployeeTypeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteEmployeeType = function () {
            $scope.dataLoading = true;
            superAdminService.deleteEmployeeType($scope.EmployeeTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Employee Type deleted successfully.", "Deleted Successfully.")
                        $scope.MainEmployeeTypeList = response.data.EmployeeTypeList;
                        $scope.filterList();
                        $scope.EmployeeTypeId = null;
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
