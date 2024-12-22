(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageHostelController', ManageHostelController);

    ManageHostelController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function ManageHostelController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formHostel.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getHostelList = function () {
            $scope.dataLoading = true;
            adminService.getHostelList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Hostels = response.data.Hostels;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getHostelList();

        $scope.editHostel = function (model) {
            $scope.HostelTitle = model.HostelTitle;
            $scope.HostelId = model.HostelId;
        }

        $scope.SaveHostel = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var Hostel = {
                    HostelTitle: $scope.HostelTitle,
                    HostelId: $scope.HostelId,
                }
                $scope.dataLoading = true;
                adminService.saveHostel(Hostel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Hostel Saved Successfully!");
                            $scope.getHostelList();

                            $scope.HostelTitle = "";
                            $scope.HostelId = 0;
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
