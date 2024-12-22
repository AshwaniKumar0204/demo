(function () {
    'use strict';

    angular
        .module('app')
        .controller('PurchaseStoreItemListController', PurchaseStoreItemListController);

    PurchaseStoreItemListController.$inject = ["$scope", "adminService", "Factory", "loadDataService", "ConstantData"];

    function PurchaseStoreItemListController($scope, adminService, Factory, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.FromDate = new Date();
        $scope.ToDate = new Date();

        $scope.getSupplierList = function () {
            $scope.dataLoading = true;
            adminService.getSupplierList(1,2)
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
        $scope.getPurchaseStoreItemList = function (PurchaseId) {
            $scope.dataLoading = true;
            adminService.getPurchaseStoreItemList(PurchaseId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PurchaseStoreItemList = response.data.PurchaseStoreItemList;
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
            adminService.getPurchaseList($scope.FromDate, $scope.ToDate, $scope.SupplierId, 2)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PurchaseList = response.data.PurchaseList;
                        $scope.TotalAmount = 0;
                        $scope.TotalTaxableAmount = 0;
                        $scope.TotalTax = 0;
                        angular.forEach($scope.PurchaseList, function (purchase, key1) {
                            $scope.TotalAmount += purchase.GrandTotal;
                            $scope.TotalTaxableAmount += purchase.TotalAmount;
                            $scope.TotalTax += purchase.TotalCGST + purchase.TotalSGST + purchase.TotalIGST;
                        })
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
