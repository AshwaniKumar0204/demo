(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageBookTypeController', ManageBookTypeController);

    ManageBookTypeController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageBookTypeController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookTypeStatusList = ConstantData.BookTypeStatus;

        $scope.BookType = {};
        $scope.BookType.BookTypeStatus = null;

        $scope.getBookTypeList = function () {
            $scope.dataLoading = true;
            adminService.getBookTypeList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookTypeList = response.data.BookTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookTypeList();

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

        $scope.$watch('formBookType.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBookType = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var BookTypeModel = {
                    BookTypeId: $scope.BookType.BookTypeId,
                    BookTypeStatus: $scope.BookType.BookTypeStatus,
                    BookTypeName: $scope.BookType.BookTypeName,
                }

                $scope.dataLoading = true;
                adminService.saveBookType(BookTypeModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (BookTypeModel.BookTypeId > 0)
                                toastr.success("Book Type Updated Successfully.");
                            else
                                toastr.success("New Book Type Created Successfully.");
                            $scope.resetForm();
                            $scope.BookTypeList = response.data.BookTypeList;
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
        $scope.BookType = {};
        $scope.BookType.BookTypeStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.BookType.BookTypeId = null;
            $scope.BookType.BookTypeName = "";
            $scope.formBookType.$setPristine();
            $scope.formBookType.$setUntouched();
        }

        $scope.newBookType = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editBookType = function (BookTypeModel) {
            $scope.resetForm();
            $scope.BookType.BookTypeName = BookTypeModel.BookTypeName;
            $scope.BookType.BookTypeId = BookTypeModel.BookTypeId;
            $scope.BookType.BookTypeStatus = BookTypeModel.BookTypeStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (BookTypeId) {
            $scope.BookTypeId = BookTypeId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteBookType = function () {
            $scope.dataLoading = true;
            adminService.deleteBookType($scope.BookTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Book Type deleted successfully.", "Deleted Successfully.")
                        $scope.BookTypeList = response.data.BookTypeList;
                        $scope.BookTypeId = null;
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
