(function () {
    'use strict';

    angular
        .module('app')
        .controller('TransportFeePeriodController', TransportFeePeriodController);

    TransportFeePeriodController.$inject = ['$scope', 'adminService', 'Factory', 'loadDataService'];

    function TransportFeePeriodController($scope, adminService, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formTransportFeePeriod.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getPeriodList = function () {
            $scope.dataLoading = true;
            adminService.getTransportFeePeriodList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.periods = response.data.TransportFeePeriods;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getPeriodList();

        $scope.editFeePeriod = function (model) {
            $scope.ValidFrom = new Date(model.ValidFromString2);

            if (model.ValidToString2 != null && model.ValidToString2 != undefined) {
                $scope.ValidTo = new Date(model.ValidToString2);
            }

            $scope.TransportFeePeriodId = model.TransportFeePeriodId;
           // $scope.per.ValidTo = model.ValidTo;
           // $scope.per.TransportFeePeriodId = model.TransportFeePeriodId;
        }

        $scope.SavePeriod = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var period = {
                    ValidFrom: loadDataService.getDateTime($scope.ValidFrom),
                    ValidTo: loadDataService.getDateTime($scope.ValidTo),
                    TransportFeePeriodId: $scope.TransportFeePeriodId,
                }
                $scope.dataLoading = true;
                adminService.savePeriod(period)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Transport Fee Period Saved Successfully!");
                            $scope.getPeriodList();

                            $scope.ValidFrom = "";
                            $scope.ValidTo = "";
                            $scope.TransportFeePeriodId = 0;
                            $scope.IsSubmitted = false;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }
    }
})();
