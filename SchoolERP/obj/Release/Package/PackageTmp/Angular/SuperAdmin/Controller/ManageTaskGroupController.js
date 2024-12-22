(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageTaskGroupController', ManageTaskGroupController);

    ManageTaskGroupController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData"];

    function ManageTaskGroupController($scope, superAdminService, loadDataService, ConstantData) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.TaskGroup = {};
        $scope.TaskGroupId = null;
        $scope.getTaskGroupList = function () {
            $scope.dataLoading = true;
            superAdminService.getTaskGroupList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TaskGroupList = response.data.TaskGroupList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getTaskGroupList();

        $scope.$watch('formTaskGroup.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveTaskGroup = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var TaskGroupModel = {
                    TaskGroupId: $scope.TaskGroup.TaskGroupId,
                    GroupName: $scope.TaskGroup.GroupName
                }

                $scope.dataLoading = true;
                superAdminService.saveTaskGroup(TaskGroupModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (TaskGroupModel.TaskGroupId > 0)
                                toastr.success("Task Group Detail Updated Successfully.");
                            else
                                toastr.success("Task Group Created Successfully.");
                            $scope.resetForm();
                            $scope.getTaskGroupList();
                            $('#modal_Student_Charge').modal('hide');
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
        $scope.TaskGroup = [];
        $scope.TaskGroup.TaskGroupId = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.TaskGroup.GroupName = null;
            $scope.TaskGroup.TaskGroupId = null;
            $scope.formTaskGroup.$setPristine();
            $scope.formTaskGroup.$setUntouched();
        }

        $scope.newTaskGroup = function () {
            $scope.resetForm();
            $scope.TaskGroup.GroupName = null;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editTaskGroup = function (TaskGroupModel) {
            console.log(TaskGroupModel);
            $scope.TaskGroup.TaskGroupId = TaskGroupModel.TaskGroupId;
            $scope.TaskGroup.GroupName = TaskGroupModel.GroupName;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (TaskGroupId) {
            $scope.TaskGroupId = TaskGroupId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteTaskGroup = function () {
            $scope.dataLoading = true;
            superAdminService.deleteTaskGroup($scope.TaskGroupId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Task Group Deleted Successfully.")
                        $scope.TaskGroupId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getTaskGroupList();
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
