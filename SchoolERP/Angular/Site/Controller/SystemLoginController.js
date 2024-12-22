(function () {
    'use strict';

    angular
        .module('app')
        .controller('SystemLoginController', SystemLoginController);

    SystemLoginController.$inject = ['$scope', 'siteService'];

    function SystemLoginController($scope, siteService) {
        $scope.$watch('formLogin.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.systemLogin = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                $scope.dataLoading = true;
                siteService.systemLogin($scope.UserId, $scope.Password)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.UserDetail = response.data.UserDetail;
                            siteService.setUserDetail($scope.UserDetail);
                            toastr.success("Login Successful.");
                            if ($scope.UserDetail.Employee.IsSuperAdmin)
                                window.location.href = '/superadmin/layoutsuperadmin';
                            else
                                window.location.href = '/admin/layoutadmin';
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields", "Error");
            }
        }
    }
})();
