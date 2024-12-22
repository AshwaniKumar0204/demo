(function () {
    'use strict';

    angular
        .module('app')
        .controller('PurchaseListController', PurchaseListController);

    PurchaseListController.$inject = ["$scope", "adminService", "Factory", "loadDataService", "ConstantData"];

    function PurchaseListController($scope, adminService, Factory, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();

        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(1,1)
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

        $scope.Purchase = {};
        $scope.getPurchaseBookList = function (PurchaseId) {
            $scope.dataLoading = true;
            adminService.getPurchaseBookList(PurchaseId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PurchaseBookList = response.data.PurchaseBookList;
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
                        $('#modal_purchase_list').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getPurchaseList = function () {
            $scope.dataLoading = true;
            adminService.getPurchaseList($scope.FromDate, $scope.ToDate, $scope.SupplierId,1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PurchaseList = response.data.PurchaseList;
                        $scope.TotalBillAmount = 0;
                        angular.forEach($scope.PurchaseList, function (purchase, key1) { $scope.TotalBillAmount += purchase.GrandTotal; })
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getPurchaseList();

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
