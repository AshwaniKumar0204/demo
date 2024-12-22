(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageBookAccessionController', ManageBookAccessionController);

    ManageBookAccessionController.$inject = ["$scope", "adminService", "loadDataService", "Factory"];

    function ManageBookAccessionController($scope, adminService, loadDataService, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookAccession = {};

        $scope.getBookPositionList = function () {
            $scope.dataLoading = true;
            adminService.getBookPositionList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookPositionList = response.data.BookPositionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookPositionList();

        $scope.getBookAccessionDetail = function () {
            if ($scope.AccessionNo == null || $scope.AccessionNo == undefined) {
                $scope.BookAccession = {};
                toastr.error("Accession is required!")
                return;
            }
            $scope.dataLoading = true;
            adminService.getBookAccessionDetail($scope.AccessionNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookAccession = response.data.BookAccession;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('formBookAccession.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBookAccession = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {


                var BookAccessionModel = {
                    BookAccessionId: $scope.BookAccession.BookAccessionId,
                    BookPosition: $scope.BookAccession.BookPosition,
                    BookId: $scope.BookAccession.BookId,
                    AccessionNo: $scope.BookAccession.AccessionNo,
                }
                $scope.dataLoading = true;
                adminService.saveBookAccession(BookAccessionModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Book Detail Updated is Successful.");
                            $scope.resetForm();
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
            $scope.BookAccession = {};
            $scope.AccessionNo = null;
            $scope.formBookAccession.$setPristine();
            $scope.formBookAccession.$setUntouched();
        }
    }
})();
