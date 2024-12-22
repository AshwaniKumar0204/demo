(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStaffGroupController', ManageStaffGroupController);

    ManageStaffGroupController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageStaffGroupController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.currentPage2 = 1;
        $scope.pageSize2 = $scope.sizeOptions[0];
        $scope.sort2 = function (key) {
            $scope.sortKey2 = key;
            $scope.reverse2 = !$scope.reverse2;
        };

        $scope.StatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;

        $scope.getStaffList = function () {
            $scope.dataLoading = true;
            adminService.getStaffList(1, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffList = response.data.StaffList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffList();

        $scope.getStaffGroupList = function () {
            $scope.dataLoading = true;
            adminService.getStaffGroupList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StaffGroupList = response.data.StaffGroupList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStaffGroupList();

        $scope.$watch('formStaffGroup.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.selectAll = function () {
            angular.forEach($scope.StaffList, function (staff) {
                if ($scope.IsSelectedAll)
                    staff.IsSelected = true;
                else
                    staff.IsSelected = false;
            })
        }

        $scope.saveStaffGroup = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var StaffGroupModel = {
                    StaffGroupId: $scope.StaffGroup.StaffGroupId,
                    GroupName: $scope.StaffGroup.GroupName,
                    Status: $scope.StaffGroup.Status,
                    StaffGroupDetailList: $scope.StaffList
                }

                $scope.dataLoading = true;
                adminService.saveStaffGroup(StaffGroupModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (StaffGroupModel.StaffGroupId > 0)
                                toastr.success("Staff Group Updated Successfully.");
                            else
                                toastr.success("New Staff Group Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStaffGroupList();
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.StaffGroup = {};
            angular.forEach($scope.StaffList, function (staff) {
                staff.IsSelected = false;
                staff.StaffGroupDetailId = null;
            })
            $scope.formStaffGroup.$setPristine();
            $scope.formStaffGroup.$setUntouched();
        }

        $scope.newStaffGroup = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editStaffGroup = function (StaffGroupModel) {
            $scope.resetForm();
            $scope.StaffGroup = StaffGroupModel;
            angular.forEach(StaffGroupModel.StaffGroupDetailList, function (staff1) {
                for (var i = 0; i < $scope.StaffList.length; i++) {
                    if (staff1.StaffId == $scope.StaffList[i].StaffId) {
                        $scope.StaffList[i].IsSelected = true;
                        $scope.StaffList[i].StaffGroupDetailId = staff1.StaffGroupDetailId;
                        break;
                    }
                }
            })
            $('#modal_Student_Charge').modal('show');
        }

        $scope.viewStaffDetail = function (staffGroup) {
            $scope.StaffGroupDetailList = [];
            angular.forEach(staffGroup.StaffGroupDetailList, function (staff1) {
                for (var i = 0; i < $scope.StaffList.length; i++) {
                    if (staff1.StaffId == $scope.StaffList[i].StaffId) {
                        $scope.StaffGroupDetailList.push($scope.StaffList[i]);
                        break;
                    }
                }
            })
            console.log($scope.StaffGroupDetailList);
            $('#modal_detail').modal('show');
        }

        $scope.deleteConfirmation = function (StaffGroupId) {
            $scope.StaffGroupId = StaffGroupId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteStaffGroup = function () {
            $scope.dataLoading = true;
            adminService.deleteStaffGroup($scope.StaffGroupId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Staff Group deleted successfully.", "Deleted Successfully.")
                        $scope.StaffGroupId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getStaffGroupList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
