(function () {
    'use strict';

    angular
        .module('app')
        .controller('DCRUserwiseController', DCRUserwiseController);

    DCRUserwiseController.$inject = ['$scope', 'loadDataService'];

    function DCRUserwiseController($scope, loadDataService) {
        $scope.checkSysytemLogin();
        //$scope.FormDate = new Date();
        //$scope.ToDate = new Date();
        $scope.$watch('formDCR.$valid', function (value) {
            $scope.IsFormDCRValid = value;
        })

        $scope.printFeeCollectionReport = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormDCRValid) {
                var FromDate = loadDataService.getDateDDMMYYYY($scope.FromDate);
                var ToDate = loadDataService.getDateDDMMYYYY($scope.ToDate);
                window.open('/admin/PrintUserDCR?FromDate=' + FromDate + '&ToDate=' + ToDate);
            } else {
                toastr.error("Fill all fields !!", "Validation Error");
            }
        }
    }
})();
