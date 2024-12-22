(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStateController', ManageStateController);

    ManageStateController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageStateController($scope, adminService, ConstantData,Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StateStatusList = ConstantData.StateStatus;
        $scope.StateId = null;


         $scope.State = {};
        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStateList();

     
        $scope.$watch('formState.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveState = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StateModel = {
                    StateId: $scope.State.StateId,
                    StateStatus: $scope.State.StateStatus,
                    StateName: $scope.State.StateName,
                }
                console.log(StateModel)
                $scope.dataLoading = true;
                adminService.saveState(StateModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StateModel.StateId > 0)
                                toastr.success("State Updated Successfully.");
                            else
                                toastr.success("New State Created Successfully.");
                            $scope.StateList = response.data.StateList;
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

        $scope.State = {};
        $scope.State.StateId = null;
        $scope.State.StateName = null;
        $scope.State.StateStatus = null;

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.State.StateId = null;
            $scope.State.StateName = null;
            $scope.State.StateStatus = null;
            $scope.formState.$setPristine();
            $scope.formState.$setUntouched();
        }

        $scope.newState = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }
        
        $scope.editState = function (StateModel) {
            $scope.resetForm();
            $scope.State.StateId = StateModel.StateId;
            $scope.State.StateName = StateModel.StateName;
            $scope.State.StateStatus = StateModel.StateStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (StateId) {
            $scope.StateId = StateId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteState = function () {
            $scope.dataLoading = true;
            adminService.deleteState($scope.StateId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("State deleted successfully.", "Deleted Successfully.")
                        $scope.StateList = response.data.StateList;
                        $scope.StateId = null;
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

