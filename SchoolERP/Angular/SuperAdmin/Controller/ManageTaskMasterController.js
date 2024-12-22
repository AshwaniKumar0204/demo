(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageTaskMasterController', ManageTaskMasterController);

    ManageTaskMasterController.$inject = ["$scope", "superAdminService", "loadDataService", "ConstantData"];

    function ManageTaskMasterController($scope, superAdminService, loadDataService, ConstantData) {
        $scope.checkSystemLogin();
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.TaskMaster = {};
        $scope.TaskGroupId = null;
        $scope.TaskMasterStatusList = ConstantData.TaskMasterStatus;

        $scope.getTaskMasterList = function () {
            $scope.dataLoading = true;
            superAdminService.getTaskMasterList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainTaskMasterList = response.data.TaskMasterList;
                        $scope.TaskMasterList = response.data.TaskMasterList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getTaskMasterList();

        $scope.changeTaskGroup = function () {
            if ($scope.TaskGroupId == null || $scope.TaskGroupId == undefined) {
                $scope.TaskMasterList = $scope.MainTaskMasterList;
            } else {
                $scope.TaskMasterList = [];
                angular.forEach($scope.MainTaskMasterList, function (taskMaster, key1) {
                    if (taskMaster.TaskGroupId == $scope.TaskGroupId) {
                        $scope.TaskMasterList.push(taskMaster);
                    }
                });
            }
        }

        $scope.getTaskGroupList = function () {
            $scope.dataLoading = true;
            superAdminService.getTaskGroupList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TaskGroupList = response.data.TaskGroupList;
                        var index = { TaskGroupId: null, GroupName: "--Select Task Group--" };
                        $scope.TaskGroupList.unshift(index);
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


        $scope.$watch('formTaskMaster.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveTaskMaster = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var TaskMasterModel = {
                    TaskMasterId: $scope.TaskMaster.TaskMasterId,
                    TaskMasterStatus: $scope.TaskMaster.TaskMasterStatus,
                    TaskGroupId: $scope.TaskMaster.TaskGroupId,
                    ControllerName: $scope.TaskMaster.ControllerName,
                    ActionName: $scope.TaskMaster.ActionName
                }

                $scope.dataLoading = true;
                superAdminService.saveTaskMaster(TaskMasterModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (TaskMasterModel.TaskMasterId > 0)
                                toastr.success("Task Detail Updated Successfully.");
                            else
                                toastr.success("Task Created Successfully.");
                            $scope.resetForm();
                            $scope.MainTaskMasterList = response.data.TaskMasterList;
                            $('#modal_Student_Charge').modal('hide');
                            $scope.changeTaskGroup();
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
        $scope.TaskMaster = [];
        $scope.TaskMaster.TaskGroupId = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            //$scope.TaskMaster.ControllerName = null;
            $scope.TaskMaster.ActionName = null;
            $scope.TaskMaster.TaskMasterId = null;
            $scope.formTaskMaster.$setPristine();
            $scope.formTaskMaster.$setUntouched();
        }

        $scope.newTaskMaster = function () {
            $scope.resetForm();
            $scope.TaskMaster.TaskMasterStatus = $scope.TaskMasterStatusList[1].Key;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editTaskMaster = function (TaskMasterModel) {
            $scope.TaskMaster = TaskMasterModel;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (TaskMasterId) {
            $scope.TaskMasterId = TaskMasterId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteTaskMaster = function () {
            $scope.dataLoading = true;
            superAdminService.deleteTaskMaster($scope.TaskMasterId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Task deleted successfully.", "Deleted Successfully.")
                        $scope.MainTaskMasterList = response.data.TaskMasterList;
                        $scope.TaskMasterId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.changeTaskGroup();
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
