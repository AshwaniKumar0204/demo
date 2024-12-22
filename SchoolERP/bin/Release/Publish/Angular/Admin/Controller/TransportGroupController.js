(function () {
    'use strict';

    angular
        .module('app')
        .controller('TransportGroupController', TransportGroupController);

    TransportGroupController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function TransportGroupController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formTransportGroup.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getGroupList = function () {
            $scope.dataLoading = true;
            adminService.getTransportGroupList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.groups = response.data.TransportGroups;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getGroupList();

        $scope.editGroup = function (model) {
            $scope.TransportGroupTitle = model.TransportGroupTitle;
            $scope.TransportGroupId = model.TransportGroupId;
        }

        $scope.SaveGroup = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var group = {
                    TransportGroupTitle: $scope.TransportGroupTitle,
                    TransportGroupId: $scope.TransportGroupId,
                }
                $scope.dataLoading = true;
                adminService.saveGroup(group)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Transport Fee Period Saved Successfully!");
                            $scope.getGroupList();

                            $scope.TransportGroupTitle = "";
                            $scope.TransportGroupId = 0;
                            $scope.IsSubmitted = false;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }
    }
})();
