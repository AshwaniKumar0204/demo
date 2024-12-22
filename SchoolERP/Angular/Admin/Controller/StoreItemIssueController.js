(function () {
    'use strict';

    angular
        .module('app')
        .controller('StoreItemIssueController', StoreItemIssueController);

    StoreItemIssueController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function StoreItemIssueController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.IssuedByList = [{ Key: 1, Value: 'Student' }, { Key: 2, Value: 'Staff' }, { Key: 3, Value: 'Others' }];
        $scope.IssuedBy = 1;
        $scope.StoreItemIssue = {};
        $scope.selectedStaff = {};
        $scope.selectedStudent = {};

        //MiscellaneousStaff
        $scope.getMiscellaneousStaffList = function (MiscellaneousStaffId) {
            $scope.dataLoading = true;
            adminService.getMiscellaneousStaffList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MiscellaneousStaffList = response.data.MiscellaneousStaffList;
                        if (MiscellaneousStaffId > 0) {
                            for (var i = 0; i < $scope.MiscellaneousStaffList.length; i++) {
                                if ($scope.MiscellaneousStaffList[i].MiscellaneousStaffId == MiscellaneousStaffId) {
                                    $scope.SelectedMiscellaneousStaff = $scope.MiscellaneousStaffList[i];
                                    break;
                                }
                            }
                            $scope.$broadcast('angucomplete-alt:changeInput', 'MiscellaneousStaffAnguComplete', $scope.SelectedMiscellaneousStaff.StaffName);
                            $('#modal_new_MiscellaneousStaff').modal('hide');
                        }
                    } else {
                        toastr.error(response.data.Message);
                    }
                        $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getMiscellaneousStaffList(0);

        $scope.$watch('formMiscellaneousStaff.$valid', function (value) {
            $scope.IsFormMiscellaneousStaffValid = value;
        })

        $scope.afterMiscellaneousStaffSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedMiscellaneousStaff = selected.originalObject;
                $scope.getStoreItemIssueList();
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
                $scope.selectedStudent = {};
                document.getElementById("StudentAutoComplete_value").value = "";
            } else {
                $scope.SelectedMiscellaneousStaff = {};
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
                $scope.selectedStudent = {};
                document.getElementById("StudentAutoComplete_value").value = "";
            }
        }

        $scope.inputChangeMiscellaneousStaff = function () {
            $scope.selectedStaff = {};
            document.getElementById("StaffAutoComplete_value").value = "";
            $scope.selectedStudent = {};
            document.getElementById("StudentAutoComplete_value").value = "";
        }

        $scope.resetMiscellaneousStaffForm = function () {
            $scope.IsMiscellaneousStaffSubmitted = false;
            $scope.MiscellaneousStaff = {};
            $scope.formMiscellaneousStaff.$setPristine();
            $scope.formMiscellaneousStaff.$setUntouched();
        }

        $scope.newMiscellaneousStaff = function () {
            $scope.resetMiscellaneousStaffForm();
            $('#modal_new_MiscellaneousStaff').modal('show');
        }

        $scope.saveMiscellaneousStaff = function () {
            $scope.IsMiscellaneousStaffSubmitted = true;
            if ($scope.IsFormMiscellaneousStaffValid) {
                var MiscellaneousStaffModel = {
                    MiscellaneousStaffId: $scope.MiscellaneousStaff.MiscellaneousStaffId,
                    StaffName: $scope.MiscellaneousStaff.StaffName,
                    MobileNo: $scope.MiscellaneousStaff.MobileNo,
                    Designation: $scope.MiscellaneousStaff.Designation,
                    Address: $scope.MiscellaneousStaff.Address,
                }
                $scope.dataLoading = true;
                adminService.saveMiscellaneousStaff(MiscellaneousStaffModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("New Staff Created Successfully.");
                            $scope.getMiscellaneousStaffList(response.data.MiscellaneousStaffId)
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


        //Student
        $scope.getSearchStudentList = function () {
            $scope.dataLoading = true;
            adminService.getSearchStudentList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentList = response.data.StudentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchStudentList();

        //Staff
        $scope.getSearchStaffList = function () {
            $scope.dataLoading = true;
            adminService.getSearchStaffList(1)
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
        $scope.getSearchStaffList();

        //Store Item Issue
        $scope.getStoreItemList = function () {
            $scope.dataLoading = true;
            adminService.getStoreItemList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StoreItemList = response.data.StoreItemList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStoreItemList();

        $scope.getStoreItemIssueList = function () {
            var StudentId = 0;
            if ($scope.selectedStudent.StudentId > 0)
                StudentId = $scope.selectedStudent.StudentId;
            var StaffId = 0;
            if ($scope.selectedStaff.StaffId > 0)
                StaffId = $scope.selectedStaff.StaffId;
            var MiscellaneousStaffId = 0;
            if ($scope.SelectedMiscellaneousStaff.MiscellaneousStaffId > 0)
                MiscellaneousStaffId = $scope.SelectedMiscellaneousStaff.MiscellaneousStaffId;

            $scope.dataLoading = true;
            adminService.getStoreItemIssueList(StudentId, StaffId, MiscellaneousStaffId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StoreItemIssueList = response.data.StoreItemIssueList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.selectedStudent = selected.originalObject;
                $scope.getStoreItemIssueList();
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
                $scope.SelectedMiscellaneousStaff = {};
                document.getElementById("MiscellaneousStaffAnguComplete").value = "";
            } else {
                $scope.selectedStudent = {};
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
                $scope.SelectedMiscellaneousStaff = {};
                document.getElementById("MiscellaneousStaffAnguComplete").value = "";
            }
        }

        $scope.inputChangeStudent = function () {
            $scope.selectedStaff = {};
            document.getElementById("StaffAutoComplete_value").value = "";
            $scope.SelectedMiscellaneousStaff = {};
            document.getElementById("MiscellaneousStaffAnguComplete").value = "";
        }

        $scope.afterStaffSelected = function (selected) {
            if (selected != undefined) {
                $scope.selectedStaff = selected.originalObject;
                $scope.getStoreItemIssueList();
                $scope.selectedStudent = {};
                document.getElementById("StudentAutoComplete_value").value = "";
                $scope.SelectedMiscellaneousStaff = {};
                document.getElementById("MiscellaneousStaffAnguComplete").value = "";
            } else {
                $scope.selectedStudent = {};
                $scope.selectedStaff = {};
                document.getElementById("StudentAutoComplete_value").value = "";
                $scope.SelectedMiscellaneousStaff = {};
                document.getElementById("MiscellaneousStaffAnguComplete").value = "";
            }
        }

        $scope.inputChangeStaff = function () {
            $scope.selectedStudent = {};
            document.getElementById("StudentAutoComplete_value").value = "";
            $scope.SelectedMiscellaneousStaff = {};
            document.getElementById("MiscellaneousStaffAnguComplete").value = "";
        }

        $scope.afterStoreItemSelected = function (selected) {
            if (selected != undefined) {
                $scope.selectedStoreItem = selected.originalObject;
            } else {
                $scope.selectedStoreItem = {};
            }
        }

        $scope.$watch('formStoreItemIssue.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveStoreItemIssue = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                //Validations
                if ($scope.selectedStoreItem.StoreItemId == null || $scope.selectedStoreItem.StoreItemId == undefined) {
                    document.getElementById("StoreItemAutoComplete_value").value = null;
                    toastr.error("Item is required.");
                    return;
                }

                var StoreItemIssueModel = {
                    StoreItemId: $scope.selectedStoreItem.StoreItemId,
                    StoreItemIssueId: $scope.StoreItemIssue.StoreItemIssueId,
                    StudentId: $scope.selectedStudent.StudentId,
                    MiscellaneousStaffId: $scope.SelectedMiscellaneousStaff.MiscellaneousStaffId,
                    StaffId: $scope.selectedStaff.StaffId,
                    Remarks: $scope.StoreItemIssue.Remarks,
                    Quantity: $scope.StoreItemIssue.Quantity,
                    DateOfIssue: loadDataService.getDateTime($scope.StoreItemIssue.DateOfIssue)
                }
                $scope.dataLoading = true;
                adminService.saveStoreItemIssue(StoreItemIssueModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StoreItemIssueModel.StoreItemIssueId > 0)
                                toastr.success("New Item Issued Successfully.");
                            else
                                toastr.success("Issued Item Detail Updated Successfully.");
                            $scope.resetForm();
                            $scope.getStoreItemIssueList();
                            $scope.getStoreItemList();
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
            $scope.StoreItemIssue = [];
            $scope.StoreItemIssue.DateOfIssue = new Date();
            document.getElementById("StoreItemAutoComplete_value").value = null;
            $scope.selectedStoreItem = {};
            $scope.formStoreItemIssue.$setPristine();
            $scope.formStoreItemIssue.$setUntouched();
        }

        $scope.newStoreItemIssue = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStoreItemIssue = function (StoreItemIssueModel) {
            $scope.resetForm();
            $scope.StoreItemIssue = StoreItemIssueModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StoreItemIssueId) {
            $scope.StoreItemIssueId = StoreItemIssueId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStoreItemIssue = function () {
            $scope.dataLoading = true;
            adminService.deleteStoreItemIssue($scope.StoreItemIssueId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Selected record deleted successfully.", "Deleted Successfully.");
                        $scope.getStoreItemIssueList();
                        $scope.StoreItemIssueId = null;
                        $scope.getStoreItemList();
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

        $scope.returnConfirmation = function (StoreItemIssue) {
            $scope.ReturnStoreItemIssue = StoreItemIssue;
            $('#modal_return_confirmation').modal('show');
        }

        $scope.returnStoreItemIssue = function () {
            $scope.dataLoading = true;
            adminService.storeItemReturn($scope.ReturnStoreItemIssue)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Selected record returnd successfully.", "returnd Successfully.");
                        $scope.getStoreItemIssueList();
                        $scope.ReturnStoreItemIssue = {};
                        $scope.getStoreItemList();
                        $('#modal_return_confirmation').modal('hide');
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
