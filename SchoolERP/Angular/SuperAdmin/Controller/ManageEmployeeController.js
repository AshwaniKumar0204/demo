(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageEmployeeController', ManageEmployeeController);

    ManageEmployeeController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData","Factory"];

    function ManageEmployeeController($scope, superAdminService, loadDataService, ConstantData, Factory) {
        $scope.checkSystemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Employee = {};
        $scope.SchoolId = null;
        $scope.EmployeeTypeId = null;
        $scope.EmployeeStatusList = ConstantData.EmployeeStatus;
        $scope.GenderList = ConstantData.Gender;

        $scope.getStaffList = function () {
            $scope.dataLoading = true;
            superAdminService.getStaffList(1, 0)
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

       
        $scope.afterStaffSelected = function (selected) {
            if (selected != undefined) {
                $scope.Employee.StaffId = selected.originalObject.StaffId;
                $scope.Employee.EmployeeName = selected.originalObject.StaffName;
                $scope.Employee.DateOfBirth = selected.originalObject.DateOfBirth;
                $scope.Employee.Gender = selected.originalObject.Gender;
                $scope.Employee.Address = selected.originalObject.Address;
                $scope.Employee.MobileNo = selected.originalObject.MobileNo;
                $scope.Employee.AlternateNo = selected.originalObject.AlternateNo;
                $scope.Employee.EmailId = selected.originalObject.EmailId;
            } else {
                $scope.Employee.StaffId = null;
            }
        }

        $scope.getEmployeeList = function () {
            $scope.dataLoading = true;
            superAdminService.getEmployeeList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.EmployeeList = response.data.EmployeeList;
                        $scope.filterList();
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
                        $scope.changeSchool();
                        $scope.changeSchool2();
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

        $scope.changeSchool = function () {
            //$scope.filterEmployeeList = [];
            $scope.EmployeeTypeList = [];
            angular.forEach($scope.MainEmployeeTypeList, function (employee, key) {
                if (employee.SchoolId == $scope.SchoolId) {
                    $scope.EmployeeTypeList.push(employee);
                }
            })
            var index = { EmployeeTypeId: null, TypeName: '--Select Employee Type--' };
            $scope.EmployeeTypeList.unshift(index);
            $scope.EmployeeTypeId = null;
            $scope.filterList();
        }

        $scope.changeSchool2 = function () {
            $scope.EmployeeTypeList2 = [];
            angular.forEach($scope.MainEmployeeTypeList, function (employee, key) {
                if (employee.SchoolId == $scope.Employee.SchoolId) {
                    $scope.EmployeeTypeList2.push(employee);
                }
            })
            var index = { EmployeeTypeId: null, TypeName: '--Select Employee Type--' };
            $scope.EmployeeTypeList2.unshift(index);
            $scope.Employee.EmployeeTypeId = null;
        }

        $scope.filterList = function () {
            $scope.filterEmployeeList = [];
            angular.forEach($scope.EmployeeList, function (employee, key) {
                if (($scope.EmployeeTypeId > 0 ? $scope.EmployeeTypeId == employee.EmployeeTypeId : true) && ($scope.SchoolId > 0 ? $scope.SchoolId == employee.SchoolId : true)) {
                    $scope.filterEmployeeList.push(employee);
                }
            });
        }

        $scope.$watch('formEmployee.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveEmployee = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var EmployeeModel = {
                    EmployeeId: $scope.Employee.EmployeeId,
                    EmployeeStatus: $scope.Employee.EmployeeStatus,
                    Address: $scope.Employee.Address,
                    AlternateNo: $scope.Employee.AlternateNo,
                    DateOfBirth: loadDataService.getDateTime($scope.Employee.DateOfBirth),
                    EmailId: $scope.Employee.EmailId,
                    EmployeeName: $scope.Employee.EmployeeName,
                    EmployeeTypeId: $scope.Employee.EmployeeTypeId,
                    Gender: $scope.Employee.Gender,
                    MobileNo: $scope.Employee.MobileNo,
                    StaffId: $scope.Employee.StaffId,
                    JoinDate: loadDataService.getDateTime($scope.Employee.JoinDate),
                }

                $scope.dataLoading = true;
                superAdminService.saveEmployee(EmployeeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (EmployeeModel.EmployeeId > 0)
                                toastr.success("Employee Detail Updated Successfully.");
                            else
                                toastr.success("Employee Created Successfully.");
                            $scope.resetForm();
                            $scope.EmployeeList = response.data.EmployeeList;
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
            $scope.Employee = [];
            $scope.Employee.EmployeeStatus = null;
            $scope.Employee.EmployeeId = null;
            $scope.Employee.SchoolId = null;
            $scope.Employee.EmployeeTypeId = null;
            $scope.Employee.Gender = null;
            $scope.Employee.JoinDate = new Date();
            document.getElementById('StaffAutoComplete_value').value = "";
            $scope.formEmployee.$setPristine();
            $scope.formEmployee.$setUntouched();
        }

        $scope.newEmployee = function () {
            $scope.resetForm();
            $scope.Employee.SchoolId = $scope.SchoolId;
            $scope.changeSchool2();
            $scope.Employee.EmployeeTypeId = $scope.EmployeeTypeId;
            $scope.Employee.EmployeeId = $scope.EmployeeId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editEmployee = function (EmployeeModel) {
            //$scope.Employee = EmployeeModel;
            $scope.Employee.JoinDate = new Date(EmployeeModel.JoinDate);
            $scope.Employee.DateOfBirth = new Date(EmployeeModel.DateOfBirth);
            $scope.Employee.EmployeeName = EmployeeModel.EmployeeName;
            $scope.Employee.Gender = EmployeeModel.Gender;
            $scope.Employee.Address = EmployeeModel.Address;
            $scope.Employee.MobileNo = parseInt(EmployeeModel.MobileNo);
            $scope.Employee.AlternateNo = parseInt(EmployeeModel.AlternateNo);
            $scope.Employee.EmailId = EmployeeModel.EmailId;
            $scope.Employee.EmployeeStatus = EmployeeModel.EmployeeStatus;
            $scope.Employee.SchoolId = EmployeeModel.SchoolId;
            $scope.Employee.EmployeeId = EmployeeModel.EmployeeId;
            $scope.changeSchool2();
            $scope.Employee.EmployeeTypeId = EmployeeModel.EmployeeTypeId;
            if ($scope.Employee.StaffId > 0) {
                for (var i = 0; i < $scope.StaffList.length; i++) {
                    if ($scope.StaffList[i].StaffId == $scope.Employee.StaffId) {
                        document.getElementById('StaffAutoComplete_value').value = $scope.StaffList[i].SearchStaff;
                        break;
                    }
                }
            }
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (EmployeeId) {
            $scope.EmployeeId = EmployeeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteEmployee = function () {
            $scope.dataLoading = true;
            superAdminService.deleteEmployee($scope.EmployeeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Employee deleted successfully.", "Deleted Successfully.")
                        $scope.EmployeeList = response.data.EmployeeList;
                        $scope.filterList();
                        $scope.EmployeeId = null;
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
