(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookPurchaseController', BookPurchaseController);

    BookPurchaseController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function BookPurchaseController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Purchase = {};
        $scope.Purchase.BillDate = new Date();
        $scope.PurchaseBook = {};
        $scope.Book = {};
        $scope.PurchaseBookList = [];
        $scope.SelectedBook = {};

        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(1, 1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SupplierList = response.data.SupplierList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSupplierList();

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

        //Book
        $scope.getSearchBookList = function (BookId) {
            $scope.dataLoading = true;
            adminService.getSearchBookListClasswise(1, $scope.PurchaseBook.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookList = response.data.BookList;
                        $scope.getAuthorList();
                        //$scope.getSubjectTypeList();
                        $scope.getPublisherList();
                        if (BookId > 0) {
                            for (var i = 0; i < $scope.BookList.length; i++) {
                                if ($scope.BookList[i].BookId == BookId) {
                                    $scope.SelectedBook = $scope.BookList[i];
                                    break;
                                }
                            }
                            $scope.$broadcast('angucomplete-alt:changeInput', 'BookAnguComplete', $scope.SelectedBook.SearchBook);
                            $('#modal_new_book').modal('hide');
                        } else {
                            $scope.$broadcast('angucomplete-alt:clearInput', 'BookAnguComplete');
                            $scope.SearchBook = {};
                        }

                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getSearchBookList(0);

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

        $scope.$watch('formBook.$valid', function (value) {
            $scope.IsFormBookValid = value;
        })

        $scope.afterBookSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedBook = selected.originalObject;
            } else {
                $scope.SelectedBook = {};
            }
        }

        $scope.resetBookForm = function () {
            $scope.IsBookSubmitted = false;
            $scope.Book = {};
            document.getElementById("AuthorAnguComplete_value").value = null;
            document.getElementById("PublisherAnguComplete_value").value = null;
            document.getElementById("SubjectTypeAnguComplete_value").value = null;
            $scope.formBook.$setPristine();
            $scope.formBook.$setUntouched();
        }

        $scope.newBook = function () {
            $scope.resetBookForm();
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
                    BookName: $scope.Book.BookName,
                    Author: AuthorName,
                    Edition: $scope.Book.Edition,
                    Publisher: PublisherName,
                    PublishingYear: $scope.Book.PublishingYear,
                    BookSubjectId: $scope.Book.BookSubjectId,
                    SubjectType: SubjectTypeName,
                    TotalPages: $scope.Book.TotalPages,
                    Volume: $scope.Book.Volume,
                    ClassNo: $scope.Book.ClassNo,
                    BookTypeId: $scope.Book.BookTypeId,
                    BookStatus: 1,
                }
                $scope.dataLoading = true;
                adminService.saveBook(BookModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("New Book Created Successfully.");
                            var BookId = response.data.BookId;
                            $scope.PurchaseBook.ClassNo = $scope.Book.ClassNo;
                            $scope.getSearchBookList(BookId);
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


        //PurchaseBook
        $scope.$watch('formPurchaseBook.$valid', function (value) {
            $scope.IsFormPurchaseBookValid = value;
        })

        $scope.resetPurchaseBookForm = function () {
            $scope.IsPurchaseBookSubmitted = false;
            $scope.PurchaseBook = {};
            $scope.SelectedBook = {};
            $scope.$broadcast('angucomplete-alt:clearInput', 'BookAnguComplete');
            $scope.formPurchaseBook.$setPristine();
            $scope.formPurchaseBook.$setUntouched();
        }

        $scope.addPurchaseBook = function () {
            $scope.IsPurchaseBookSubmitted = true;
            if (!$scope.IsFormPurchaseBookValid) {
                toastr.error("Fill all required fields.");
                return;
            }

            if ($scope.SelectedBook.BookId == null || $scope.SelectedBook.BookId == undefined) {
                toastr.error("No book is selected.");
                return;
            }

            for (var i = 0; i < $scope.PurchaseBookList.length; i++) {
                if ($scope.PurchaseBookList[i].BookId == $scope.SelectedBook.BookId) {
                    toastr.error("This book is already added");
                    return;
                }
            }
            var selectedClass = {};
            for (var i = 0; i < $scope.ClassList.length; i++) {
                if ($scope.ClassList[i].ClassNo == $scope.PurchaseBook.ClassNo) {
                    selectedClass = $scope.ClassList[i];
                    break;
                }
            }
            var DiscountRate = 0;
            if ($scope.PurchaseBook.DiscountRate != null && $scope.PurchaseBook.DiscountRate != undefined) {
                var DiscountRate = $scope.PurchaseBook.DiscountRate;
            }
            var PurchaseBookModel = {
                BookId: $scope.SelectedBook.BookId,
                SearchBook: $scope.SelectedBook.SearchBook,
                ClassName: selectedClass.ClassName,
                ClassNo: selectedClass.ClassNo,
                BookPrice: $scope.PurchaseBook.BookPrice,
                Quantity: $scope.PurchaseBook.Quantity,
                GrossAmount: $scope.PurchaseBook.BookPrice * $scope.PurchaseBook.Quantity,
                DiscountRate: DiscountRate,
                Remarks: $scope.PurchaseBook.Remarks,
            }
            PurchaseBookModel.DiscountAmount = PurchaseBookModel.GrossAmount * PurchaseBookModel.DiscountRate / 100;
            PurchaseBookModel.TotalAmount = PurchaseBookModel.GrossAmount - PurchaseBookModel.DiscountAmount;
            PurchaseBookModel.BookCost = PurchaseBookModel.TotalAmount / PurchaseBookModel.Quantity;
            $scope.PurchaseBookList.push(PurchaseBookModel);
            $scope.calculateTotal();
            $scope.resetPurchaseBookForm();
        }

        $scope.deletePurchaseBook = function (index) {
            $scope.PurchaseBookList.splice(index, 1);
        }

        $scope.calculateTotal = function () {
            $scope.Purchase.TotalQuantity = 0;
            $scope.Purchase.TotalGrossAmount = 0;
            $scope.Purchase.TotalDiscountAmount = 0;
            $scope.Purchase.GrandTotal = 0;
            angular.forEach($scope.PurchaseBookList, function (purchaseBook, key1) {
                $scope.Purchase.TotalQuantity += purchaseBook.Quantity;
                $scope.Purchase.TotalGrossAmount += purchaseBook.GrossAmount;
                $scope.Purchase.TotalDiscountAmount += purchaseBook.DiscountAmount;
                $scope.Purchase.GrandTotal += purchaseBook.TotalAmount;
            })
        }

        //Purchase
        $scope.$watch('formPurchase.$valid', function (value) {
            $scope.IsFormPurchaseValid = value;
        })

        $scope.SavePurchase = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormPurchaseValid) {
                var PurchaseModel = {
                    PurchaseId: $scope.Purchase.PurchaseId,
                    SupplierId: $scope.Purchase.SupplierId,
                    BillNo: $scope.Purchase.BillNo,
                    BillDate: loadDataService.getDateTime($scope.Purchase.BillDate),
                    PurchaseBookList: $scope.PurchaseBookList,
                }
                $scope.dataLoading = true;
                adminService.savePurchase(PurchaseModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Purchase Successfully.");
                            $scope.resetPurchaseForm();
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

        $scope.resetPurchaseForm = function () {
            $scope.IsSubmitted = false;
            $scope.Purchase = {};
            $scope.Purchase.BillDate = new Date();
            $scope.PurchaseBookList = [];
            $scope.formPurchase.$setPristine();
            $scope.formPurchase.$setUntouched();
            $scope.resetPurchaseBookForm();
        }
    }
})();

