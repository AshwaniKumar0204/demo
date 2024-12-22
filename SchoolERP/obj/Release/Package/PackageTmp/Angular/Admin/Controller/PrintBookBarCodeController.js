(function () {
    'use strict';

    angular
        .module('app')
        .controller('PrintBookBarCodeController', PrintBookBarCodeController);

    PrintBookBarCodeController.$inject = ['$scope', 'adminService'];

    function PrintBookBarCodeController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.$watch('formBook.$valid', function (value) {
            $scope.isFormBookValid = value;
        })

        $scope.printBarcode = function (ReportNo) {
            $scope.IsSubmitted = true;
            if ($scope.isFormBookValid) {
                window.open("/print/PrintBookBarCode?FromAccessionNo=" + $scope.FromAccessionNo + "&ToAccessionNo=" + $scope.ToAccessionNo + "&ReportNo=" + ReportNo + "&Quantity=" + $scope.Quantity + "&ClassNo=" + $scope.ClassNo);
            } else {
                toastr.error("Fill all the required fields !!", "Validation Error");
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


        $scope.changeAccessionNo = function () {
            $scope.selectedBookAccession = {};
            if ($scope.AccessionNo) {
                for (var i = 0; i < $scope.BookAccessionList.length; i++) {
                    if ($scope.BookAccessionList[i].AccessionNo == $scope.AccessionNo) {
                        $scope.selectedBookAccession = $scope.BookAccessionList[i];
                        break;
                    }
                }
            }
        }

        $scope.$watch('formBookAccession.$valid', function (value) {
            $scope.isFormBookAccessionValid = value;
        })

        $scope.AccessionList = [];
        $scope.selectedBookAccession = [];
        $scope.addAccession = function () {
            $scope.IsSubmittedAccession = true;
            if ($scope.isFormBookAccessionValid) {
                if ($scope.selectedBookAccession.BookName == null || $scope.selectedBookAccession.BookName == undefined) {
                    toastr.error("This Accession No is not Exist!!");
                    return;
                }

                var obj = { id: $scope.AccessionNo, NoOfQuantity: $scope.NoOfQuantity, BookName: $scope.selectedBookAccession.BookName };
                $scope.AccessionList.push(obj);
                $scope.AccessionNo = null;
                $scope.NoOfQuantity = null;
                $scope.selectedBookAccession = [];
                $scope.IsSubmittedAccession = false;
                $scope.formBookAccession.$setUntouched();
                $scope.formBookAccession.$setPristine();
            } else {
                toastr.error("Fill all the required fields !!", "Validation Error");
            }
        }

        $scope.printBookBarCodeWithAccessionNo = function () {
            $scope.dataLoading = true;
            adminService.setSessionForPrintBarcode($scope.AccessionList)
                .then(function (response) {
                    console.log(response);
                    if (response.data.Message == 'Success') {
                        window.open('/print/PrintBookBarCodeWithAccessionNo');
                        $scope.AccessionList = [];
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
