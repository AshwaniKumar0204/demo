(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffAdvancePaymentController', ManageStaffAdvancePaymentController);

    ManageStaffAdvancePaymentController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStaffAdvancePaymentController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        var todayDate = new Date();
        $scope.MonthNo = todayDate.getMonth() + 1;
        $scope.Year = todayDate.getFullYear();
        $scope.StaffAdvancePayment = {};

        $scope.getStaffList = function () {
            $scope.dataLoading = true;
            adminService.getStaffList(1)
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

        $scope.getEnumMonthList = function () {
            $scope.dataLoading = true;
            adminService.getEnumMonthList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MonthList = response.data.MonthList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEnumMonthList();

        $scope.afterStaffSelected = function (obj) {
            if (obj != undefined) {
                $scope.StaffAdvancePayment.StaffId = obj.originalObject.StaffId;
            } else {
                $scope.StaffAdvancePayment.StaffId = null;
            }
        }

        $scope.getStaffAdvancePaymentList = function () {
            $scope.TotalAmount = 0;
            var obj = {
                StaffId: $scope.StaffId,
                MonthNo: $scope.MonthNo,
                Year: $scope.Year
            }
            $scope.dataLoading = true;
            adminService.getStaffAdvancePaymentList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffAdvancePaymentList = response.data.StaffAdvancePaymentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffAdvancePaymentList();

        $scope.afterStaffSelectedSearch = function (obj) {
            if (obj != undefined) {
                $scope.StaffId = obj.originalObject.StaffId;
                $scope.getStaffAdvancePaymentList();
            } else {
                $scope.StaffId = 0;
            }
        }

        $scope.$watch('formStaffAdvancePayment.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStaffAdvancePayment = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StaffAdvancePaymentModel = {
                    StaffAdvancePaymentId: $scope.StaffAdvancePayment.StaffAdvancePaymentId,
                    StaffId: $scope.StaffAdvancePayment.StaffId,
                    MonthNo: $scope.StaffAdvancePayment.MonthNo,
                    Year: $scope.StaffAdvancePayment.Year,
                    AdvanceAmount: $scope.StaffAdvancePayment.AdvanceAmount,
                    Remarks: $scope.StaffAdvancePayment.Remarks,
                    AdvanceDate: loadDataService.getDateTime($scope.StaffAdvancePayment.AdvanceDate),
                }

                $scope.dataLoading = true;
                adminService.saveStaffAdvancePayment(StaffAdvancePaymentModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StaffAdvancePaymentModel.StaffAdvancePaymentId > 0)
                                toastr.success("Staff Advance Payment Updated Successfully.");
                            else
                                toastr.success("New Staff Advance Payment Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStaffAdvancePaymentList();
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
            $scope.StaffAdvancePayment = {};
            $scope.StaffAdvancePayment.AdvanceDate = new Date();
            document.getElementById("StaffAnguComplete_value").value = "";
            $scope.formStaffAdvancePayment.$setPristine();
            $scope.formStaffAdvancePayment.$setUntouched();
        }

        $scope.newStaffAdvancePayment = function () {
            $scope.resetForm();
            $scope.StaffAdvancePayment.StaffId = $scope.StaffId;
            for (var i = 0; i < $scope.StaffList.length; i++) {
                var staff = $scope.StaffList[i];
                if (staff.StaffId == $scope.StaffId) {
                    document.getElementById("StaffAnguComplete_value").value = staff.StaffName + " " + staff.DesignationName + " " + staff.StaffCode;
                    break;
                }
            }
            $scope.StaffAdvancePayment.MonthNo = todayDate.getMonth() + 1;
            $scope.StaffAdvancePayment.Year = todayDate.getFullYear();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStaffAdvancePayment = function (StaffAdvancePaymentModel) {
            $scope.resetForm();
            $scope.StaffAdvancePayment = StaffAdvancePaymentModel;
            $scope.StaffAdvancePayment.AdvanceDate = new Date(StaffAdvancePaymentModel.AdvanceDate);
            document.getElementById("StaffAnguComplete_value").value = StaffAdvancePaymentModel.StaffName + " " + StaffAdvancePaymentModel.DesignationName + " " + StaffAdvancePaymentModel.StaffCode;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StaffAdvancePaymentId) {
            $scope.StaffAdvancePaymentId = StaffAdvancePaymentId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffAdvancePayment = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffAdvancePayment($scope.StaffAdvancePaymentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Staff Advance Payment deleted successfully.", "Deleted Successfully.")
                        $scope.StaffAdvancePaymentId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffAdvancePaymentList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
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
