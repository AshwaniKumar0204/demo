(function () {
    'use strict';

    angular
        .module('app')
        .controller('NewsPaperPurchaseController', NewsPaperPurchaseController);

    NewsPaperPurchaseController.$inject = ['$scope', 'adminService', 'loadDataService', 'ConstantData', '$location'];

    function NewsPaperPurchaseController($scope, adminService, loadDataService, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.PurchaseDate = new Date();

        $scope.getNewPaperListForPurchase = function () {
            $scope.dataLoading = true;
            adminService.getNewPaperListForPurchase($scope.PurchaseDate)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.NewsPaperPurchaseList = response.data.NewsPaperPurchaseList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getNewPaperListForPurchase();

        $scope.saveNewsPaperPurchase = function () {
            angular.forEach($scope.NewsPaperPurchaseList, function (news) {
                news.PurchaseDate = loadDataService.getDateTime(news.PurchaseDate);
            });
            $scope.dataLoading = true;
            adminService.saveNewsPaperPurchase($scope.NewsPaperPurchaseList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Operation Successfull.", "Success");
                        $scope.NewsPaperPurchaseList = [];
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
