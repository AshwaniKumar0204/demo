(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookListController', BookListController);

    BookListController.$inject = ["$scope", "adminService", "Factory", "ConstantData"];

    function BookListController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookStatusList = ConstantData.BookStatus;
        $scope.$watch('formBook.$valid', function (value) {
            $scope.IsFormBookValid = value;
        })

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

        $scope.getBookSubjectList = function () {
            $scope.dataLoading = true;
            adminService.getBookSubjectList(1)
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

        $scope.afterSubjectTypeSelected = function (obj) {
            if (obj != null & obj != undefined) {
                $scope.Book.BookSubjectId = obj.originalObject.BookSubjectId;
            } else {
                $scope.Book.BookSubjectId = null;
            }
        }

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }

        $scope.getClassList();
        $scope.getAuthorList = function () {
            $scope.dataLoading = true;
            adminService.getAuthorList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AuthorList = response.data.AuthorList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        //$scope.getSubjectTypeList = function () {
        //    $scope.dataLoading = true;
        //    adminService.getSubjectTypeList()
        //        .then(function (response) {
        //            if (response.data.Message == 'Success') {
        //                $scope.SubjectTypeList = response.data.SubjectTypeList;
        //            } else {
        //                toastr.error(response.data.Message);
        //            }
        //            $scope.dataLoading = false;
        //        }, function (err) {
        //            toastr.error("Some error occured while fetching data");
        //            $scope.dataLoading = false;
        //        })
        //}

        $scope.getPublisherList = function () {
            $scope.dataLoading = true;
            adminService.getPublisherList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PublisherList = response.data.PublisherList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }


        $scope.getBookList = function () {
            $scope.dataLoading = true;
            adminService.getBookList($scope.BookStatus, $scope.ClassNo, $scope.BookTypeId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookList = response.data.BookList;
                        $scope.getAuthorList();
                        //$scope.getSubjectTypeList();
                        $scope.getPublisherList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookList();

        $scope.resetBookForm = function () {
            $scope.IsBookSubmitted = false;
            $scope.Book = {};
            document.getElementById("AuthorAnguComplete_value").value = null;
            document.getElementById("PublisherAnguComplete_value").value = null;
            document.getElementById("SubjectTypeAnguComplete_value").value = null;
            $scope.formBook.$setPristine();
            $scope.formBook.$setUntouched();
        }

        $scope.editBook = function (book) {
            $scope.resetBookForm();
            $scope.Book = book;
            $scope.Book.PublishingYear = parseInt(book.PublishingYear);
            $scope.Book.TotalPages = parseInt(book.TotalPages);
            document.getElementById("AuthorAnguComplete_value").value = book.Author;
            document.getElementById("PublisherAnguComplete_value").value = book.Publisher;
            document.getElementById("SubjectTypeAnguComplete_value").value = book.SubjectName;
            $('#modal_new_book').modal('show');
        }

        $scope.saveBook = function () {
            $scope.IsBookSubmitted = true;
            if ($scope.IsFormBookValid) {
                var AuthorName = document.getElementById("AuthorAnguComplete_value").value;
                var PublisherName = document.getElementById("PublisherAnguComplete_value").value;
                var SubjectTypeName = document.getElementById("SubjectTypeAnguComplete_value").value;
                if (AuthorName == null || AuthorName == undefined || AuthorName == "") {
                    toastr.error("Author is required!!");
                    return;
                }

                var BookModel = {
                    BookId: $scope.Book.BookId,
                    BookName: $scope.Book.BookName,
                    Author: AuthorName,
                    Edition: $scope.Book.Edition,
                    Publisher: PublisherName,
                    PublishingYear: $scope.Book.PublishingYear,
                    SubjectType: SubjectTypeName,
                    BookSubjectId: $scope.Book.BookSubjectId,
                    TotalPages: $scope.Book.TotalPages,
                    Volume: $scope.Book.Volume,
                    ClassNo: $scope.Book.ClassNo,
                    BookTypeId: $scope.Book.BookTypeId,
                    BookStatus: $scope.Book.BookStatus,
                }
                $scope.dataLoading = true;
                adminService.saveBook(BookModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Book Detail Updated Successfully.");
                            $scope.getBookList();
                            $('#modal_new_book').modal('hide');
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

        $scope.deleteConfirmation = function (BookId) {
            $scope.BookId = BookId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteBook = function () {
            $scope.dataLoading = true;
            adminService.deleteBook($scope.BookId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Book deleted successfully.", "Deleted Successfully.")
                        $scope.BookId = null;
                        $scope.getBookList();
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

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions - 1];
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
