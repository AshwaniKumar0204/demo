(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageHostelFeePeriodController', ManageHostelFeePeriodController);

    ManageHostelFeePeriodController.$inject = ['$scope', 'adminService', 'Factory', 'loadDataService'];

    function ManageHostelFeePeriodController($scope, adminService, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formHostelFeePeriod.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getPeriodList = function () {
            $scope.dataLoading = true;
            adminService.getHostelFeePeriodList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.periods = response.data.HostelFeePeriods;
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
            $scope.ValidFrom = new Date(model.ValidFrom);
            if (model.ValidTo != null && model.ValidTo != undefined) {
                $scope.ValidTo = new Date(model.ValidTo);
            }
            $scope.HostelFeePeriodId = model.HostelFeePeriodId;
        }

        $scope.SavePeriod = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var period = {
                    ValidFrom: loadDataService.getDateTime($scope.ValidFrom),
                    ValidTo: loadDataService.getDateTime($scope.ValidTo),
                    HostelFeePeriodId: $scope.HostelFeePeriodId,
                }
                $scope.dataLoading = true;
                adminService.savePeriod(period)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Hostel Fee Period Saved Successfully!");
                            $scope.getPeriodList();

                            $scope.ValidFrom = "";
                            $scope.ValidTo = "";
                            $scope.HostelFeePeriodId = 0;
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
