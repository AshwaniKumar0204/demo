(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookIssueController', BookIssueController);

    BookIssueController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function BookIssueController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookIssue = {};
        $scope.selectedStaff = {};
        $scope.selectedStudent = {};

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

        $scope.getSearchBookAccessionList = function () {
            $scope.dataLoading = true;
            adminService.getSearchBookAccessionList(1, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookAccessionList = response.data.BookAccessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchBookAccessionList();

        $scope.getBookIssueTypeList = function () {
            $scope.dataLoading = true;
            adminService.getBookIssueTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookIssueTypeList = response.data.BookIssueTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookIssueTypeList();

        $scope.getBookIssueList = function () {
            var StudentId = 0;
            if ($scope.selectedStudent.StudentId > 0)
                StudentId = $scope.selectedStudent.StudentId;
            var StaffId = 0;
            if ($scope.selectedStaff.StaffId > 0)
                StaffId = $scope.selectedStaff.StaffId;

            $scope.dataLoading = true;
            adminService.getBookIssueList(StudentId, StaffId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookIssueList = response.data.BookIssueList;
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
                $scope.getBookIssueList();
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
            } else {
                $scope.selectedStudent = {};
                $scope.selectedStaff = {};
                document.getElementById("StaffAutoComplete_value").value = "";
            }
        }

        $scope.localSearch = function (str, accessionList) {
            var matches = [];
            accessionList.forEach(function (person) {
                if (person.AccessionNo.toString().startsWith(str.toString()) || person.AccessionNo.toString() == str.toString()) {
                    matches.push(person);
                }
            });
            return matches;
        };

        $scope.inputChangeStudent = function () {
            $scope.selectedStaff = {};
            document.getElementById("StaffAutoComplete_value").value = "";
        }

        $scope.afterStaffSelected = function (selected) {
            if (selected != undefined) {
                $scope.selectedStaff = selected.originalObject;
                $scope.getBookIssueList();
                $scope.selectedStudent = {};
                document.getElementById("StudentAutoComplete_value").value = "";
            } else {
                $scope.selectedStudent = {};
                $scope.selectedStaff = {};
                document.getElementById("StudentAutoComplete_value").value = "";
            }
        }

        $scope.inputChangeStaff = function () {
            $scope.selectedStudent = {};
            document.getElementById("StudentAutoComplete_value").value = "";
        }

        $scope.afterBookAccessionSelected = function (selected) {
            if (selected != undefined) {
                $scope.selectedBookAccession = selected.originalObject;
            } else {
                $scope.selectedBookAccession = {};
            }
        }

        $scope.changeAccessionNo = function () {
            $scope.selectedBookAccession = {};
            if ($scope.BookIssue.AccessionNo) {
                for (var i = 0; i < $scope.BookAccessionList.length; i++) {
                    if ($scope.BookAccessionList[i].AccessionNo == $scope.BookIssue.AccessionNo) {
                        $scope.selectedBookAccession = $scope.BookAccessionList[i];
                        break;
                    }
                }
            }
        }

        $scope.$watch('formBookIssue.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBookIssue = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                //Validations
                if ($scope.selectedBookAccession.BookAccessionId == null || $scope.selectedBookAccession.BookAccessionId == undefined) {
                    //document.getElementById("BookAccessionAutoComplete_value").value = null;
                    toastr.error("Book is required.");
                    return;
                }

                var BookIssueModel = {
                    BookAccessionId: $scope.selectedBookAccession.BookAccessionId,
                    BookIssueId: $scope.BookIssue.BookIssueId,
                    StudentId: $scope.selectedStudent.StudentId,
                    StaffId: $scope.selectedStaff.StaffId,
                    IssueRemarks: $scope.BookIssue.IssueRemarks,
                    BookIssueType: $scope.BookIssue.BookIssueType,
                    DateOfIssue: loadDataService.getDateTime($scope.BookIssue.DateOfIssue),
                    DueDate: loadDataService.getDateTime($scope.BookIssue.DueDate)
                }
                $scope.dataLoading = true;
                adminService.saveBookIssue(BookIssueModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (BookIssueModel.BookIssueId > 0)
                                toastr.success("New Book Issued Successfully.");
                            else
                                toastr.success("Issued Book Detail Updated Successfully.");
                            $scope.resetForm();
                            $scope.getBookIssueList();
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
            $scope.BookIssue = [];
            $scope.BookIssue.DateOfIssue = new Date();
            //document.getElementById("BookAccessionAutoComplete_value").value = null;
            $scope.selectedBookAccession = {};
            $scope.formBookIssue.$setPristine();
            $scope.formBookIssue.$setUntouched();
        }

        $scope.newBookIssue = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editBookIssue = function (BookIssueModel) {
            $scope.resetForm();
            $scope.BookIssue = BookIssueModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (BookIssueId) {
            $scope.BookIssueId = BookIssueId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteBookIssue = function () {
            $scope.dataLoading = true;
            adminService.deleteBookIssue($scope.BookIssueId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Selected record deleted successfully.", "Deleted Successfully.");
                        $scope.getBookIssueList();
                        $scope.BookIssueId = null;
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
