(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageBookSubjectController', ManageBookSubjectController);

    ManageBookSubjectController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageBookSubjectController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SubjectStatusList = ConstantData.SubjectStatus;

        $scope.BookSubject = {};
        $scope.BookSubject.SubjectStatus = null;

        $scope.getBookSubjectList = function () {
            $scope.dataLoading = true;
            adminService.getBookSubjectList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookSubjectList = response.data.BookSubjectList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookSubjectList();

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

        $scope.$watch('formBookSubject.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBookSubject = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var BookSubjectModel = {
                    BookSubjectId: $scope.BookSubject.BookSubjectId,
                    SubjectStatus: $scope.BookSubject.SubjectStatus,
                    SubjectName: $scope.BookSubject.SubjectName,
                }

                $scope.dataLoading = true;
                adminService.saveBookSubject(BookSubjectModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (BookSubjectModel.BookSubjectId > 0)
                                toastr.success("Subject Updated Successfully.");
                            else
                                toastr.success("New Subject Created Successfully.");
                            $scope.resetForm();
                            $scope.BookSubjectList = response.data.BookSubjectList;
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
        $scope.BookSubject = {};
        $scope.BookSubject.SubjectStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.BookSubject.BookSubjectId = null;
            $scope.BookSubject.SubjectName = "";
            $scope.formBookSubject.$setPristine();
            $scope.formBookSubject.$setUntouched();
        }

        $scope.newBookSubject = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editBookSubject = function (BookSubjectModel) {
            $scope.resetForm();
            $scope.BookSubject.SubjectName = BookSubjectModel.SubjectName;
            $scope.BookSubject.BookSubjectId = BookSubjectModel.BookSubjectId;
            $scope.BookSubject.SubjectStatus = BookSubjectModel.SubjectStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (BookSubjectId) {
            $scope.BookSubjectId = BookSubjectId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteBookSubject = function () {
            $scope.dataLoading = true;
            adminService.deleteBookSubject($scope.BookSubjectId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Subject deleted successfully.", "Deleted Successfully.")
                        $scope.BookSubjectList = response.data.BookSubjectList;
                        $scope.BookSubjectId = null;
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
