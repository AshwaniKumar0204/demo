(function () {
    'use strict';

    angular
        .module('app')
        .controller('PrintController', PrintController);

    PrintController.$inject = ['$scope'];

    function PrintController($scope) {
        $scope.printPage = function () {
            $scope.pageSize = sessionStorage.getItem('page_size');
            $scope.pageLayout = sessionStorage.getItem('page_layout');
            var content = sessionStorage.getItem('print-page');
            document.getElementById('printPage').innerHTML = content;
            window.print();
        }
        $scope.printPage();
    }
})();
