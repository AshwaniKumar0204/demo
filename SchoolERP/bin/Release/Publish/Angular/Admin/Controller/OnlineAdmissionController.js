(function () {
    'use strict';

    angular
        .module('app')
        .controller('OnlineAdmissionController', OnlineAdmissionController);

    OnlineAdmissionController.$inject = ["$scope", "adminService", "$location", "Factory"];

    function OnlineAdmissionController($scope, adminService, $location, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.getSearchRegistrationOnlineList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationOnlineList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchRegistrationOnlineList();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.dataLoading = true;
                adminService.getRegistrationOnlineDetail(selected.originalObject.RegistrationOnlineId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.SelectedRegistrationOnline = response.data.RegistrationOnline;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching Registration detail.");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.SelectedRegistrationOnline = {};
            }
        }

        $scope.newAdmission = function () {
            sessionStorage.setItem("registrationOnlineDetail", JSON.stringify($scope.SelectedRegistrationOnline));
            $location.path("NewAdmission");
        }
    }
})();
