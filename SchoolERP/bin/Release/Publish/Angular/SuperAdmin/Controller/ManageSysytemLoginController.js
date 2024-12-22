(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSystemLoginController', ManageSystemLoginController);

    ManageSystemLoginController.$inject = ["$scope", "superAdminService", "Factory", "ConstantData"];

    function ManageSystemLoginController($scope, superAdminService, Factory, ConstantData) {
        $scope.checkSystemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SystemLogin = {};
        $scope.EmployeeId = null;
        $scope.LoginStatusList = ConstantData.LoginStatus;

        $scope.getRoleList = function () {
            $scope.dataLoading = true;
            superAdminService.getRoleList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RoleList = response.data.RoleList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRoleList();

        $scope.getSystemLoginList = function () {
            var EmployeeId = 0;
            if ($scope.EmployeeId != null && $scope.EmployeeId != undefined) {
                EmployeeId = $scope.EmployeeId;
            }
            $scope.dataLoading = true;
            superAdminService.getSystemLoginList(0, EmployeeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SystemLoginList = response.data.SystemLoginList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSystemLoginList();

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

        $scope.getEmployeeList = function () {
            $scope.dataLoading = true;
            superAdminService.getEmployeeList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.EmployeeList = response.data.EmployeeList;
                        //console.log(response.data, "Employee list Response");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEmployeeList();

        $scope.$watch('formSystemLogin.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveSystemLogin = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SystemLoginModel = {
                    EmployeeId: $scope.SystemLogin.EmployeeId,
                    LoginId: $scope.SystemLogin.LoginId,
                    SchoolId: $scope.SystemLogin.SchoolId,
                    UserName: $scope.SystemLogin.UserName,
                    //LoginName: $scope.SystemLogin.LoginName,
                    LoginPassword: $scope.SystemLogin.LoginPassword,
                    LoginStatus: $scope.SystemLogin.LoginStatus,
                    RoleList: $scope.SystemLogin.RoleList
                }

                $scope.dataLoading = true;
                superAdminService.saveSystemLogin(SystemLoginModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SystemLoginModel.SystemLoginId > 0)
                                toastr.success("Login Detail Updated Successfully.", "Success");
                            else
                                toastr.success("Login Created Successfully.", "Success");
                            $scope.resetForm();
                            $scope.getSystemLoginList();
                            $('#modal_Student_Charge').modal('hide');
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
            $scope.SystemLogin = [];
            $scope.SystemLogin.SchoolId = null;
            $scope.SystemLogin.EmployeeId = null;
            $scope.SystemLogin.LoginStatus = null;
            $scope.formSystemLogin.$setPristine();
            $scope.formSystemLogin.$setUntouched();
        }

        $scope.newSystemLogin = function () {
            $scope.resetForm();
            //$scope.SystemLogin.EmployeeId = $scope.EmployeeId;
            $scope.SystemLogin.RoleList = $scope.RoleList;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editSystemLogin = function (SystemLoginModel) {
            $scope.SystemLogin = SystemLoginModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (LoginId) {
            $scope.LoginId = LoginId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSystemLogin = function () {
            $scope.dataLoading = true;
            superAdminService.deleteSystemLogin($scope.LoginId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Login deleted successfully.", "Deleted Successfully.")
                        $scope.LoginId = null;
                        $scope.getSystemLoginList();
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
