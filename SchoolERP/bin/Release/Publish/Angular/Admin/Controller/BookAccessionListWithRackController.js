(function () {
    'use strict';

    angular
        .module('app')
        .controller('BookAccessionListWithRackController', BookAccessionListWithRackController);

    BookAccessionListWithRackController.$inject = ["$scope", "adminService", "Factory", "ConstantData"];

    function BookAccessionListWithRackController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getBookAccessionListWithRack = function () {
            $scope.dataLoading = true;
            adminService.getBookAccessionListWithRack($scope.AccessionNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookAccessionListWithRack = response.data.BookAccessionListWithRack;
                        $scope.Book = $scope.BookAccessionListWithRack[0];
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
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
