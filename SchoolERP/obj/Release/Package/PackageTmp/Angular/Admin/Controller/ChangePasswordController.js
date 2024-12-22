(function () {
    'use strict';

    angular
        .module('app')
        .controller('ChangePasswordController', ChangePasswordController);

    ChangePasswordController.$inject = ["$scope", "adminService"];

    function ChangePasswordController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.ChangePassword = {};
        $scope.$watch('fromStudent.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.changeSystemLoginPassword = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                if ($scope.ChangePassword.NewPassword != $scope.ChangePassword.ConfirmNewPassword) {
                    toastr.error("New Passwords Mismatched");
                    return;
                }
                var ChangePasswordModel = {
                    OldPassword: $scope.ChangePassword.OldPassword,
                    NewPassword: $scope.ChangePassword.NewPassword,
                }
                $scope.dataLoading = true;
                adminService.changeSystemLoginPassword(ChangePasswordModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Password changes successfully.");
                            $scope.resetForm();
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

        $scope.resetForm = function () {
            $scope.ChangePassword = {};
            $scope.IsSubmitted = false;
            $scope.fromStudent.$setPristine();
            $scope.fromStudent.$setUntouched();
        }
    }
})();
