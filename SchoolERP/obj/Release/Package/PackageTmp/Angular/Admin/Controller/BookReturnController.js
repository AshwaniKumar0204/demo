(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookReturnController', BookReturnController);

    BookReturnController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function BookReturnController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookIssue = {};
        $scope.searchBookIssueList = function () {
            $scope.dataLoading = true;
            adminService.searchBookIssueList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookAccessionList = response.data.BookIssueList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.searchBookIssueList();


        $scope.localSearch = function (str, accessionList) {
            var matches = [];
            accessionList.forEach(function (person) {
                if (person.AccessionNo.toString().startsWith(str.toString()) || person.AccessionNo.toString() == str.toString()) {
                    matches.push(person);
                }
            });
            return matches;
        };

        $scope.changeAccessionNo = function () {
            $scope.BookIssue = {};
            if ($scope.AccessionNo) {
                for (var i = 0; i < $scope.BookAccessionList.length; i++) {
                    if ($scope.BookAccessionList[i].AccessionNo == $scope.AccessionNo) {
                        $scope.BookIssue = $scope.BookAccessionList[i];
                        $scope.BookIssue.ReturnDate = new Date();
                        if ($scope.BookIssue.DueDays > 0) {
                            $scope.BookIssue.FineAmount = $scope.BookIssue.DueDays * $scope.BookIssue.FinePerDay;
                        }
                        break;
                    }
                }
            }
        }

        $scope.afterBookAccessionSelected = function (obj) {
            if (obj != null) {
                $scope.resetForm();
                $scope.BookIssue = obj.originalObject;
                $scope.BookIssue.ReturnDate = new Date();
                if ($scope.BookIssue.DueDays > 0) {
                    $scope.BookIssue.FineAmount = $scope.BookIssue.DueDays * $scope.BookIssue.FinePerDay;
                }
            } else {
                $scope.BookIssue = {};
            }
        }

        $scope.$watch('formBookIssue.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.bookReturn = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                //Validations
                if ($scope.BookIssue.BookIssueId == null || $scope.BookIssue.BookIssueId == undefined) {
                    //document.getElementById("BookAccessionAutoComplete_value").value = null;
                    toastr.error("Accession No is required.");
                    return;
                }

                var BookIssueModel = {
                    BookIssueId: $scope.BookIssue.BookIssueId,
                    FineAmount: $scope.BookIssue.FineAmount,
                    ReturnRemarks: $scope.BookIssue.ReturnRemarks,
                    ReturnDate: loadDataService.getDateTime($scope.BookIssue.ReturnDate),
                }
                $scope.dataLoading = true;
                adminService.bookReturn(BookIssueModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Return of book is Successful.");
                            $scope.resetForm();
                            $scope.searchBookIssueList();
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

        $scope.bookReissue = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                //Validations
                if ($scope.BookIssue.BookIssueId == null || $scope.BookIssue.BookIssueId == undefined) {
                    //document.getElementById("BookAccessionAutoComplete_value").value = null;
                    toastr.error("Accession No is required.");
                    return;
                }

                var BookIssueModel = {
                    BookIssueId: $scope.BookIssue.BookIssueId,
                    FineAmount: $scope.BookIssue.FineAmount,
                    ReturnRemarks: $scope.BookIssue.ReturnRemarks,
                    ReturnDate: loadDataService.getDateTime($scope.BookIssue.ReturnDate),
                }
                $scope.dataLoading = true;
                adminService.bookReissue(BookIssueModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Reissue of book is successful.");
                            $scope.resetForm();
                            $scope.searchBookIssueList();
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
            $scope.BookIssue = {};
            $scope.AccessionNo = null;
            //document.getElementById("BookAccessionAutoComplete_value").value = null;
            $scope.formBookIssue.$setPristine();
            $scope.formBookIssue.$setUntouched();
        }
    }
})();
