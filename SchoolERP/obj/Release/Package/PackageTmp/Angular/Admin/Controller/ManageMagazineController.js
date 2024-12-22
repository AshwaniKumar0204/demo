(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageMagazineController', ManageMagazineController);

    ManageMagazineController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageMagazineController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.MagazineStatusList = ConstantData.MagazineStatus;

        $scope.Magazine = {};
        $scope.Magazine.MagazineStatus = null;

        $scope.getMagazineList = function () {
            $scope.dataLoading = true;
            adminService.getMagazineList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MagazineList = response.data.MagazineList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMagazineList();

        $scope.$watch('formMagazine.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveMagazine = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var MagazineModel = {
                    MagazineId: $scope.Magazine.MagazineId,
                    MagazineStatus: $scope.Magazine.MagazineStatus,
                    MagazineName: $scope.Magazine.MagazineName,
                }

                $scope.dataLoading = true;
                adminService.saveMagazine(MagazineModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (MagazineModel.MagazineId > 0)
                                toastr.success("Magazine Updated Successfully.");
                            else
                                toastr.success("New Magazine Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getMagazineList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }
        $scope.Magazine = {};
        $scope.Magazine.MagazineStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Magazine.MagazineId = null;
            $scope.Magazine.MagazineName = "";
            $scope.formMagazine.$setPristine();
            $scope.formMagazine.$setUntouched();
        }

        $scope.newMagazine = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editMagazine = function (MagazineModel) {
            $scope.resetForm();
            $scope.Magazine.MagazineName = MagazineModel.MagazineName;
            $scope.Magazine.MagazineId = MagazineModel.MagazineId;
            $scope.Magazine.MagazineStatus = MagazineModel.MagazineStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (MagazineId) {
            $scope.MagazineId = MagazineId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteMagazine = function () {
            $scope.dataLoading = true;
            adminService.deleteMagazine($scope.MagazineId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Magazine deleted successfully.", "Deleted Successfully.")
                        $scope.MagazineId = null;
                        $scope.getMagazineList();
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
