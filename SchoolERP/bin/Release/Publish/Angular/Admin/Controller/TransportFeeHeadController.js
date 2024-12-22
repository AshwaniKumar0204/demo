(function () {
    'use strict';

    angular
        .module('app')
        .controller('TransportFeeHeadController', TransportFeeHeadController);

    TransportFeeHeadController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function TransportFeeHeadController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getTransportFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.heads = response.data.FeeHeadLists;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeHeadList();

        $scope.SaveFeeHead = function () {
            $scope.IsSubmitted = true;

            var headList = [];

            for (var i = 0; i < $scope.heads.length; i++) {

                var item = {
                    FeeHeadId: $scope.heads[i].FeeHeadId,
                    FeeHeadTitle: $scope.heads[i].FeeHeadTitle,
                    isSelected: $scope.heads[i].isSelected,
                };

                if (item.isSelected)
                    headList.push(item);

                //tasks.push($scope.taskList.ChildMenu[i]);
            }

            $scope.dataLoading = true;
            adminService.feeHeadSave(headList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Transport Fee Head Saved Successfully!");
                        $scope.getFeeHeadList();
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
    }
})();
