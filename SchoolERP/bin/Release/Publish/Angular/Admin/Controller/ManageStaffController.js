(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffController', ManageStaffController);

    ManageStaffController.$inject = ["$scope", "adminService", "loadDataService", "ConstantData","Factory"];

    function ManageStaffController($scope, adminService, loadDataService, ConstantData, Factory ) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Staff = {};
        $scope.StaffStatusList = ConstantData.StaffStatus;
        $scope.GenderList = ConstantData.Gender;

        $scope.getStaffList = function () {
            $scope.dataLoading = true;
            adminService.getStaffList(0, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffList = response.data.StaffList;
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
        $scope.getStaffList();

        $scope.getStaffTypeList = function () {
            $scope.dataLoading = true;
            adminService.getStaffTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffTypeList = response.data.StaffTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffTypeList();

        $scope.getDepartmentList = function () {
            $scope.dataLoading = true;
            adminService.getDepartmentList(1)
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

        $scope.getDesignationList = function () {
            $scope.dataLoading = true;
            adminService.getDesignationList(1)
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

        $scope.filterList = function () {
            $scope.filterStaffList = [];
            angular.forEach($scope.StaffList, function (Staff, key) {
                if (($scope.StaffType > 0 ? $scope.StaffType == Staff.StaffType : true) && ($scope.DepartmentId > 0 ? $scope.DepartmentId == Staff.DepartmentId : true)) {
                    $scope.filterStaffList.push(Staff);
                }
            });
        }

        $scope.$watch('formStaff.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStaff = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StaffModel = {
                    StaffId: $scope.Staff.StaffId,
                    DepartmentId: $scope.Staff.DepartmentId,
                    DesignationId: $scope.Staff.DesignationId,
                    StaffStatus: $scope.Staff.StaffStatus,
                    Address: $scope.Staff.Address,
                    AlternateNo: $scope.Staff.AlternateNo,
                    DateOfBirth: loadDataService.getDateTime($scope.Staff.DateOfBirth),
                    EmailId: $scope.Staff.EmailId,
                    StaffName: $scope.Staff.StaffName,
                    StaffType: $scope.Staff.StaffType,
                    Gender: $scope.Staff.Gender,
                    MobileNo: $scope.Staff.MobileNo,
                    PanNo: $scope.Staff.PanNo,
                    BankAccountNo: $scope.Staff.BankAccountNo,
                    PFNo: $scope.Staff.PFNo,
                    ESINo: $scope.Staff.ESINo,
                    JoinDate: loadDataService.getDateTime($scope.Staff.JoinDate),
                }

                $scope.dataLoading = true;
                adminService.saveStaff(StaffModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StaffModel.StaffId > 0)
                                toastr.success("Staff Detail Updated Successfully.");
                            else
                                toastr.success("Staff Created Successfully.");
                            $scope.resetForm();
                            $scope.StaffList = response.data.StaffList;
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
            $scope.Staff = [];
            $scope.Staff.StaffStatus = null;
            $scope.Staff.Gender = null;
            $scope.Staff.JoinDate = new Date();
            $scope.formStaff.$setPristine();
            $scope.formStaff.$setUntouched();
        }

        $scope.newStaff = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStaff = function (StaffModel) {
            //$scope.Staff = StaffModel;
            $scope.Staff.JoinDate = new Date(StaffModel.JoinDate);
            $scope.Staff.DateOfBirth = new Date(StaffModel.DateOfBirth);
            $scope.Staff.StaffName = StaffModel.StaffName;
            $scope.Staff.Gender = StaffModel.Gender;
            $scope.Staff.Address = StaffModel.Address;
            $scope.Staff.MobileNo = parseInt(StaffModel.MobileNo);
            $scope.Staff.AlternateNo = parseInt(StaffModel.AlternateNo);
            $scope.Staff.EmailId = StaffModel.EmailId;
            $scope.Staff.StaffStatus = StaffModel.StaffStatus;
            $scope.Staff.DepartmentId = StaffModel.DepartmentId;
            $scope.Staff.StaffId = StaffModel.StaffId;
            $scope.Staff.PanNo = StaffModel.PanNo;
            $scope.Staff.BankAccountNo = StaffModel.BankAccountNo;
            $scope.Staff.DesignationId = StaffModel.DesignationId;
            $scope.Staff.PFNo = StaffModel.PFNo;
            $scope.Staff.ESINo = StaffModel.ESINo;
            $scope.Staff.StaffType = StaffModel.StaffType;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StaffId) {
            $scope.StaffId = StaffId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaff = function () {
            $scope.dataLoading = true;
            adminService.deleteStaff($scope.StaffId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Staff deleted successfully.", "Deleted Successfully.")
                        $scope.StaffList = response.data.StaffList;
                        $scope.filterList();
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
