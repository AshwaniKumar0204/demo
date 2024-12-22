(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageCityController', ManageCityController);

    ManageCityController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageCityController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.CityStatusList = ConstantData.CityStatus;
        $scope.StateId = null;
        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                        var index = { StateId: null, StateName: '--Select State--' };
                        $scope.StateList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getStateList();

        $scope.getCityList = function () {
            $scope.dataLoading = true;
            adminService.getCityList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainCityList = response.data.CityList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getCityList();

        $scope.CityList = [];
        $scope.filterCity = function () {
            $scope.CityList = [];
            if ($scope.City.StateId > 0) {
                angular.forEach($scope.MainCityList, function (city, key) {
                    if (city.StateId == $scope.City.StateId) {
                        $scope.CityList.push(city);
                    }
                });
            }
        }

        $scope.$watch('formCity.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveCity = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var CityModel = {
                    CityId: $scope.City.CityId,
                    CityStatus: $scope.City.CityStatus,
                    CityName: $scope.City.CityName,
                    StateId: $scope.City.StateId,
                }

                $scope.dataLoading = true;
                adminService.saveCity(CityModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (CityModel.CityId > 0)
                                toastr.success("City Updated Successfully.");
                            else
                                toastr.success("New City Created Successfully.");
                            $scope.MainCityList = response.data.CityList;
                            $scope.filterCity();
                            $('#modal_Student_Charge').modal('hide');
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

        $scope.City = {};
        $scope.City.CityId = null;
        $scope.City.CityName = null;
        $scope.City.StateId = null;
        $scope.City.CityStatus = null;

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.City.CityId = null;
            $scope.City.CityName = null;
            $scope.City.CityStatus = null;
            $scope.formCity.$setPristine();
            $scope.formCity.$setUntouched();
        }

        $scope.newCity = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editCity = function (CityModel) {
            $scope.resetForm();
            $scope.City.StateId = CityModel.StateId;
            $scope.City.CityId = CityModel.CityId;
            $scope.City.CityName = CityModel.CityName;
            $scope.City.CityStatus = CityModel.CityStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (CityId) {
            $scope.CityId = CityId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteCity = function () {
            $scope.dataLoading = true;
            adminService.deletecity($scope.CityId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("City deleted successfully.", "Deleted Successfully.")
                        $scope.MainCityList = response.data.CityList;
                        $scope.filterCity();
                        $scope.CityId = null;
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }

})();

