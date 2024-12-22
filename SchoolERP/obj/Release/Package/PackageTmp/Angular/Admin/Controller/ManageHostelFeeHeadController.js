(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageHostelFeeHeadController', ManageHostelFeeHeadController);

    ManageHostelFeeHeadController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function ManageHostelFeeHeadController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getHostelFeeHeadList()
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
            adminService.saveHostelFeeHead(headList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Hostel Fee Head Saved Successfully!");
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
