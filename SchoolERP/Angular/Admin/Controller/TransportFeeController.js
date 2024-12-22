(function () {
    'use strict';

    angular
        .module('app')
        .controller('TransportFeeController', TransportFeeController);

    TransportFeeController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function TransportFeeController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();

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
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getPeriodList();

        $scope.getFeeList = function () {
            $scope.dataLoading = true;
            $scope.periodId = 0;
            if ($scope.TransportFeePeriod != null && $scope.TransportFeePeriod != undefined) {
                $scope.periodId = $scope.TransportFeePeriod.TransportFeePeriodId;
            }

            adminService.transportFeeList($scope.periodId)
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

        $scope.SaveTransportFee = function () {
            $scope.IsSubmitted = true;

            if ($scope.TransportFeePeriod != null && $scope.TransportFeePeriod != undefined) {
                var templist = [];

                for (var i = 0; i < $scope.list.length; i++) {

                    var item = {
                        TransportGroupId: $scope.list[i].TransportGroupId,
                        TransportGroupTitle: $scope.list[i].TransportGroupTitle,
                        FeeList: $scope.list[i].FeeList,
                    }

                    templist.push(item);
                }

                var sendlist = {
                    GroupList: templist,
                    TransportFeePeriodId: $scope.TransportFeePeriod.TransportFeePeriodId
                }

                $scope.dataLoading = true;
                adminService.feeSave(sendlist)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Transport Fee Head Saved Successfully!");
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
                toastr.error("Select Transport Fee Period");
                $scope.dataLoading = false;
            }
        }
    }
}) ();
