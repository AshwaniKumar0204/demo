(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageHostelFeeController', ManageHostelFeeController);

    ManageHostelFeeController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function ManageHostelFeeController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();

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
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getPeriodList();

        $scope.getFeeList = function () {
            $scope.dataLoading = true;
            $scope.periodId = 0;
            if ($scope.HostelFeePeriod != null && $scope.HostelFeePeriod != undefined) {
                $scope.periodId = $scope.HostelFeePeriod.HostelFeePeriodId;
            }

            adminService.HostelFeeList($scope.periodId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.list = response.data.List;
                    } else {
                        $scope.list = [];
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeList();

        $scope.SaveHostelFee = function () {
            $scope.IsSubmitted = true;

            if ($scope.HostelFeePeriod != null && $scope.HostelFeePeriod != undefined) {
                var templist = [];

                for (var i = 0; i < $scope.list.length; i++) {

                    var item = {
                        HostelId: $scope.list[i].HostelId,
                        HostelTitle: $scope.list[i].HostelTitle,
                        FeeList: $scope.list[i].FeeList,
                    }

                    templist.push(item);
                }

                var sendlist = {
                    HostelList: templist,
                    HostelFeePeriodId: $scope.HostelFeePeriod.HostelFeePeriodId
                }

                $scope.dataLoading = true;
                adminService.saveHostelFee(sendlist)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Hostel Fee Head Saved Successfully!");
                            $scope.getFeeList();
                            $scope.IsSubmitted = false;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })
            }
            else {
                toastr.error("Select Hostel Fee Period");
                $scope.dataLoading = false;
            }
        }
    }
})();
